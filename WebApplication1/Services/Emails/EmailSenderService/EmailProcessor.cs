using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Services.Emails.TemplateService;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.Emails.TemplateService.Enitities;

namespace WebApplication1.Services.Emails.EmailService
{
    internal class EmailProcessor : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<EmailProcessor> _logger;

        // ── Tune these for testing ──
        private static readonly TimeSpan StuckThreshold = TimeSpan.FromMinutes(5);
        private const int BatchSize = 10;

        public EmailProcessor(IServiceProvider services, ILogger<EmailProcessor> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailProcessor starting up. Stuck threshold = {T} min",
                StuckThreshold.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var queueRepo = scope.ServiceProvider.GetRequiredService<IEmailQueueRepository>();
                    var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
                    var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

                    // ── 1. Recover stuck emails FIRST ──
                    var recovered = await queueRepo.RecoverStuckEmailsAsync(StuckThreshold, stoppingToken);
                    if (recovered > 0)
                        _logger.LogWarning("Recovered {Count} email(s) stuck in Processing state.", recovered);

                    // ── 2. NO DELETES (disabled for testing) ──
                    // await queueRepo.DeleteSentEmailsAsync(TimeSpan.FromDays(14));
                    // await queueRepo.DeleteStaleEmailsAsync(TimeSpan.FromDays(30));

                    // ── 3. Fetch pending batch ──
                    var emails = await queueRepo.GetPendingAsync(BatchSize);

                    _logger.LogInformation("Fetched {Count} pending email(s).", emails.Count);

                    foreach (var email in emails)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        await ProcessSingleEmailAsync(
                            email, queueRepo, templateService, emailSender, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in email processor main loop");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (OperationCanceledException) { break; }
            }

            _logger.LogInformation("EmailProcessor stopped.");
        }

        private async Task ProcessSingleEmailAsync(
            QueuedEmail email,
            IEmailQueueRepository queueRepo,
            IEmailTemplateService templateService,
            IEmailSenderService emailSender,
            CancellationToken stoppingToken)
        {
            var startedAt = DateTime.UtcNow;

            try
            {
                // ── Mark as Processing ──
                email.Status = EmailQueueStatus.Processing;
                email.LastAttemptAt = startedAt;
                await queueRepo.UpdateAsync(email);

                _logger.LogInformation(
                    "[{Id}] Marked Processing. To={To} Retry={Retry}",
                    email.Id, email.To, email.RetryCount);

                // ── Deserialize ──
                var (deserializedModel, bccList) = DeserializeQueuedEmail(email);

                _logger.LogInformation(
                    "[{Id}] Deserialized model. Bcc={BccCount}",
                    email.Id, bccList.Count);

                // ── Render ──
                var renderStart = DateTime.UtcNow;
                var body = await templateService.RenderEmailTemplateAsync(
                    (AllEmailsTemplateModel)deserializedModel,
                    email.TemplateName);

                _logger.LogInformation(
                    "[{Id}] Rendered template in {Ms}ms. BodyLength={Len}",
                    email.Id, (DateTime.UtcNow - renderStart).TotalMilliseconds, body?.Length ?? 0);

                // ── Send ──
                // IMPORTANT: not linking stoppingToken here — we want the send
                // to finish even if shutdown is requested. It should only take seconds.
                var sendStart = DateTime.UtcNow;
                _logger.LogInformation("[{Id}] SMTP START at {Time:O}", email.Id, sendStart);

                var sendResult = await emailSender.SendEmailAsync(
                    email.To, email.Subject, body, bccList, true);

                var sendMs = (DateTime.UtcNow - sendStart).TotalMilliseconds;
                _logger.LogInformation(
                    "[{Id}] SMTP END at {Time:O} ({Ms}ms)",
                    email.Id, DateTime.UtcNow, sendMs);

                // ── CRITICAL: check the result. Sender can fail without throwing. ──
                if (!sendResult.Success)
                {
                    throw new Exception($"Email sender reported failure: {sendResult.Message}");
                }

                // ── Mark as Sent (only reached when send actually succeeded) ──
                email.Status = EmailQueueStatus.Sent;
                email.SentAt = DateTime.UtcNow;
                email.ErrorMessage = null;
                await queueRepo.UpdateAsync(email);

                _logger.LogInformation(
                    "[{Id}] SUCCESS. Sent to {To} in {TotalMs}ms (BCC: {BccCount})",
                    email.Id, email.To,
                    (DateTime.UtcNow - startedAt).TotalMilliseconds,
                    bccList.Count);
            }
            catch (JsonException ex)
            {
                email.RetryCount = QueuedEmail.MaxRetryAttempts;
                email.ErrorMessage = $"Malformed JSON: {ex.Message}";
                email.Status = EmailQueueStatus.Failed;
                await queueRepo.UpdateAsync(email);

                _logger.LogError(ex, "[{Id}] Malformed JSON, marking Failed.", email.Id);
            }
            catch (Exception ex)
            {
                email.RetryCount++;
                email.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";
                email.Status = email.RetryCount >= QueuedEmail.MaxRetryAttempts
                    ? EmailQueueStatus.Failed
                    : EmailQueueStatus.Pending;

                if (email.Status == EmailQueueStatus.Pending)
                {
                    var delayMinutes = Math.Min(Math.Pow(2, email.RetryCount), 60);
                    email.ScheduledFor = DateTime.UtcNow.AddMinutes(delayMinutes);

                    _logger.LogWarning(ex,
                        "[{Id}] Send FAILED. Retry #{Retry} scheduled in {Delay} min.",
                        email.Id, email.RetryCount, delayMinutes);
                }
                else
                {
                    _logger.LogError(ex,
                        "[{Id}] Send FAILED permanently after {Retry} attempts.",
                        email.Id, email.RetryCount);
                }

                await queueRepo.UpdateAsync(email);
            }
        }

        private (object model, List<string> bcc) DeserializeQueuedEmail(QueuedEmail email)
        {
            var modelType = Type.GetType(email.TemplateModelType)
                ?? throw new InvalidOperationException(
                    $"Could not resolve template model type '{email.TemplateModelType}'.");

            var json = email.TemplateModelJson;
            if (string.IsNullOrWhiteSpace(json))
                throw new JsonException($"TemplateModelJson is empty for email {email.Id}.");

            using var doc = JsonDocument.Parse(json);
            var bccList = new List<string>();

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("model", out var modelElement))
            {
                if (doc.RootElement.TryGetProperty("bccList", out var bccElement) &&
                    bccElement.ValueKind == JsonValueKind.Array)
                {
                    bccList = bccElement.EnumerateArray()
                        .Where(x => x.ValueKind == JsonValueKind.String)
                        .Select(x => x.GetString())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()!;
                }

                var model = JsonSerializer.Deserialize(modelElement.GetRawText(), modelType);
                if (model is null)
                    throw new InvalidOperationException(
                        $"Deserialized model was null for email {email.Id}.");

                return (model, bccList);
            }

            var legacyModel = JsonSerializer.Deserialize(json, modelType);
            if (legacyModel is null)
                throw new InvalidOperationException(
                    $"Deserialized legacy model was null for email {email.Id}.");

            return (legacyModel, bccList);
        }
    }
}
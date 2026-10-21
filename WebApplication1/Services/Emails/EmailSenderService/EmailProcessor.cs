using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.Services.Emails.EmailService.Entities;

namespace WebApplication1.Services.Emails.EmailService
{
    internal class EmailProcessor : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<EmailProcessor> _logger;

        // Must be LARGER than worst-case SendEmailAsync.
        private static readonly TimeSpan StuckThreshold = TimeSpan.FromMinutes(15);
        private const int BatchSize = 10;
        private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(30);

        public EmailProcessor(IServiceProvider services, ILogger<EmailProcessor> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "EmailProcessor starting. Stuck threshold = {T} min, batch = {B}",
                StuckThreshold.TotalMinutes, BatchSize);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 1. Recover stuck — short DB scope
                    using (var scope = _services.CreateScope())
                    {
                        var queueRepo = scope.ServiceProvider
                            .GetRequiredService<IEmailQueueRepository>();
                        var recovered = await queueRepo
                            .RecoverStuckEmailsAsync(StuckThreshold, stoppingToken);
                        if (recovered > 0)
                            _logger.LogWarning(
                                "Recovered {Count} email(s) stuck in Processing.", recovered);
                    }

                    // 2. Fetch IDs — short DB scope
                    List<Guid> pendingIds;
                    using (var scope = _services.CreateScope())
                    {
                        var queueRepo = scope.ServiceProvider
                            .GetRequiredService<IEmailQueueRepository>();
                        pendingIds = await queueRepo.GetPendingIdsAsync(BatchSize);
                    }

                    if (pendingIds.Count == 0)
                    {
                        _logger.LogDebug("No pending emails.");
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Fetched {Count} pending email(s).", pendingIds.Count);

                        foreach (var id in pendingIds)
                        {
                            if (stoppingToken.IsCancellationRequested) break;
                            await ProcessSingleEmailAsync(id, stoppingToken);
                        }
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

                try { await Task.Delay(IdleDelay, stoppingToken); }
                catch (OperationCanceledException) { break; }
            }

            _logger.LogInformation("EmailProcessor stopped.");
        }

        private async Task ProcessSingleEmailAsync(Guid emailId, CancellationToken stoppingToken)
        {
            var startedAt = DateTime.UtcNow;

            try
            {
                // ── A. CLAIM (short scope) ──
                QueuedEmail? email;
                using (var scope = _services.CreateScope())
                {
                    var queueRepo = scope.ServiceProvider
                        .GetRequiredService<IEmailQueueRepository>();
                    email = await queueRepo
                        .TryClaimPendingAsync(emailId, startedAt, stoppingToken);

                    if (email is null) return; // already taken or not pending

                    _logger.LogInformation(
                        "[{Id}] Claimed. To={To} Retry={Retry}",
                        email.Id, email.To, email.RetryCount);
                }

                // ── B. RENDER (short scope) ──
                string body;
                List<string> bccList;
                try
                {
                    using var scope = _services.CreateScope();
                    var templateService = scope.ServiceProvider
                        .GetRequiredService<IEmailTemplateService>();

                    var (model, bcc) = DeserializeQueuedEmail(email);
                    bccList = bcc;

                    var renderStart = DateTime.UtcNow;
                    body = await templateService.RenderEmailTemplateAsync(
                        (AllEmailsTemplateModel)model, email.TemplateName);

                    _logger.LogInformation(
                        "[{Id}] Rendered in {Ms}ms (BodyLength={Len})",
                        email.Id,
                        (DateTime.UtcNow - renderStart).TotalMilliseconds,
                        body?.Length ?? 0);
                }
                catch (Exception ex)
                {
                    await MarkFailedAsync(email,
                        ex,
                        permanent: ex is JsonException
                                   || ex is InvalidOperationException,
                        ct: stoppingToken);
                    return;
                }

                // ── C. SEND (NO DB scope) ──
                var sendStart = DateTime.UtcNow;
                _logger.LogInformation(
                    "[{Id}] SEND START at {Time:O}", email.Id, sendStart);

                EmailResult sendResult;
                try
                {
                    using var scope = _services.CreateScope();
                    var emailSender = scope.ServiceProvider
                        .GetRequiredService<IEmailSenderService>();

                    sendResult = await emailSender.SendEmailAsync(
                        email.To, email.Subject, body, bccList, true);
                }
                catch (Exception ex)
                {
                    await MarkFailedAsync(email, ex, permanent: false, ct: stoppingToken);
                    return;
                }

                _logger.LogInformation(
                    "[{Id}] SEND END at {Time:O} ({Ms}ms)",
                    email.Id, DateTime.UtcNow,
                    (DateTime.UtcNow - sendStart).TotalMilliseconds);

                if (!sendResult.Success)
                {
                    await MarkFailedAsync(
                        email,
                        new Exception($"Sender reported failure: {sendResult.Message}"),
                        permanent: false,
                        ct: stoppingToken);
                    return;
                }

                // ── D. MARK SENT (short scope) ──
                using (var scope = _services.CreateScope())
                {
                    var queueRepo = scope.ServiceProvider
                        .GetRequiredService<IEmailQueueRepository>();
                    await queueRepo.MarkSentAsync(email.Id, DateTime.UtcNow, stoppingToken);
                }

                _logger.LogInformation(
                    "[{Id}] SUCCESS → {To} in {TotalMs}ms (BCC: {BccCount})",
                    email.Id, email.To,
                    (DateTime.UtcNow - startedAt).TotalMilliseconds,
                    bccList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "[{Id}] Unhandled error in ProcessSingleEmailAsync.", emailId);
            }
        }

        private async Task MarkFailedAsync(
            QueuedEmail email, Exception ex, bool permanent, CancellationToken ct)
        {
            try
            {
                using var scope = _services.CreateScope();
                var queueRepo = scope.ServiceProvider
                    .GetRequiredService<IEmailQueueRepository>();

                var nextRetry = email.RetryCount + 1;

                if (permanent || nextRetry >= QueuedEmail.MaxRetryAttempts)
                {
                    await queueRepo.MarkFailedAsync(
                        email.Id, nextRetry,
                        $"{ex.GetType().Name}: {ex.Message}", ct);

                    _logger.LogError(ex,
                        "[{Id}] FAILED permanently after {Retry} attempt(s).",
                        email.Id, nextRetry);
                }
                else
                {
                    var delayMinutes = Math.Min(Math.Pow(2, nextRetry), 60);
                    var scheduledFor = DateTime.UtcNow.AddMinutes(delayMinutes);

                    await queueRepo.MarkRetryAsync(
                        email.Id, nextRetry, scheduledFor,
                        $"{ex.GetType().Name}: {ex.Message}", ct);

                    _logger.LogWarning(ex,
                        "[{Id}] FAILED. Retry #{Retry} scheduled +{Delay} min.",
                        email.Id, nextRetry, delayMinutes);
                }
            }
            catch (Exception inner)
            {
                _logger.LogError(inner,
                    "[{Id}] Could not persist failure state — may be retried.",
                    email.Id);
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
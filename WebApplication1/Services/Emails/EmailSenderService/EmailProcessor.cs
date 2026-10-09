using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public EmailProcessor(IServiceProvider services, ILogger<EmailProcessor> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var queueRepo = scope.ServiceProvider.GetRequiredService<IEmailQueueRepository>();
                        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();
                        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

                        var emails = await queueRepo.GetPendingAsync(10);

                        await queueRepo.DeleteSentEmails();

                        foreach (var email in emails)
                        {
                            try
                            {
                                // Mark as processing
                                email.Status = EmailQueueStatus.Processing;
                                email.LastAttemptAt = DateTime.UtcNow;
                                await queueRepo.UpdateAsync(email);

                                // Deserialize model + optional BCC.
                                // Supports three shapes:
                                //   1. legacy: raw model JSON, no envelope  → no BCC
                                //   2. envelope without bccList             → no BCC
                                //   3. envelope with bccList                → BCC recipients
                                var (deserializedModel, bccList) = DeserializeQueuedEmail(email);

                                // Render and send
                                var body = await templateService.RenderEmailTemplateAsync(
                                    (AllEmailsTemplateModel)deserializedModel,
                                    email.TemplateName);

                                await emailSender.SendEmailAsync(
                                    email.To,
                                    email.Subject,
                                    body,
                                    bccList,
                                    true);

                                // Mark as sent
                                email.Status = EmailQueueStatus.Sent;
                                email.SentAt = DateTime.UtcNow;
                                await queueRepo.UpdateAsync(email);

                                _logger.LogInformation(
                                    "Sent email {Id} to {To} (BCC: {BccCount})",
                                    email.Id, email.To, bccList.Count);
                            }
                            catch (JsonException ex)
                            {
                                // Malformed JSON — don't burn retries on a permanent failure
                                email.RetryCount = QueuedEmail.MaxRetryAttempts;
                                email.ErrorMessage = $"Malformed TemplateModelJson: {ex.Message}";
                                email.Status = EmailQueueStatus.Failed;
                                await queueRepo.UpdateAsync(email);

                                _logger.LogError(ex,
                                    "Email {Id} has malformed JSON, marking failed", email.Id);
                            }
                            catch (Exception ex)
                            {
                                email.RetryCount++;
                                email.ErrorMessage = ex.Message;
                                email.Status = email.RetryCount >= QueuedEmail.MaxRetryAttempts
                                    ? EmailQueueStatus.Failed
                                    : EmailQueueStatus.Pending;

                                // Exponential backoff for retries
                                if (email.Status == EmailQueueStatus.Pending)
                                {
                                    email.ScheduledFor = DateTime.UtcNow.AddMinutes(Math.Pow(2, email.RetryCount));
                                }

                                await queueRepo.UpdateAsync(email);

                                _logger.LogError(ex,
                                    "Failed to send email {Id} to {To}, retry {RetryCount}",
                                    email.Id, email.To, email.RetryCount);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in email processor");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        /// <summary>
        /// Deserializes <see cref="QueuedEmail.TemplateModelJson"/> into the
        /// concrete template model, and pulls out any BCC list.
        ///
        /// Handles:
        ///   * Legacy rows where the whole JSON is the model.
        ///   * New rows where the JSON is <c>{ "model": ..., "bccList": [...] }</c>.
        /// </summary>
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

            // Envelope shape: JSON object with a "model" property
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("model", out var modelElement))
            {
                // Extract BCC if present. Key name matches the writer.
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

            // Legacy shape: the whole JSON is the model
            var legacyModel = JsonSerializer.Deserialize(json, modelType);
            if (legacyModel is null)
                throw new InvalidOperationException(
                    $"Deserialized legacy model was null for email {email.Id}.");

            return (legacyModel, bccList);
        }
    }
}
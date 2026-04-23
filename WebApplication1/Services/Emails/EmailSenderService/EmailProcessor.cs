using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using WebApplication1.Services.Emails.TemplateService;
using WebApplication1.Services.Emails.EmailSenderService.Sender;
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

                                // Deserialize model
                                var modelType = Type.GetType(email.TemplateModelType);
                                var model = JsonSerializer.Deserialize(email.TemplateModelJson, modelType);

                                // Render and send
                                var body = await templateService.RenderEmailTemplateAsync((AllEmailsTemplateModel)model, email.TemplateName);
                                await emailSender.SendEmailAsync(email.To, email.Subject, body);

                                // Mark as sent
                                email.Status = EmailQueueStatus.Sent;
                                email.SentAt = DateTime.UtcNow;
                                await queueRepo.UpdateAsync(email);

                                _logger.LogInformation("Sent email {Id} to {To}", email.Id, email.To);
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

                                _logger.LogError(ex, "Failed to send email {Id} to {To}, retry {RetryCount}",
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
    }
}

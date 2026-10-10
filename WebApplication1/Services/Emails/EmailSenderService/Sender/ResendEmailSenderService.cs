using Resend;
using Microsoft.Extensions.Options;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService;

// Resolve name clashes between Resend SDK types and your own entities
using ResendEmailMessage = Resend.EmailMessage;
using AppEmailMessage = WebApplication1.Services.Emails.EmailService.Entities.EmailMessage;
using AppEmailAttachment = WebApplication1.Services.Emails.EmailService.Entities.EmailAttachment;

public class ResendEmailSenderService : IEmailSenderService
{
    private readonly ResendClient _client;
    private readonly ILogger<ResendEmailSenderService> _logger;
    private readonly EmailSettings _settings;

    public ResendEmailSenderService(
        ResendClient client,
        IOptions<EmailSettings> settings,
        ILogger<ResendEmailSenderService> logger)
    {
        _client = client;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<EmailResult> SendEmailAsync(AppEmailMessage message)
    {
        try
        {
            var msg = new ResendEmailMessage
            {
                From = $"{_settings.SenderName} <{_settings.SenderEmail}>",
                To = new[] { message.To },
                Subject = message.Subject,
                HtmlBody = message.IsHtml ? message.Body : null,
                TextBody = message.IsHtml ? null : message.Body,
                Bcc = message.Bcc?.ToArray()
            };

            await _client.EmailSendAsync(msg);

            _logger.LogInformation("Resend: sent email to {To}", message.To);

            return new EmailResult
            {
                Success = true,
                Message = "Sent via Resend"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resend send failed for {To}", message.To);

            return new EmailResult
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<EmailResult> SendEmailAsync(
        string to, string subject, string body,
        List<string>? Bcc = null, bool isHtml = true)
    {
        var message = new AppEmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            Cc = new List<string>(),
            Bcc = Bcc ?? new List<string>(),
            Attachments = new List<AppEmailAttachment>()   // ← alias used here
        };

        return await SendEmailAsync(message);
    }
}
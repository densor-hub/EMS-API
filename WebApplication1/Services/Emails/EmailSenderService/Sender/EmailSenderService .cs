using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication1.Services.Emails.EmailService.Entities;
using System.Net.Mail;
using System.Net.Mime;
using WebApplication1.Services.Emails.EmailService;

namespace WebApplication1.Services.Emails.EmailSenderService.Sender
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IOptions<EmailSettings> emailSettings, ILogger<EmailSenderService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<EmailResult> SendEmailAsync(EmailMessage message)
        {
            try
            {
                _logger.LogInformation("Attempting to send email via {Server}:{Port} with user {Username}",
                    _emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.SmtpUsername);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(message.To));
                email.Subject = message.Subject;

                var builder = new BodyBuilder();
                if (message.IsHtml)
                    builder.HtmlBody = message.Body;
                else
                    builder.TextBody = message.Body;

                // Add attachments if any
                //if (message.Attachments != null && message.Attachments.Any())
                //{
                //    foreach (var attachment in message.Attachments)
                //    {
                //        builder.Attachments.Add(attachment.FileName, attachment.Content,
                //           (MimeKit.ContentType) attachment.ContentType);
                //    }
                //}

                email.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                // Accept all SSL certificates (for development only)
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                // Remove spaces from password if present
                var password = _emailSettings.SmtpPassword.Replace(" ", "");

                await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", message.To);

                return new EmailResult
                {
                    Success = true,
                    Message = "Email sent successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", message.To);

                return new EmailResult
                {
                    Success = false,
                    Message = $"Failed to send email: {ex.Message}"
                };
            }
        }

        public async Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var message = new EmailMessage
            {
                To = to,
                Subject = subject,
                Body = body,
                IsHtml = isHtml,
                Cc = new List<string>(),
                Bcc = new List<string>(),
                Attachments = new List<EmailAttachment>()
            };

            return await SendEmailAsync(message);
        }
    }
}
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using System.Net;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService;

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
        // Try multiple connection methods (great for mobile networks)
        var connectionMethods = new List<Func<Task<EmailResult>>>
        {
            () => SendWithPort587StartTlsAsync(message),
            () => SendWithPort465SslAsync(message),
            () => SendWithPort25StartTlsAsync(message),
            () => SendWithAltServerAsync(message),
            () => SendWithSystemNetMailAsync(message)
        };

        foreach (var method in connectionMethods)
        {
            try
            {
                var result = await method();
                if (result.Success)
                    return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Connection method failed, trying next...");
                await Task.Delay(1000);
            }
        }

        return new EmailResult
        {
            Success = false,
            Message = "All email sending methods failed. Your mobile network may be blocking SMTP ports."
        };
    }

    private async Task<EmailResult> SendWithPort587StartTlsAsync(EmailMessage message)
    {
        return await SendWithSpecificSettingsAsync(message, "smtp.gmail.com", 587, SecureSocketOptions.StartTls, "Port 587");
    }

    private async Task<EmailResult> SendWithPort465SslAsync(EmailMessage message)
    {
        return await SendWithSpecificSettingsAsync(message, "smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect, "Port 465");
    }

    private async Task<EmailResult> SendWithPort25StartTlsAsync(EmailMessage message)
    {
        return await SendWithSpecificSettingsAsync(message, "smtp.gmail.com", 25, SecureSocketOptions.StartTls, "Port 25");
    }

    private async Task<EmailResult> SendWithAltServerAsync(EmailMessage message)
    {
        // Try Google's alternative SMTP servers
        var altServers = new[] {
            "alt1.smtp.mail.gmail.com",
            "alt2.smtp.mail.gmail.com",
            "alt3.smtp.mail.gmail.com",
            "alt4.smtp.mail.gmail.com"
        };

        foreach (var server in altServers)
        {
            try
            {
                _logger.LogInformation("Attempting with alt server: {Server}", server);
                return await SendWithSpecificSettingsAsync(message, server, 587, SecureSocketOptions.StartTls, $"Alt Server {server}");
            }
            catch
            {
                // Try next alt server
            }
        }

        throw new Exception("All alt servers failed");
    }

    private async Task<EmailResult> SendWithSpecificSettingsAsync(
        EmailMessage message,
        string server,
        int port,
        SecureSocketOptions sslOptions,
        string methodName)
    {
        try
        {
            _logger.LogInformation("Attempting {Method}: {Server}:{Port} with {Ssl}",
                methodName, server, port, sslOptions);

            var password = _emailSettings.SmtpPassword?.Replace(" ", "") ?? "";
            var email = CreateMimeMessage(message);

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            smtp.Timeout = 30000;
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            // Try to connect
            await smtp.ConnectAsync(server, port, sslOptions);

            if (!smtp.IsConnected)
                throw new Exception("Failed to connect");

            // Authenticate
            await smtp.AuthenticateAsync(_emailSettings.SmtpUsername, password);

            if (!smtp.IsAuthenticated)
                throw new Exception("Failed to authenticate");

            // Send
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("✓ Email sent successfully via {Method} to {Recipient}", methodName, message.To);

            return new EmailResult
            {
                Success = true,
                Message = $"Email sent via {methodName}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ {Method} failed", methodName);
            throw;
        }
    }

    private async Task<EmailResult> SendWithSystemNetMailAsync(EmailMessage message)
    {
        try
        {
            _logger.LogInformation("Attempting with System.Net.Mail");

            var password = _emailSettings.SmtpPassword?.Replace(" ", "") ?? "";

            // Try different ports
            var ports = new[] { 587, 465, 25 };

            foreach (var port in ports)
            {
                try
                {
                    using var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", port)
                    {
                        EnableSsl = port != 25,
                        Credentials = new NetworkCredential(_emailSettings.SmtpUsername, password),
                        Timeout = 30000,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false
                    };

                    using var mail = new System.Net.Mail.MailMessage
                    {
                        From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                        Subject = message.Subject,
                        Body = message.Body,
                        IsBodyHtml = message.IsHtml
                    };
                    mail.To.Add(message.To);

                    await client.SendMailAsync(mail);

                    _logger.LogInformation("✓ Email sent via System.Net.Mail on port {Port}", port);

                    return new EmailResult
                    {
                        Success = true,
                        Message = $"Email sent via System.Net.Mail on port {port}"
                    };
                }
                catch
                {
                    // Try next port
                }
            }

            throw new Exception("All System.Net.Mail ports failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System.Net.Mail method failed");
            throw;
        }
    }

    private MimeMessage CreateMimeMessage(EmailMessage message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;

        var builder = new BodyBuilder();
        if (message.IsHtml)
            builder.HtmlBody = message.Body;
        else
            builder.TextBody = message.Body;

        email.Body = builder.ToMessageBody();
        return email;
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
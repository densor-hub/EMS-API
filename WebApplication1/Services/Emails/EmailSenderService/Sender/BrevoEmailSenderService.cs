using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService;

public class BrevoEmailSenderService : IEmailSenderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BrevoEmailSenderService> _logger;
    private readonly EmailSettings _settings;

    public BrevoEmailSenderService(
        HttpClient httpClient,
        IOptions<EmailSettings> settings,
        ILogger<BrevoEmailSenderService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        // ── Diagnostic: confirm the key is actually loaded ──
        var key = _settings.ResendApiToken;   // holds the Brevo API key

        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError(
                "Brevo key is NULL/EMPTY. Check env var EmailSettings__ResendApiToken on Render.");
        }
        else
        {
            var prefix = key.StartsWith("xkeysib-") ? "xkeysib-" : "!!UNEXPECTED!!";
            var masked = key.Length <= 12
                ? new string('*', key.Length)
                : $"{key.Substring(0, 8)}...{key.Substring(key.Length - 4)}";

            _logger.LogInformation(
                "Brevo key loaded: Length={Length}, Prefix='{Prefix}', Masked='{Masked}'",
                key.Length, prefix, masked);
        }

        // Set the API key header for all requests
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", key ?? string.Empty);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<EmailResult> SendEmailAsync(EmailMessage message)
    {
        try
        {
            // ── Normalize sender fields ──
            var senderName = _settings.SenderName?.Trim();
            var senderEmail = _settings.SenderEmail?.Trim();

            _logger.LogInformation(
                "Brevo config: SenderName='{Name}' SenderEmail='{Email}'",
                senderName ?? "<null>",
                senderEmail ?? "<null>");

            if (string.IsNullOrWhiteSpace(senderEmail))
            {
                const string err = "SenderEmail is not configured (empty or missing).";
                _logger.LogError(err);
                return new EmailResult { Success = false, Message = err };
            }

            // ── Build the Brevo payload ──
            var payload = new
            {
                sender = new
                {
                    name = string.IsNullOrWhiteSpace(senderName) ? null : senderName,
                    email = senderEmail
                },
                to = new[]
                {
                    new { email = message.To }
                },
                subject = message.Subject,
                htmlContent = message.IsHtml ? message.Body : null,
                textContent = message.IsHtml ? null : message.Body,
                bcc = message.Bcc != null && message.Bcc.Any()
                    ? message.Bcc
                        .Where(b => !string.IsNullOrWhiteSpace(b))
                        .Select(b => new { email = b })
                        .ToArray()
                    : null
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            _logger.LogInformation("Brevo FROM = '{From}'", senderEmail);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(
                "https://api.brevo.com/v3/smtp/email", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Brevo send failed. Status: {Status}, Error: {Error}",
                    response.StatusCode, responseBody);

                return new EmailResult
                {
                    Success = false,
                    Message = $"Brevo {response.StatusCode}: {responseBody}"
                };
            }

            _logger.LogInformation(
                "Brevo: sent email to {To}. Response: {Response}",
                message.To, responseBody);

            return new EmailResult
            {
                Success = true,
                Message = "Sent via Brevo"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Brevo send failed for {To}", message.To);
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
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml,
            Cc = new List<string>(),
            Bcc = Bcc ?? new List<string>(),
            Attachments = new List<EmailAttachment>()
        };

        return await SendEmailAsync(message);
    }
}
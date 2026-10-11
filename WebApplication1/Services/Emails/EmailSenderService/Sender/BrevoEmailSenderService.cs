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
        // Set the API key header for all requests
        _httpClient.DefaultRequestHeaders.Add("api-key", _settings.BrevoApiToken); // Use the same config key or rename
    }

    public async Task<EmailResult> SendEmailAsync(EmailMessage message)
    {
        try
        {
            _logger.log($"BREVO API KEY {settings.BrevoApiToken}");
            var payload = new
            {
                sender = new { name = _settings.SenderName, email = _settings.SenderEmail },
                to = new[] { new { email = message.To } },
                subject = message.Subject,
                htmlContent = message.IsHtml ? message.Body : null,
                textContent = message.IsHtml ? null : message.Body
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Brevo send failed. Status: {Status}, Error: {Error}", response.StatusCode, error);
                return new EmailResult { Success = false, Message = error };
            }

            _logger.LogInformation("Brevo: sent email to {To}", message.To);
            return new EmailResult { Success = true, Message = "Sent via Brevo" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Brevo send failed for {To}", message.To);
            return new EmailResult { Success = false, Message = ex.Message };
        }
    }

    public Task<EmailResult> SendEmailAsync(string to, string subject, string body, List<string> Bcc = null, bool isHtml = true)
    {
        var message = new EmailMessage { To = to, Subject = subject, Body = body, IsHtml = isHtml, Bcc = Bcc ?? new List<string>() };
        return SendEmailAsync(message);
    }
}
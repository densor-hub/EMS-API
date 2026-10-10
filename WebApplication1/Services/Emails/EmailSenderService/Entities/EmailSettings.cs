using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.Emails.EmailService.Entities
{
    public class EmailSettings
    {
        public bool UseResend { get; set; }
        // Shared sender info (used by both SMTP and Resend)
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string AppUrl { get; set; } = string.Empty;

        // SMTP-only (used when Email:UseResend = false)
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;

        // Resend-only (used when Email:UseResend = true)
        public string ResendApiToken { get; set; } = string.Empty;
    }
}

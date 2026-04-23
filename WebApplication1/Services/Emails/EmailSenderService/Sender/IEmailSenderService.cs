using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Services.Emails.EmailService.Entities;

namespace WebApplication1.Services.Emails.EmailService
{
    public interface IEmailSenderService
    {
        Task<EmailResult> SendEmailAsync(EmailMessage message);
        Task<EmailResult> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    }
}

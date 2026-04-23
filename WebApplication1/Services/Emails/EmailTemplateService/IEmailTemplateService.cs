using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Services.Emails.TemplateService.Enitities;

namespace WebApplication1.Services.Emails.TemplateService
{
    public interface IEmailTemplateService
    {
        Task<string> RenderEmailTemplateAsync(AllEmailsTemplateModel model, string templateName);
        Task<string> RenderWelcomeEmailTemplateAsync(WelcomeEmailTemplateModel model);
    }
}

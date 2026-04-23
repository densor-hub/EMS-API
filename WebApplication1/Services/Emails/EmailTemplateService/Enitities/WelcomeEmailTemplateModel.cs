using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.Emails.TemplateService.Enitities
{
    public class WelcomeEmailTemplateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string TemporaryPassword { get; set; }
        public string LoginUrl { get; set; }
    }
}

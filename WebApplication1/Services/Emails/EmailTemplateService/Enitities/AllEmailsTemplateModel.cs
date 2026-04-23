using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.Emails.TemplateService.Enitities
{
    public class AllEmailsTemplateModel
    {
        //companyInfo
        public string CompanyLogo { get;  set; }          // URL of company logo
        public string CompanyName { get;  set; }          // Name of the company
        public string CompanyAddress { get;  set; }       // Physical address of company
        public string CompanyPhone { get;  set; }         // Company phone number
        public string CompanyEmail { get;  set; }
        public string ManagerName { get;  set; }
        public string ManagerTitle { get;  set; }
        public string ManagerEmail { get;  set; }
        public string ManagerPhone { get;  set; }
        public string SupportEmail { get;  set; }
        public string SupportName { get;  set; }
        public string SupportPhone { get;  set; }


        //receiver info
        public string ReceiverName { get;  set; }
        public string ReceiverEmail { get;  set; }
        public string ReceiverType { get;  set; }
        public string ReceiverCode { get;  set; }
        public string ReceiverRole { get;  set; }
        public string ReceiverUserName { get;  set; }


        //app stuff
        public string AppName { get;  set; }
        public string TemporaryPassword { get;  set; }
        public string MinPasswordLength { get;  set; }
        public string LoginUrl { get;  set; }
        public string PinCode { get;  set; }
        public string QrCodeImageBase64 { get;  set; }
        public int ValidityHours { get;  set; } = 24;

        //business partners
        public string PrimaryEmail { get;  set; }       // Category/classification of partner
        public string SecondaryEmail { get;  set; }          // Industry or business sector
        public string PrimaryPhoneNumber { get;  set; }
        public string SecondaryPhoneNumber { get;  set; }
        public string Address { get;  set; }


        public AllEmailsTemplateModel()
        {
            
        }
    }

}

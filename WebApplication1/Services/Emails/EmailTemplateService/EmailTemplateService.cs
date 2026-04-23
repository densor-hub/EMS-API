using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

using WebApplication1.Services.Emails.Exceptions;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.QrCodeService;
using WebApplication1.Services.Emails.EmailService.Entities;
using static QRCoder.PayloadGenerator;

namespace WebApplication1.Services.Emails.TemplateService
{
    internal class EmailTemplateService : IEmailTemplateService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IQrCodeService _qrCodeService;
        private readonly EmailSettings _emailSettings;

        public EmailTemplateService(IWebHostEnvironment environment, IQrCodeService qrCodeService, IOptions<EmailSettings> emailSettings)
        {
            _environment = environment;
            _qrCodeService = qrCodeService;
            _emailSettings = emailSettings.Value;
        }


        public async Task<string> RenderEmailTemplateAsync(AllEmailsTemplateModel model, string templateName)
        {
            var template = await GetTemplateAsync(templateName);

            var qrCode = await GenerateVisitQrCodeAsync(model);


            var placeholders = new Dictionary<string, object>
            {
                ["CompanyLogo"] = model.CompanyLogo,
                ["CompanyName"] = model.CompanyName,
                ["CompanyAddress"] = model.CompanyAddress,
                ["CompanyPhone"] = model.CompanyPhone,
                ["CompanyEmail"] = model.CompanyEmail,
                // ["Date"] = model.VisitDate.ToString("dddd, MMMM dd, yyyy"),
                //["Time"] = model.VisitTime.ToString(@"hh\:mm"),
                ["AppName"] = model.AppName,
                ["ReceiverName"] = model.ReceiverName,
                ["ReceiverRole"] = model.ReceiverRole,
                ["ReceiverUserName"] = model.ReceiverUserName,
                ["ReceiverCode"] = model.ReceiverCode,
                ["TemporaryPassword"] = model.TemporaryPassword,
                ["MinPasswordLength"] = model.MinPasswordLength,
                ["LoginUrl"] = model.LoginUrl,
                ["SupportEmail"] = model.SupportEmail,
                ["SupportPhone"] = model.SupportPhone,
                ["SupportName"] = model.SupportName,
                ["PinCode"] = model.PinCode,
                ["QrCodeImageBase64"] = model.QrCodeImageBase64,
                ["ValidityHours"] = model.ValidityHours,
                ["ReceiverType"] = model.ReceiverType,
                ["PinCode"] = model.PinCode,
               
                ["PrimaryEmail"] = model.PrimaryEmail,
                ["SecondaryEmail"] = model.SecondaryEmail,
                ["PrimaryPhoneNumber"] = model.PrimaryPhoneNumber,
                ["SecondaryPhoneNumber"] = model.SecondaryPhoneNumber,
                ["Address"] = model.Address,
                ["ShopManagerName"] = model.ManagerName,
                ["ShopManagerEmail"] = model.ManagerEmail,
                ["ShopManagerPhone"] = model.ManagerPhone,
                ["ShopManagerTitle"] = model.ManagerTitle
            };

            return await ProcessTemplateAsync(template, placeholders);
        }



        public async Task<string> RenderWelcomeEmailTemplateAsync(WelcomeEmailTemplateModel model)
        {
            var template = await GetTemplateAsync("WelcomeEmail");

            var placeholders = new Dictionary<string, object>
            {
                ["Name"] = model.Name,
                ["Email"] = model.Email,
                ["TemporaryPassword"] = model.TemporaryPassword,
                ["LoginUrl"] = model.LoginUrl
            };

            return await ProcessTemplateAsync(template, placeholders);
        }



        private async Task<string> ProcessTemplateAsync(string template, Dictionary<string, object> model)
        {
            var result = template;

            foreach (var item in model)
            {
                // Replace {{PropertyName}} format
                var curlyPlaceholder = $"{{{{{item.Key}}}}}";
                result = result.Replace(curlyPlaceholder, item.Value?.ToString() ?? string.Empty);

                // Replace [PropertyName] format
                var bracketPlaceholder = $"[{item.Key}]";
                result = result.Replace(bracketPlaceholder, item.Value?.ToString() ?? string.Empty);
            }

            // Handle conditional sections
            result = ProcessConditionalSections(result, model);

            return result;
        }

        private string ProcessConditionalSections(string template, Dictionary<string, object> model)
        {
            // Handle conditional sections like [If:HasSpecialInstructions]...[/If:HasSpecialInstructions]
            var conditionalPattern = @"\[If:(\w+)\](.*?)\[\/If:\1\]";
            var matches = System.Text.RegularExpressions.Regex.Matches(template,
                conditionalPattern, System.Text.RegularExpressions.RegexOptions.Singleline);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var conditionKey = match.Groups[1].Value;
                var content = match.Groups[2].Value;

                if (model.ContainsKey(conditionKey) && IsConditionTrue(model[conditionKey]))
                {
                    template = template.Replace(match.Value, content);
                }
                else
                {
                    template = template.Replace(match.Value, string.Empty);
                }
            }

            return template;
        }

        private bool IsConditionTrue(object conditionValue)
        {
            return conditionValue switch
            {
                bool boolValue => boolValue,
                string stringValue => !string.IsNullOrWhiteSpace(stringValue),
                int intValue => intValue > 0,
                _ => conditionValue != null
            };
        }

        private async Task<string> GetTemplateAsync(string templateName)
        {
            return templateName switch
            {
                "EmpolyeeAppAccess" => GetEmployeeAppAccessTemplate(),
                "BusinessPartnerAdded" => GetBusinessPartnerWelcomeTemplate(),
                //"VisitReschedule" => GetVisitRescheduleTemplate(),
                //"VisitCancellation" => GetVisitCancellationTemplate(),
                //"VisitDeclined" => GetVisitDeclinedTemplate(),
                _ => throw new ArgumentException($"Template '{templateName}' is not supported.")
            };
        }

        private async Task<byte[]> GenerateVisitQrCodeAsync(AllEmailsTemplateModel model)
        {
            //var testBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==";
            var qrCodeData = $"PIN:{model.PinCode}";
            var qrCodeBytes = _qrCodeService.GenerateQrCode(model?.PinCode) as byte[];

            if (string.IsNullOrEmpty(Convert.ToBase64String(qrCodeBytes))) { throw new FailedToGenerateQrCodeException("Failed to generate QR code"); }
            return qrCodeBytes;
        }

        private string GetEmployeeAppAccessTemplate()
        {
            return @"<!DOCTYPE html>
    <html lang=""en"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
    <head>
        <meta charset=""utf-8"">
        <meta name=""x-apple-disable-message-reformatting"">
        <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
        <meta name=""format-detection"" content=""telephone=no, date=no, address=no, email=no"">
        <!--[if mso]>
        <xml>
            <o:OfficeDocumentSettings>
                <o:PixelsPerInch>96</o:PixelsPerInch>
            </o:OfficeDocumentSettings>
        </xml>
        <style>
            td, th, div, p, a, h1, h2, h3, h4, h5, h6 {
                font-family: ""Segoe UI"", sans-serif;
                mso-line-height-rule: exactly;
            }
        </style>
        <![endif]-->

        <style>
            .hover-underline:hover {
                text-decoration: underline !important;
            }

            .credentials-container {
                background-color: #f8f9fa;
                padding: 25px;
                border-radius: 8px;
                margin: 20px 0;
                border: 1px solid #e9ecef;
            }

            .credential-row {
                margin-bottom: 20px;
                padding-bottom: 20px;
                border-bottom: 1px dashed #dee2e6;
            }

            .credential-row:last-child {
                border-bottom: none;
                margin-bottom: 0;
                padding-bottom: 0;
            }

            .credential-label {
                font-weight: 600;
                color: #495057;
                margin-bottom: 5px;
                font-size: 14px;
            }

            .credential-value {
                font-size: 22px;
                font-weight: 500;
                color: #1a3e6f;
                letter-spacing: 0.5px;
                font-family: 'SF Mono', 'Menlo', 'Monaco', 'Cascadia Code', 'Consolas', 'Courier New', monospace;
                padding: 10px 16px;
                background-color: #ffffff;
                border-radius: 8px;
                border: 1px solid #d1d9e6;
                display: inline-block;
                box-shadow: 0 2px 4px rgba(0,0,0,0.02);
                line-height: 1.4;
            }

            .button-container {
                text-align: center;
                margin: 25px 0;
            }
            
            .button {
                display: inline-block;
                padding: 8px 20px;
                background-color: #2c5aa0;
                color: #ffffff !important;
                text-decoration: none;
                border-radius: 50px;
                font-weight: 500;
                font-size: 13px;
                letter-spacing: 0.3px;
                border: 1px solid #1e3f7a;
                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                transition: all 0.2s ease;
                width: auto;
                min-width: 140px;
                max-width: 200px;
            }

            .button:hover {
                background-color: #1e3f7a;
                box-shadow: 0 4px 8px rgba(0,0,0,0.15);
                transform: translateY(-1px);
            }
            
            .button:active {
                transform: translateY(0);
                box-shadow: 0 1px 2px rgba(0,0,0,0.1);
            }
            
            .url-text {
                font-size: 11px;
                color: #666;
                margin: 6px 0 0 0;
                word-break: break-all;
                font-family: 'SF Mono', 'Menlo', 'Monaco', 'Consolas', monospace;
            }
            
            .security-notice {
                background-color: #fff3cd;
                border-left: 4px solid #ffc107;
                padding: 20px;
                margin: 25px 0;
                border-radius: 4px;
            }

            .security-notice.important {
                background-color: #f8d7da;
                border-left-color: #dc3545;
            }

            .security-notice.warning {
                background-color: #fff3cd;
                border-left-color: #ffc107;
            }

            .app-details {
                background-color: #e7f3ff;
                padding: 20px;
                border-radius: 8px;
                margin: 20px 0;
            }

            .step-by-step {
                margin: 20px 0;
                padding: 0;
                list-style: none;
            }

            .step-by-step li {
                margin-bottom: 12px;
                padding-left: 28px;
                position: relative;
                font-size: 14px;
            }

            .step-by-step li:before {
                content: ""✓"";
                color: #28a745;
                font-weight: bold;
                position: absolute;
                left: 0;
                font-size: 16px;
            }

            .password-requirements {
                background-color: #f8f9fa;
                padding: 15px;
                border-radius: 6px;
                margin: 20px 0;
            }
            
            .password-requirements h4 {
                color: #495057;
                margin: 0 0 10px 0;
                font-size: 15px;
            }
            
            .password-requirements ul {
                margin: 0;
                padding-left: 20px;
                color: #666;
                font-size: 13px;
            }
            
            .support-info {
                margin: 30px 0 20px 0;
                padding-top: 20px;
                border-top: 2px solid #e9ecef;
                font-size: 14px;
            }
            
            .reminders {
                background-color: #e7f3ff;
                padding: 15px;
                border-radius: 6px;
                margin: 20px 0;
            }
            
            .reminders h4 {
                color: #2c5aa0;
                margin: 0 0 10px 0;
                font-size: 15px;
            }
            
            .reminders ul {
                margin: 0;
                padding-left: 20px;
                color: #2c5aa0;
                font-size: 13px;
            }
            
            .footer-note {
                margin: 30px 0 0 0;
                font-size: 12px;
                color: #999;
                text-align: center;
                border-top: 1px solid #eee;
                padding-top: 20px;
            }
            
            .powered-by {
                margin: 20px 0 0 0;
                text-align: center;
                font-weight: 500;
                color: #666;
                font-size: 12px;
            }

            @media (max-width: 600px) {
                .sm-w-full {
                    width: 100% !important;
                }
                
                .sm-px-24 {
                    padding-left: 24px !important;
                    padding-right: 24px !important;
                }
                
                .credential-value {
                    font-size: 18px;
                    word-break: break-all;
                    padding: 8px 12px;
                }
                
                .button {
                    display: block;
                    width: 100%;
                    max-width: 100%;
                    padding: 10px;
                    font-size: 13px;
                }
                
                h1 {
                    font-size: 24px !important;
                }
            }
        </style>
    </head>

    <body style=""margin: 0; padding: 0; width: 100%; word-break: break-word; -webkit-font-smoothing: antialiased; background-color: #eceff1;"">
        <div role=""article"" aria-roledescription=""email"" aria-label=""Employee App Access"" lang=""en"">
            <table style=""font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif; width: 100%;"" width=""100%""
                   cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                <tr>
                    <td align=""center"" style=""background-color: #eceff1; font-family: 'Segoe UI', sans-serif;"">
                        <table class=""sm-w-full"" style=""font-family: 'Segoe UI', sans-serif; width: 600px;"" width=""600""
                               cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                            <tr>
                                <td class=""sm-py-32 sm-px-24""
                                    style=""font-family: 'Segoe UI', sans-serif; padding: 48px 10px 10px 10px; text-align: center;""
                                    align=""center"">
                                    <img src=""{{CompanyLogo}}"" alt=""{{CompanyName}}"" style=""max-height: 50px; width: auto;"">
                                </td>
                            </tr>
                            <tr>
                                <td align=""center"" class=""sm-px-24"" style=""font-family: 'Segoe UI', sans-serif;"">
                                    <table style=""font-family: 'Segoe UI', sans-serif; width: 100%;"" width=""100%""
                                           cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                                        <tr>
                                            <td class=""sm-px-24""
                                                style=""background-color: #ffffff; border-radius: 8px; font-family: 'Segoe UI', sans-serif; font-size: 15px; line-height: 1.5; padding: 35px; text-align: left; color: #333333; box-shadow: 0 2px 8px rgba(0,0,0,0.1);""
                                                align=""left"">

                                                <h1 style=""font-size: 26px; color: #2c5aa0; text-align: center; margin: 0 0 15px 0; font-weight: 600;"">
                                                    Welcome to {{AppName}}!
                                                </h1>

                                                <p style=""margin: 0 0 15px; font-size: 15px;"">
                                                    Hello <strong>{{ReceiverName}}</strong>,
                                                </p>
                                                
                                                <p style=""margin: 0 0 15px; font-size: 15px;"">
                                                    You have been offered the <strong>{{ReceiverRole}}</strong> role at <strong>{{CompanyName}}</strong>.
                                                    Your account has been created for <strong>{{AppName}}</strong>. Please find your login credentials below. 
                                                    For security reasons, you will be required to change your password upon first login.
                                                </p>

                                                <div class=""app-details"">
                                                    <h3 style=""color: #2c5aa0; margin: 0; font-size: 17px;"">
                                                        Application Access Information
                                                    </h3>
                                                </div>

                                                <div class=""credentials-container"">
                                                    <h3 style=""color: #2c5aa0; margin: 0 0 15px 0; text-align: center; font-size: 18px;"">
                                                        Your Login Credentials
                                                    </h3>

                                                    <div class=""credential-row"">
                                                        <div class=""credential-label"">ReceiverUserName / Email:</div>
                                                        <div class=""credential-value"">{{ReceiverUserName}}</div>
                                                    </div>

                                                    <div class=""credential-row"">
                                                        <div class=""credential-label"">Temporary Password:</div>
                                                        <div class=""credential-value"">{{TemporaryPassword}}</div>
                                                    </div>
                                                </div>

                                                <div class=""button-container"">
                                                    <a href=""{{LoginUrl}}"" class=""button"">
                                                        Access {{AppName}}
                                                    </a>
                                                    <div class=""url-text"">
                                                        {{LoginUrl}}
                                                    </div>
                                                </div>

                                                <div class=""security-notice important"">
                                                    <h4 style=""color: #721c24; margin: 0 0 8px 0; font-size: 16px;"">
                                                        ⚠️ Password Change Required
                                                    </h4>
                                                    <p style=""margin: 0; color: #721c24; font-size: 14px;"">
                                                        You must change your password immediately after logging in. 
                                                        This is a mandatory requirement </strong>.
                                                    </p>
                                                </div>

                                                <div class=""security-notice warning"">
                                                    <h4 style=""color: #856404; margin: 0 0 12px 0; font-size: 16px;"">
                                                        📋 First-Time Login Instructions
                                                    </h4>
                                                    <ol class=""step-by-step"">
                                                        <li>Click the ""Access {{AppName}}"" button above</li>
                                                        <li>Enter your username/email and the temporary password</li>
                                                        <li>Create a new password meeting security requirements</li>
                                                        <li>Confirm your new password and complete setup</li>
                                                        <li>Set up security questions if prompted</li>
                                                    </ol>
                                                </div>

                                                <div class=""password-requirements"">
                                                    <h4>🔒 Password Requirements:</h4>
                                                    <ul>
                                                        <li>Minimum {{MinPasswordLength}} characters</li>
                                                        <li>At least one uppercase letter (A-Z)</li>
                                                        <li>At least one lowercase letter (a-z)</li>
                                                        <li>At least one number (0-9)</li>
                                                        <li>At least one special character (!@#$%^&*)</li>
                                                    </ul>
                                                </div>

                                                <div class=""support-info"">
                                                    <h4 style=""color: #2c5aa0; margin: 0 0 12px 0; font-size: 15px;"">
                                                        Need Help?
                                                    </h4>
                                                    <p style=""margin: 0 0 3px; font-size: 13px;"">
                                                        <strong>IT Support:</strong> {{SupportName}}
                                                    </p>
                                                    <p style=""margin: 0 0 3px; font-size: 13px;"">
                                                        <strong>Email:</strong> <a href=""mailto:{{SupportEmail}}"" style=""color: #2c5aa0; text-decoration: none;"">{{SupportEmail}}</a>
                                                    </p>
                                                    <p style=""margin: 0 0 3px; font-size: 13px;"">
                                                        <strong>Phone:</strong> {{SupportPhone}}
                                                    </p>
                                                </div>

                                                <div class=""reminders"">
                                                    <h4>📌 Important Reminders:</h4>
                                                    <ul>
                                                        <li>Never share your password with anyone</li>
                                                        <li>We will never ask for your password via email</li>
                                                        <li>Log out when using shared computers</li>
                                                        <li>Report suspicious activity immediately</li>
                                                        <!-- The access expiry list item has been removed as requested -->
                                                    </ul>
                                                </div>

                                                <div class=""footer-note"">
                                                    This is an automated message from {{CompanyName}}. Please do not reply to this email.
                                                </div>

                                                <div class=""powered-by"">
                                                    Powered by Abibeck Software Solutions
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </body>
    </html>";
        }
        private string GetBusinessPartnerWelcomeTemplate()
        {
            return """
    <!DOCTYPE html>
    <html lang="en" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office">
    <head>
        <meta charset="utf-8">
        <meta name="x-apple-disable-message-reformatting">
        <meta http-equiv="x-ua-compatible" content="ie=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <meta name="format-detection" content="telephone=no, date=no, address=no, email=no">
        <!--[if mso]>
        <xml>
            <o:OfficeDocumentSettings>
                <o:PixelsPerInch>96</o:PixelsPerInch>
            </o:OfficeDocumentSettings>
        </xml>
        <style>
            td, th, div, p, a, h1, h2, h3, h4, h5, h6 {
                font-family: "Segoe UI", sans-serif;
                mso-line-height-rule: exactly;
            }
        </style>
        <![endif]-->

        <style>
            .hover-underline:hover {
                text-decoration: underline !important;
            }

            .welcome-banner {
                background: linear-gradient(135deg, #2c5aa0 0%, #1e3f7a 100%);
                color: #ffffff;
                padding: 40px 30px;
                border-radius: 8px 8px 0 0;
                text-align: center;
                margin: -40px -40px 30px -40px;
            }

            .partner-type-badge {
                display: inline-block;
                padding: 8px 20px;
                background-color: rgba(255,255,255,0.2);
                border-radius: 50px;
                font-size: 14px;
                font-weight: 600;
                letter-spacing: 0.5px;
                margin-top: 15px;
                color: #ffffff;
            }

            .partner-details {
                background-color: #f8f9fa;
                padding: 25px;
                border-radius: 8px;
                margin: 25px 0;
                border: 1px solid #e9ecef;
            }

            .detail-grid {
                display: grid;
                grid-template-columns: repeat(2, 1fr);
                gap: 20px;
                margin-top: 15px;
            }

            .detail-item {
                padding: 15px;
                background-color: #ffffff;
                border-radius: 6px;
                border-left: 4px solid #2c5aa0;
            }

            .detail-item.full-width {
                grid-column: span 2;
            }

            .detail-label {
                font-size: 12px;
                color: #666;
                margin-bottom: 5px;
                text-transform: uppercase;
                letter-spacing: 0.5px;
            }

            .detail-value {
                font-size: 16px;
                font-weight: 600;
                color: #333;
            }

            .message-box {
                background-color: #e8f4f8;
                padding: 25px;
                border-radius: 8px;
                margin: 25px 0;
                border-left: 4px solid #2c5aa0;
                font-style: italic;
            }

            .partnership-values {
                display: flex;
                justify-content: space-around;
                margin: 30px 0;
                text-align: center;
                flex-wrap: wrap;
            }

            .value-item {
                flex: 1;
                min-width: 150px;
                padding: 15px;
            }

            .value-icon {
                font-size: 32px;
                margin-bottom: 10px;
                color: #2c5aa0;
            }

            .value-title {
                font-weight: 600;
                color: #333;
                margin-bottom: 5px;
            }

            .value-description {
                font-size: 13px;
                color: #666;
            }

            .contact-info {
                background-color: #ffffff;
                border: 2px solid #e9ecef;
                padding: 20px;
                border-radius: 8px;
                margin: 25px 0;
            }

            .relationship-manager {
                background-color: #f0f7ff;
                padding: 20px;
                border-radius: 8px;
                margin: 25px 0;
                display: flex;
                align-items: center;
                gap: 20px;
            }

            .manager-avatar {
                width: 60px;
                height: 60px;
                background-color: #2c5aa0;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                color: white;
                font-size: 24px;
                font-weight: bold;
            }

            .manager-details {
                flex: 1;
            }

            .manager-name {
                font-size: 18px;
                font-weight: 600;
                color: #333;
                margin-bottom: 5px;
            }

            .manager-title {
                font-size: 14px;
                color: #666;
                margin-bottom: 5px;
            }

            .manager-contact {
                font-size: 14px;
                color: #2c5aa0;
            }

            .next-steps {
                background-color: #fff3cd;
                padding: 20px;
                border-radius: 8px;
                margin: 25px 0;
            }

            .step-item {
                display: flex;
                align-items: center;
                margin-bottom: 15px;
                padding: 10px;
                background-color: rgba(255,255,255,0.5);
                border-radius: 6px;
            }

            .step-number {
                width: 30px;
                height: 30px;
                background-color: #2c5aa0;
                color: white;
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-weight: bold;
                margin-right: 15px;
                flex-shrink: 0;
            }

            .portal-access {
                text-align: center;
                margin: 30px 0;
            }

            .portal-button {
                display: inline-block;
                padding: 15px 20px;
                background-color: #2c5aa0;
                color: #ffffff;
                text-decoration: none;
                border-radius: 50px;
                font-weight: 600;
                font-size: 16px;
                margin: 10px 0;
            }

            .portal-button:hover {
                background-color: #1e3f7a;
            }

            @media (max-width: 600px) {
                .sm-w-full {
                    width: 100% !important;
                }
                
                .sm-px-24 {
                    padding-left: 24px !important;
                    padding-right: 24px !important;
                }
                
                .detail-grid {
                    grid-template-columns: 1fr;
                }
                
                .detail-item.full-width {
                    grid-column: auto;
                }
                
                .partnership-values {
                    flex-direction: column;
                }
                
                .welcome-banner {
                    padding: 30px 20px;
                }
                
                .welcome-banner h1 {
                    font-size: 24px;
                }
                
                .relationship-manager {
                    flex-direction: column;
                    text-align: center;
                }
                
                .step-item {
                    flex-direction: column;
                    text-align: center;
                }
                
                .step-number {
                    margin-right: 0;
                    margin-bottom: 10px;
                }
            }
        </style>
    </head>

    <body style="margin: 0; padding: 0; width: 100%; word-break: break-word; -webkit-font-smoothing: antialiased; background-color: #eceff1;">
        <div role="article" aria-roledescription="email" aria-label="Business Partner Welcome" lang="en">
            <table style="font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif; width: 100%;" width="100%"
                   cellpadding="0" cellspacing="0" role="presentation">
                <tr>
                    <td align="center" style="background-color: #eceff1; font-family: 'Segoe UI', sans-serif;">
                        <table class="sm-w-full" style="font-family: 'Segoe UI', sans-serif; width: 600px;" width="600"
                               cellpadding="0" cellspacing="0" role="presentation">
                            <tr>
                                <td class="sm-py-32 sm-px-24"
                                    style="font-family: 'Segoe UI', sans-serif; padding: 48px 10px 10px 10px; text-align: center;"
                                    align="center">
                                    <img src="{{CompanyLogo}}" alt="{{CompanyName}}" style="max-height: 60px; width: auto;">
                                </td>
                            </tr>
                            <tr>
                                <td align="center" class="sm-px-24" style="font-family: 'Segoe UI', sans-serif;">
                                    <table style="font-family: 'Segoe UI', sans-serif; width: 100%;" width="100%"
                                           cellpadding="0" cellspacing="0" role="presentation">
                                        <tr>
                                            <td class="sm-px-24"
                                                style="background-color: #ffffff; border-radius: 8px; font-family: 'Segoe UI', sans-serif; font-size: 16px; line-height: 1.6; padding: 40px; text-align: left; color: #333333; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"
                                                align="left">

                                                <!-- Welcome Banner with Partner Type -->
                                                <div class="welcome-banner">
                                                    <h1 style="font-size: 32px; margin: 0 0 10px 0; color: #ffffff;">
                                                        Welcome to {{CompanyName}}!
                                                    </h1>
                                                    <p style="font-size: 18px; margin: 0 0 15px 0; opacity: 0.9; color: #ffffff;">
                                                        We're delighted to have you onboard
                                                    </p>
                                                    <div class="partner-type-badge">
                                                        {{ReceiverType}}
                                                    </div>
                                                </div>

                                                <!-- Personal Greeting -->
                                                <p style="margin: 0 0 20px; font-size: 16px;">
                                                    Dear <strong>{{ReceiverName}}</strong>,
                                                </p>
                                                
                                                <p style="margin: 0 0 20px; font-size: 16px;">
                                                    We are pleased to confirm that you have been successfully registered in our system as a valued 
                                                    <strong>{{ReceiverType}}</strong>. This marks the beginning of what we hope will be a long and 
                                                    prosperous business relationship.
                                                </p>


                                                <!-- Contact Information -->
                                                <div class="contact-info">
                                                    <h3 style="color: #2c5aa0; margin: 0 0 15px 0; font-size: 18px;">
                                                        📞 Registered Contact Details
                                                    </h3>
                                                     <div style="margin: 0 0 10px 0;">
                                                        <strong>Primary Name:</strong> {{ReceiverName}}
                                                    </div>
                                                    <div style="margin: 0 0 10px 0;">
                                                        <strong>Primary Email:</strong> {{PrimaryEmail}}
                                                    </div>
                                                    <div style="margin: 0 0 10px 0;">
                                                        <strong>Secondary Email:</strong> {{SecondaryEmail}}
                                                    </div>
                                                    <div style="margin: 0 0 10px 0;">
                                                        <strong>Phone:</strong> {{PrimaryPhoneNumber}}
                                                    </div>
                                                    <div style="margin: 0 0 10px 0;">
                                                        <strong>Alternative Phone:</strong> {{SecondaryPhoneNumber}}
                                                    </div>
                                                    <div style="margin: 0 0 10px 0;">
                                                        <strong>Address:</strong> {{Address}}
                                                    </div>
                                                    <div style="margin: 0 0 10px 0;">
                                                        <strong>Postal Address:</strong> {{PostalAddress}}
                                                    </div>
                                                 
                                                </div>

                                                <!-- Partnership Values -->
                                                <div class="partnership-values">
                                                    <div class="value-item">
                                                        <div class="value-icon">🤝</div>
                                                        <div class="value-title">Trust</div>
                                                        <div class="value-description">Building lasting partnerships based on mutual trust</div>
                                                    </div>
                                                    <div class="value-item">
                                                        <div class="value-icon">📈</div>
                                                        <div class="value-title">Growth</div>
                                                        <div class="value-description">Partners in mutual business success</div>
                                                    </div>
                                                    <div class="value-item">
                                                        <div class="value-icon">⭐</div>
                                                        <div class="value-title">Excellence</div>
                                                        <div class="value-description">Committed to service excellence</div>
                                                    </div>
                                                    <div class="value-item">
                                                        <div class="value-icon">🔄</div>
                                                        <div class="value-title">Collaboration</div>
                                                        <div class="value-description">Working together for shared success</div>
                                                    </div>
                                                </div>

                                                <!-- Personalized Message -->
                                                <div class="message-box">
                                                    <p style="margin: 0; font-size: 16px; color: #2c5aa0;">
                                                        "We look forward to building a prosperous and rewarding partnership with you. 
                                                        Your success is our success, and we're committed to providing you with the best 
                                                        support and services to help our partnership thrive."
                                                    </p>
                                                </div>

                                                <!-- Closing -->
                                                <p style="margin: 30px 0 10px 0;">
                                                    We are excited about the possibilities our partnership holds and look forward to a 
                                                    mutually beneficial relationship.
                                                </p>
                                                
                                                <p style="margin: 0 0 5px 0;">
                                                    Warm regards,
                                                </p>
                                                <p style="margin: 0 0 20px 0;">
                                                    {{CompanyName}}
                                                </p>

                                                <!-- Footer -->
                                                <div style="margin-top: 30px; padding-top: 20px; border-top: 2px solid #e9ecef; text-align: center; font-size: 12px; color: #999;">
                                                    <p style="margin: 0 0 10px 0;">
                                                        {{CompanyName}} | {{CompanyAddress}} | {{CompanyPhone}} | {{CompanyEmail}}
                                                    </p>
                                                    <p style="margin: 0;">
                                                        This email was sent to {{PrimaryEmail}}. Please contact us if you have any question.
                                                    </p>
                                                   
                                                    <p style="margin: 20px 0 0 0; font-weight: 600; color: #666;">
                                                        Powered by Abibeck Software Solutions
                                                    </p>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </body>
    </html>
    """;
        }
    }
}

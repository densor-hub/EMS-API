using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using HandlebarsDotNet;
using WebApplication1.Services.Emails.Exceptions;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.QrCodeService;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Helpers;

namespace WebApplication1.Services.Emails.TemplateService
{
    internal class EmailTemplateService : IEmailTemplateService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IQrCodeService _qrCodeService;
        private readonly EmailSettings _emailSettings;
        private readonly IHandlebars _handlebars;

        public EmailTemplateService(IWebHostEnvironment environment, IQrCodeService qrCodeService, IOptions<EmailSettings> emailSettings)
        {
            _environment = environment;
            _qrCodeService = qrCodeService;
            _emailSettings = emailSettings.Value;

            // Initialize Handlebars
            _handlebars = Handlebars.Create();

            // Register custom helpers
            RegisterHelpers();
        }

        private void RegisterHelpers()
        {
            // Register a helper for formatting currency
            _handlebars.RegisterHelper("formatCurrency", (writer, context, parameters) =>
            {
                if (parameters.Length > 0)
                {
                    var value = parameters[0] as decimal?;
                    if (value.HasValue)
                    {
                        writer.WriteSafeString(value.Value.ToString("N2"));
                        return;
                    }
                }
                writer.WriteSafeString("0.00");
            });

            // Register a helper for formatting dates using the extension method
            _handlebars.RegisterHelper("formatDate", (writer, context, parameters) =>
            {
                if (parameters.Length > 0)
                {
                    var date = parameters[0] as DateTime?;
                    var format = parameters.Length > 1 ? parameters[1]?.ToString() ?? "Standard" : "Standard";

                    if (date.HasValue)
                    {
                        writer.WriteSafeString(date.Value.ToAlphanumericDate(format));
                        return;
                    }
                }
                writer.WriteSafeString(DateTime.Now.ToAlphanumericDate("Standard"));
            });

            // Register a helper for formatting dates with full format
            _handlebars.RegisterHelper("formatDateFull", (writer, context, parameters) =>
            {
                if (parameters.Length > 0)
                {
                    var date = parameters[0] as DateTime?;
                    if (date.HasValue)
                    {
                        writer.WriteSafeString(date.Value.ToAlphanumericDate("Full"));
                        return;
                    }
                }
                writer.WriteSafeString(DateTime.Now.ToAlphanumericDate("Full"));
            });

            // Register a helper for formatting dates with compact format
            _handlebars.RegisterHelper("formatDateCompact", (writer, context, parameters) =>
            {
                if (parameters.Length > 0)
                {
                    var date = parameters[0] as DateTime?;
                    if (date.HasValue)
                    {
                        writer.WriteSafeString(date.Value.ToAlphanumericDate("Compact"));
                        return;
                    }
                }
                writer.WriteSafeString(DateTime.Now.ToAlphanumericDate("Compact"));
            });
        }

        public async Task<string> RenderEmailTemplateAsync(AllEmailsTemplateModel model, string templateName)
        {
            var template = await GetTemplateAsync(templateName);

            // var qrCode = await GenerateVisitQrCodeAsync(model);

            // Build the data object for Handlebars with formatted dates
            var data = new
            {
                CompanyLogo = model.CompanyLogo,
                CompanyName = model.CompanyName,
                CompanyAddress = model.CompanyAddress,
                CompanyPhone = model.CompanyPhone,
                CompanyEmail = model.CompanyEmail,
                AppName = model.AppName,
                ReceiverName = model.ReceiverName,
                ReceiverRole = model.ReceiverRole,
                ReceiverUserName = model.ReceiverUserName,
                ReceiverCode = model.ReceiverCode,
                TemporaryPassword = model.TemporaryPassword,
                MinPasswordLength = model.MinPasswordLength,
                AppUrl = model.AppUrl,
                SupportEmail = model.SupportEmail,
                SupportPhone = model.SupportPhone,
                SupportName = model.SupportName,
                PinCode = model.PinCode,
                QrCodeImageBase64 = model.QrCodeImageBase64,
                ValidityHours = model.ValidityHours,
                ReceiverType = model.ReceiverType,
                PrimaryEmail = model.PrimaryEmail,
                SecondaryEmail = model.SecondaryEmail,
                PrimaryPhoneNumber = model.PrimaryPhoneNumber,
                SecondaryPhoneNumber = model.SecondaryPhoneNumber,
                Address = model.Address,
                //PostalAddress = model.PostalAddress,
                ShopManagerName = model.ManagerName,
                ShopManagerEmail = model.ManagerEmail,
                ShopManagerPhone = model.ManagerPhone,
                ShopManagerTitle = model.ManagerTitle,
                Token = model.Token,
                Amount = model.Amount?.ToString("N2"),
                Cost = model.Cost?.ToString("N2"),
                Balance = model.Balance?.ToString("N2"),
                // Format the date using the extension method
                Date = model.Date?.ToAlphanumericDate("Standard"),
                DateFull = model.Date?.ToAlphanumericDate("Full"),
                DateCompact = model.Date?.ToAlphanumericDate("Compact"),
                DateMonthYear = model.Date?.ToAlphanumericDate("MonthYear"),
                DateAlphanumeric = model.Date?.ToAlphanumericDate("Alphanumeric"),
                Reference = model.Reference,
                //Reference = model.Reference,
                Currency = model.Currency,
                Items = model.Items ?? new List<EmailItem>()
            };

            // Use Handlebars to render the template
            return await ProcessTemplateWithHandlebarsAsync(template, data);
        }

        private async Task<string> ProcessTemplateWithHandlebarsAsync(string template, object data)
        {
            try
            {
                // Compile the template
                var compiledTemplate = _handlebars.Compile(template);

                // Render with data
                return compiledTemplate(data);
            }
            catch (Exception ex)
            {
                // If Handlebars fails, fall back to simple replacement
                var placeholders = new Dictionary<string, object>();

                foreach (var prop in data.GetType().GetProperties())
                {
                    var value = prop.GetValue(data);

                    if (value is DateTime dt)
                    {
                        placeholders[prop.Name] = dt.ToAlphanumericDate("Standard");
                    }
                    else if (value is decimal dec)
                    {
                        placeholders[prop.Name] = dec.ToString("N2");
                    }
                    else if (value is List<EmailItem> items)
                    {
                        var itemsHtml = BuildItemsHtml(items);
                        placeholders[prop.Name] = itemsHtml;
                    }
                    else
                    {
                        placeholders[prop.Name] = value?.ToString() ?? string.Empty;
                    }
                }

                return await ProcessTemplateAsync(template, placeholders);
            }
        }

        private string BuildItemsHtml(List<EmailItem> items)
        {
            if (items == null || !items.Any())
                return string.Empty;

            var html = new System.Text.StringBuilder();
            foreach (var item in items)
            {
                html.AppendLine($@"
                <tr>
                    <td>{System.Net.WebUtility.HtmlEncode(item.Name)}</td>
                    <td>{item.Price}</td>
                    <td style=""text-align: center;"">{item.Quantity}</td>
                    <td>{item.Amount}</td>
                </tr>");
            }
            return html.ToString();
        }

        public async Task<string> RenderWelcomeEmailTemplateAsync(WelcomeEmailTemplateModel model)
        {
            var template = await GetTemplateAsync("WelcomeEmail");

            var placeholders = new Dictionary<string, object>
            {
                ["Name"] = model.Name,
                ["Email"] = model.Email,
                ["TemporaryPassword"] = model.TemporaryPassword,
                ["AppUrl"] = model.AppUrl
            };

            return await ProcessTemplateAsync(template, placeholders);
        }

        private async Task<string> ProcessTemplateAsync(string template, Dictionary<string, object> model)
        {
            var result = template;

            foreach (var item in model)
            {
                var curlyPlaceholder = $"{{{{{item.Key}}}}}";
                result = result.Replace(curlyPlaceholder, item.Value?.ToString() ?? string.Empty);

                var bracketPlaceholder = $"[{item.Key}]";
                result = result.Replace(bracketPlaceholder, item.Value?.ToString() ?? string.Empty);
            }

            result = ProcessConditionalSections(result, model);

            return result;
        }

        private string ProcessConditionalSections(string template, Dictionary<string, object> model)
        {
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
                "CustomerPayment" => GetPaymentConfirmationTemplate(),
                "ItemsDelivery" => GetDeliveryEmailTemplate(),
                "EmployeeSetPassword" => GetEmployeeSetPasswordTemplate(),
                "DepositConfirmation" => GetDepositConfirmationTemplate(),
                _ => throw new ArgumentException($"Template '{templateName}' is not supported.")
            };
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
            padding: 12px 32px;
            background-color: #2c5aa0;
            color: #ffffff !important;
            text-decoration: none;
            border-radius: 50px;
            font-weight: 600;
            font-size: 16px;
            letter-spacing: 0.3px;
            border: 1px solid #1e3f7a;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            transition: all 0.2s ease;
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
            font-size: 12px;
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

        .security-notice.success {
            background-color: #d4edda;
            border-left-color: #28a745;
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
                padding: 12px;
                font-size: 15px;
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
                                                Your account has been created for <strong>{{AppName}}</strong>. 
                                                Please click the button below to set up your password and activate your account.
                                            </p>

                                            <div class=""app-details"">
                                                <h3 style=""color: #2c5aa0; margin: 0; font-size: 17px;"">
                                                    Application Access Information
                                                </h3>
                                            </div>

                                            <div class=""credentials-container"">
                                                <h3 style=""color: #2c5aa0; margin: 0 0 15px 0; text-align: center; font-size: 18px;"">
                                                    Your Account Details
                                                </h3>

                                                <div class=""credential-row"">
                                                    <div class=""credential-label"">Email / Username:</div>
                                                    <div class=""credential-value"">{{ReceiverUserName}}</div>
                                                </div>

                                                <div class=""credential-row"">
                                                    <div class=""credential-label"">Employee Code:</div>
                                                    <div class=""credential-value"">{{PinCode}}</div>
                                                </div>

                                                <div class=""credential-row"" style=""border-bottom: none; margin-bottom: 0; padding-bottom: 0;"">
                                                    <div class=""credential-label"">Role:</div>
                                                    <div class=""credential-value"" style=""font-family: 'Segoe UI', sans-serif; font-size: 18px; letter-spacing: normal;"">
                                                        {{ReceiverRole}}
                                                    </div>
                                                </div>
                                            </div>

                                            <div class=""security-notice success"">
                                                <h4 style=""color: #155724; margin: 0 0 8px 0; font-size: 16px;"">
                                                    🔐 Set Up Your Password
                                                </h4>
                                                <p style=""margin: 0; color: #155724; font-size: 14px;"">
                                                    Click the button below to set up your password. This link will expire in 24 hours for security reasons.
                                                </p>
                                            </div>

                                            <div class=""button-container"">
                                                <a href=""{{AppUrl}}"" class=""button"">
                                                    Set Up Your Password
                                                </a>
                                                <div class=""url-text"">
                                                    Or copy and paste this URL into your browser:
                                                    <br>
                                                    {{AppUrl}}
                                                </div>
                                            </div>

                                            <div class=""security-notice warning"">
                                                <h4 style=""color: #856404; margin: 0 0 12px 0; font-size: 16px;"">
                                                    📋 First-Time Setup Instructions
                                                </h4>
                                                <ol class=""step-by-step"">
                                                    <li>Click the ""Set Up Your Password"" button above</li>
                                                    <li>Create a new password meeting the security requirements</li>
                                                    <li>Confirm your new password</li>
                                                    <li>Click ""Submit"" to complete your account setup</li>
                                                    <li>You will be automatically redirected to the login page</li>
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
                                                    <li>This link expires in 24 hours</li>
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

        private string GetPaymentConfirmationTemplate()
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

                        .header-banner {
                            background: linear-gradient(135deg, #2c5aa0 0%, #1e3f7a 100%);
                            color: #ffffff;
                            padding: 25px 30px;
                            border-radius: 8px 8px 0 0;
                            text-align: center;
                            margin: -40px -40px 25px -40px;
                        }

                        .status-badge {
                            display: inline-block;
                            padding: 5px 16px;
                            background-color: rgba(255,255,255,0.2);
                            border-radius: 50px;
                            font-size: 12px;
                            font-weight: 600;
                            letter-spacing: 0.5px;
                            margin-top: 8px;
                            color: #ffffff;
                        }

                        .payment-details {
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

                        .amount-highlight {
                            background: linear-gradient(135deg, #2c5aa0 0%, #1e3f7a 100%);
                            color: white;
                            padding: 20px;
                            border-radius: 8px;
                            text-align: center;
                            margin: 20px 0;
                        }

                        .amount-highlight .label {
                            font-size: 14px;
                            opacity: 0.9;
                        }

                        .amount-highlight .amount {
                            font-size: 36px;
                            font-weight: 700;
                            margin: 5px 0;
                        }

                        .items-table {
                            width: 100%;
                            border-collapse: collapse;
                            margin: 15px 0;
                            font-size: 14px;
                        }

                        .items-table th {
                            background-color: #2c5aa0;
                            color: #ffffff;
                            padding: 10px 12px;
                            text-align: left;
                            font-weight: 600;
                            font-size: 14px;
                        }

                        .items-table td {
                            padding: 10px 12px;
                            border-bottom: 1px solid #e9ecef;
                            color: #333;
                            font-size: 14px;
                            word-wrap: break-word;
                            word-break: break-word;
                        }

                        .items-table tr:nth-child(even) {
                            background-color: #f8f9fa;
                        }

                        .items-table tr:hover {
                            background-color: #e8f4f8;
                        }

                        .items-table td:last-child {
                            text-align: right !important;
                        }

                        .action-button {
                            display: block;
                            width: 100%;
                            max-width: 300px;
                            margin: 0 auto;
                            padding: 15px 40px;
                            background-color: #2c5aa0;
                            color: #ffffff;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: 600;
                            font-size: 18px;
                            text-align: center;
                            transition: background-color 0.3s;
                        }

                        .action-button:hover {
                            background-color: #1e3f7a;
                        }

                        .button-note {
                            text-align: center;
                            font-size: 13px;
                            color: #666;
                            margin-top: 15px;
                        }

                        .verification-box {
                            background-color: #fff8e1;
                            padding: 20px;
                            border-radius: 8px;
                            margin: 25px 0;
                            border-left: 4px solid #ff9800;
                        }

                        .verification-box h4 {
                            margin: 0 0 10px 0;
                            color: #e65100;
                            font-size: 15px;
                        }

                        .verification-box ul {
                            margin: 0;
                            padding-left: 20px;
                        }

                        .verification-box li {
                            margin-bottom: 8px;
                            color: #333;
                            font-size: 14px;
                        }

                        .support-box {
                            background-color: #e8f4f8;
                            padding: 20px;
                            border-radius: 8px;
                            margin: 25px 0;
                            border-left: 4px solid #2c5aa0;
                            text-align: center;
                        }

                        .highlight-text {
                            font-weight: 700;
                            color: #2c5aa0;
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

                            .header-banner {
                                padding: 20px;
                                margin: -20px -20px 20px -20px;
                            }

                            .header-banner h1 {
                                font-size: 20px;
                            }

                            .header-banner p {
                                font-size: 14px;
                            }

                            .amount-highlight .amount {
                                font-size: 28px;
                            }

                            .items-table {
                                font-size: 11px !important;
                            }

                            .items-table th {
                                font-size: 10px !important;
                                padding: 6px 8px !important;
                            }

                            .items-table td {
                                font-size: 10px !important;
                                padding: 6px 8px !important;
                                word-wrap: break-word !important;
                                word-break: break-word !important;
                                max-width: 60px;
                            }

                            .items-table td:first-child {
                                max-width: 80px;
                            }

                            .items-table td:nth-child(2) {
                                max-width: 50px;
                            }

                            .items-table td:nth-child(3) {
                                max-width: 30px;
                                text-align: center !important;
                            }

                            .items-table td:last-child {
                                max-width: 40px;
                                text-align: right !important;
                            }
                        }
                    </style>
                </head>

                <body style="margin: 0; padding: 0; width: 100%; word-break: break-word; -webkit-font-smoothing: antialiased; background-color: #eceff1;">
                    <div role="article" aria-roledescription="email" aria-label="Payment Confirmation" lang="en">
                        <table style="font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif; width: 100%;" width="100%"
                               cellpadding="0" cellspacing="0" role="presentation">
                            <tr>
                                <td align="center" style="background-color: #eceff1; font-family: 'Segoe UI', sans-serif;">
                                    <table class="sm-w-full" style="font-family: 'Segoe UI', sans-serif; width: 600px;" width="600"
                                           cellpadding="0" cellspacing="0" role="presentation">

                                        <tr>
                                            <td align="center" class="" style="font-family: 'Segoe UI', sans-serif;">
                                                <table style="font-family: 'Segoe UI', sans-serif; width: 100%;" width="100%"
                                                       cellpadding="0" cellspacing="0" role="presentation">
                                                    <tr>
                                                        <td class="sm-px-24"
                                                            style="background-color: #ffffff; border-radius: 8px; font-family: 'Segoe UI', sans-serif; font-size: 16px; line-height: 1.6; padding: 40px; text-align: left; color: #333333; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"
                                                            align="left">


                                                            <!-- Greeting -->
                                                            <p style="margin: 0 0 20px; font-size: 16px;">
                                                                Dear <strong>{{ReceiverName}}</strong>,
                                                            </p>

                                                            <p style="margin: 0 0 20px; font-size: 16px;">
                                                                Payment of <strong>{{Currency}} {{Amount}}</strong> has been successfully been made on <strong class="highlight-text">{{Date}}</strong>.
                                                            </p>

                                                            <!-- Payment Amount -->
                                                            <div class="amount-highlight">
                                                                <div class="label">Payment Amount</div>
                                                                <div class="amount">{{Currency}} {{Amount}}</div>
                                                                <div style="font-size: 14px; opacity: 0.9;">
                                                                    Reference: {{Reference}}
                                                                </div>
                                                            </div>

                                                            <div style="margin: 25px 0;">
                                                                <div>Total Cost <strong>{{Cost}}</strong></div>
                                                                <div>Balance/Debt <strong>{{Balance}}</strong></div>
                                                            </div>

                                                            <!-- Items Purchased -->
                                                            <div>
                                                                <h3 style="color: #2c5aa0; margin: 0 0 15px 0; font-size: 17px;">
                                                                    Items Purchased
                                                                </h3>
                                                                <table class="items-table">
                                                                    <thead>
                                                                        <tr>
                                                                            <th>Item Name</th>
                                                                            <th>Price</th>
                                                                            <th style="text-align: center;">Qty</th>
                                                                            <th style="text-align: right;">Amount</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                        {{#each Items}}
                                                                        <tr>
                                                                            <td>{{this.name}}</td>
                                                                            <td>{{this.price}}</td>
                                                                            <td style="text-align: center;">{{this.quantity}}</td>
                                                                            <td style="text-align: right;">{{this.amount}}</td>
                                                                        </tr>
                                                                        {{/each}}
                                                                    </tbody>
                                                                </table>
                                                            </div>

                                                            <!-- Verification Message -->
                                                            <div class="verification-box">
                                                                <h4>⚠️ Please Verify Payment Amount</h4>
                                                                <ul>
                                                                    <li>
                                                                        <strong>Confirm the payment amount</strong>
                                                                    </li>
                                                                    <li>
                                                                        <strong>Report any discrepancies immediately</strong>
                                                                    </li>
                                                                    <li>
                                                                        <strong>Please keep this confirmation record as it can be used as reference</strong>
                                                                    </li>
                                                                </ul>
                                                            </div>

                                                            <!-- Support -->
                                                            <div class="support-box">
                                                                <p style="margin: 0; font-size: 15px; color: #2c5aa0;">
                                                                    <strong>Need assistance?</strong> Contact our support team at 
                                                                    <a href="mailto:{{CompanyEmail}}" style="color: #2c5aa0; text-decoration: underline;">{{CompanyEmail}}</a>
                                                                    or call {{CompanyPhone}}
                                                                </p>
                                                            </div>

                                                            <!-- Closing -->
                                                            <p style="margin: 25px 0 5px 0; font-size: 16px;">
                                                                Thank you for your business.
                                                            </p>

                                                            <p style="margin: 0 0 5px 0; font-size: 16px;">
                                                                Best regards,
                                                            </p>
                                                            <p style="margin: 0 0 20px 0; font-size: 16px; font-weight: 600; color: #2c5aa0;">
                                                                {{CompanyName}}
                                                            </p>

                                                            <!-- Footer -->
                                                            <div style="margin-top: 30px; padding-top: 20px; border-top: 2px solid #e9ecef; text-align: center; font-size: 12px; color: #999;">
                                                                <p style="margin: 0 0 10px 0;">
                                                                    {{CompanyName}} | {{CompanyAddress}} | {{CompanyPhone}} | {{CompanyEmail}}
                                                                </p>
                                                                <p style="margin: 0;">
                                                                    This email was sent to {{PrimaryEmail}}
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
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                </body>
                </html>
                """;
        }

        private string GetDeliveryEmailTemplate()
        {
            return @"
    <!DOCTYPE html>
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

            .header-banner {
                background: linear-gradient(135deg, #2c5aa0 0%, #1e3f7a 100%);
                color: #ffffff;
                padding: 25px 30px;
                border-radius: 8px 8px 0 0;
                text-align: center;
                margin: -40px -40px 25px -40px;
            }

            .status-badge {
                display: inline-block;
                padding: 5px 16px;
                background-color: rgba(255,255,255,0.2);
                border-radius: 50px;
                font-size: 12px;
                font-weight: 600;
                letter-spacing: 0.5px;
                margin-top: 8px;
                color: #ffffff;
            }

            .delivery-details {
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

            .delivery-highlight {
                background: linear-gradient(135deg, #2c5aa0 0%, #1e3f7a 100%);
                color: white;
                padding: 20px;
                border-radius: 8px;
                text-align: center;
                margin: 20px 0;
            }

            .delivery-highlight .label {
                font-size: 14px;
                opacity: 0.9;
            }

            .delivery-highlight .reference {
                font-size: 20px;
                font-weight: 700;
                margin: 5px 0;
                letter-spacing: 1px;
            }

            .items-table {
                width: 100%;
                border-collapse: collapse;
                margin: 15px 0;
                font-size: 14px;
            }

            .items-table th {
                background-color: #2c5aa0;
                color: #ffffff;
                padding: 10px 12px;
                text-align: left;
                font-weight: 600;
                font-size: 14px;
            }

            .items-table td {
                padding: 10px 12px;
                border-bottom: 1px solid #e9ecef;
                color: #333;
                font-size: 14px;
                word-wrap: break-word;
                word-break: break-word;
            }

            .items-table tr:nth-child(even) {
                background-color: #f8f9fa;
            }

            .items-table tr:hover {
                background-color: #e8f4f8;
            }

            .items-table td:last-child {
                text-align: center !important;
            }

            .verification-box {
                background-color: #fff8e1;
                padding: 20px;
                border-radius: 8px;
                margin: 25px 0;
                border-left: 4px solid #ff9800;
            }

            .verification-box h4 {
                margin: 0 0 10px 0;
                color: #e65100;
                font-size: 15px;
            }

            .verification-box ul {
                margin: 0;
                padding-left: 20px;
            }

            .verification-box li {
                margin-bottom: 8px;
                color: #333;
                font-size: 14px;
            }

            .support-box {
                background-color: #e8f4f8;
                padding: 20px;
                border-radius: 8px;
                margin: 25px 0;
                border-left: 4px solid #2c5aa0;
                text-align: center;
            }

            .highlight-text {
                font-weight: 700;
                color: #2c5aa0;
            }

            .success-icon {
                font-size: 48px;
                margin-bottom: 10px;
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

                .header-banner {
                    padding: 20px;
                    margin: -20px -20px 20px -20px;
                }

                .header-banner h1 {
                    font-size: 20px;
                }

                .header-banner p {
                    font-size: 14px;
                }

                .delivery-highlight .reference {
                    font-size: 16px;
                }

                .items-table {
                    font-size: 11px !important;
                }

                .items-table th {
                    font-size: 10px !important;
                    padding: 6px 8px !important;
                }

                .items-table td {
                    font-size: 10px !important;
                    padding: 6px 8px !important;
                    word-wrap: break-word !important;
                    word-break: break-word !important;
                    max-width: 60px;
                }

                .items-table td:first-child {
                    max-width: 80px;
                }

                .items-table td:last-child {
                    max-width: 40px;
                    text-align: center !important;
                }
            }
        </style>
    </head>

    <body style=""margin: 0; padding: 0; width: 100%; word-break: break-word; -webkit-font-smoothing: antialiased; background-color: #eceff1;"">
        <div role=""article"" aria-roledescription=""email"" aria-label=""Delivery Confirmation"" lang=""en"">
            <table style=""font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif; width: 100%;"" width=""100%""
                   cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                <tr>
                    <td align=""center"" style=""background-color: #eceff1; font-family: 'Segoe UI', sans-serif;"">
                        <table class=""sm-w-full"" style=""font-family: 'Segoe UI', sans-serif; width: 600px;"" width=""600""
                               cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                            <tr>
                                <td align=""center"" style=""font-family: 'Segoe UI', sans-serif;"">
                                    <table style=""font-family: 'Segoe UI', sans-serif; width: 100%;"" width=""100%""
                                           cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                                        <tr>
                                            <td class=""sm-px-24""
                                                style=""background-color: #ffffff; border-radius: 8px; font-family: 'Segoe UI', sans-serif; font-size: 16px; line-height: 1.6; padding: 40px; text-align: left; color: #333333; box-shadow: 0 2px 10px rgba(0,0,0,0.1);""
                                                align=""left"">

                                                <!-- Header Banner -->
                                                <div class=""header-banner"">
                                                    <div class=""success-icon"">📦</div>
                                                    <h1 style=""margin: 0 0 5px 0; font-size: 24px; font-weight: 700;"">
                                                        Delivery Confirmation
                                                    </h1>
                                                    <p style=""margin: 0; opacity: 0.9; font-size: 14px;"">
                                                        Items have been successfully delivered
                                                    </p>
                                                    <div class=""status-badge"">✓ DELIVERED</div>
                                                </div>

                                                <!-- Greeting -->
                                                <p style=""margin: 0 0 20px; font-size: 16px;"">
                                                    Dear <strong>{{ReceiverName}}</strong>,
                                                </p>

                                                <p style=""margin: 0 0 20px; font-size: 16px;"">
                                                    This is to confirm that Items have been successfully delivered on <strong class=""highlight-text"">{{Date}}</strong>.
                                                </p>

                                                <!-- Delivery Reference -->
                                                <div class=""delivery-highlight"">
                                                    <div class=""label"">Reference</div>
                                                    <div class=""reference"">{{Reference}}</div>
                                                    <div style=""font-size: 14px; opacity: 0.9; margin-top: 5px;"">
                                                        Date: {{Date}}
                                                    </div>
                                                </div>


                                                <!-- Items Delivered -->
                                                <div>
                                                    <h3 style=""color: #2c5aa0; margin: 0 0 15px 0; font-size: 17px;"">
                                                        Items Delivered
                                                    </h3>
                                                    <table class=""items-table"">
                                                        <thead>
                                                            <tr>
                                                                <th>Item Name</th>
                                                                <th style=""text-align: center;"">Quantity</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            {{#each Items}}
                                                            <tr>
                                                                <td>{{this.name}}</td>
                                                                <td style=""text-align: center;"">{{this.quantity}}</td>
                                                            </tr>
                                                            {{/each}}
                                                        </tbody>
                                                    </table>
                                                </div>

                                                <!-- Verification Message -->
                                                <div class=""verification-box"">
                                                    <h4>📋 Please Verify Your Delivery</h4>
                                                    <ul>
                                                        <li>
                                                            <strong>Check all items</strong> against this delivery note
                                                        </li>
                                                        <li>
                                                            <strong>Report any discrepancies</strong> within 24 hours
                                                        </li>
                                                        <li>
                                                            <strong>Keep this confirmation</strong> as proof of delivery
                                                        </li>
                                                    </ul>
                                                </div>

                                                <!-- Support -->
                                                <div class=""support-box"">
                                                    <p style=""margin: 0; font-size: 15px; color: #2c5aa0;"">
                                                        <strong>Need assistance?</strong> Contact our support team at 
                                                        <a href=""mailto:{{CompanyEmail}}"" style=""color: #2c5aa0; text-decoration: underline;"">{{CompanyEmail}}</a>
                                                        or call {{CompanyPhone}}
                                                    </p>
                                                </div>

                                                <!-- Closing -->
                                                <p style=""margin: 25px 0 5px 0; font-size: 16px;"">
                                                    Thank you for choosing {{CompanyName}}.
                                                </p>

                                                <p style=""margin: 0 0 5px 0; font-size: 16px;"">
                                                    Best regards,
                                                </p>
                                                <p style=""margin: 0 0 20px 0; font-size: 16px; font-weight: 600; color: #2c5aa0;"">
                                                    {{CompanyName}}
                                                </p>

                                                <!-- Footer -->
                                                <div style=""margin-top: 30px; padding-top: 20px; border-top: 2px solid #e9ecef; text-align: center; font-size: 12px; color: #999;"">
                                                    <p style=""margin: 0 0 10px 0;"">
                                                        {{CompanyName}} | {{CompanyAddress}} | {{CompanyPhone}} | {{CompanyEmail}}
                                                    </p>
                                                    <p style=""margin: 0;"">
                                                        This email was sent to {{PrimaryEmail}}
                                                    </p>
                                                    <p style=""margin: 20px 0 0 0; font-weight: 600; color: #666;"">
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
    ";
        }

        private string GetEmployeeSetPasswordTemplate()
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
                .header-banner {
                    background: linear-gradient(135deg, #2c5aa0 0%, #1e3f7a 100%);
                    color: #ffffff;
                    padding: 30px;
                    border-radius: 8px 8px 0 0;
                    text-align: center;
                    margin: -40px -40px 25px -40px;
                }

                .header-banner h1 {
                    margin: 0 0 8px 0;
                    font-size: 24px;
                    font-weight: 700;
                    color: #ffffff;
                }

                .header-banner p {
                    margin: 0;
                    font-size: 14px;
                    opacity: 0.9;
                    color: #ffffff;
                }

                .action-button {
                    display: inline-block;
                    padding: 12px 28px;
                    background-color: #2c5aa0;
                    color: #ffffff !important;
                    text-decoration: none;
                    border-radius: 6px;
                    font-weight: 600;
                    font-size: 15px;
                    text-align: center;
                }

                .link-fallback {
                    margin-top: 20px;
                    padding: 14px 16px;
                    background-color: #f8f9fa;
                    border: 1px solid #e9ecef;
                    border-radius: 6px;
                    font-size: 13px;
                    color: #666;
                    word-break: break-all;
                }

                .link-fallback .label {
                    display: block;
                    font-size: 11px;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                    color: #999;
                    margin-bottom: 6px;
                }

                .link-fallback a {
                    color: #2c5aa0;
                    text-decoration: underline;
                    word-break: break-all;
                }

                .detail-label {
                    font-size: 12px;
                    color: #666;
                    margin-bottom: 3px;
                    text-transform: uppercase;
                    letter-spacing: 0.5px;
                }

                .detail-value {
                    font-size: 15px;
                    font-weight: 600;
                    color: #333;
                }

                @media (max-width: 600px) {
                    .sm-w-full { width: 100% !important; }
                    .sm-px-24 { padding-left: 24px !important; padding-right: 24px !important; }
                    .header-banner { padding: 20px; margin: -20px -20px 20px -20px; }
                    .header-banner h1 { font-size: 20px; }
                    .header-banner p { font-size: 13px; }
                }
            </style>
        </head>

        <body style="margin: 0; padding: 0; width: 100%; word-break: break-word; -webkit-font-smoothing: antialiased; background-color: #eceff1;">
            <div role="article" aria-roledescription="email" aria-label="Welcome - Set Your Password" lang="en">
                <table style="font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif; width: 100%;" width="100%"
                       cellpadding="0" cellspacing="0" role="presentation">
                    <tr>
                        <td align="center" style="background-color: #eceff1; font-family: 'Segoe UI', sans-serif;">
                            <table class="sm-w-full" style="font-family: 'Segoe UI', sans-serif; width: 600px;" width="600"
                                   cellpadding="0" cellspacing="0" role="presentation">
                                <tr>
                                    <td align="center" style="font-family: 'Segoe UI', sans-serif;">
                                        <table style="font-family: 'Segoe UI', sans-serif; width: 100%;" width="100%"
                                               cellpadding="0" cellspacing="0" role="presentation">
                                            <tr>
                                                <td class="sm-px-24"
                                                    style="background-color: #ffffff; border-radius: 8px; font-family: 'Segoe UI', sans-serif; font-size: 16px; line-height: 1.6; padding: 40px; text-align: left; color: #333333; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"
                                                    align="left">

                                                    <!-- Header Banner -->
                                                    <div class="header-banner">
                                                        <h1>Welcome to {{CompanyName}}</h1>
                                                        <p>Your {{AppName}} account is ready</p>
                                                    </div>

                                                    <p style="margin: 0 0 15px; font-size: 16px;">
                                                        Dear <strong>{{ReceiverName}}</strong>,
                                                    </p>

                                                    <p style="margin: 0 0 20px; font-size: 16px;">
                                                        Your employee account has been created successfully. To get started,
                                                        you need to set up your password using the secure link below.
                                                    </p>

                                                    <!-- Account Details -->
                                                    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%"
                                                           style="margin: 25px 0; background-color: #f8f9fa; border: 1px solid #e9ecef; border-radius: 8px;">
                                                        <tr>
                                                            <td style="padding: 18px; font-family: 'Segoe UI', sans-serif;">
                                                                <h3 style="color: #2c5aa0; margin: 0 0 12px 0; font-size: 16px;">
                                                                    Your Account Details
                                                                </h3>
                                                                <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%">
                                                                    <tr>
                                                                        <td width="50%" valign="top" style="padding: 6px 0;">
                                                                            <div class="detail-label">Full Name</div>
                                                                            <div class="detail-value">{{ReceiverName}}</div>
                                                                        </td>
                                                                        <td width="50%" valign="top" style="padding: 6px 0;">
                                                                            <div class="detail-label">Role</div>
                                                                            <div class="detail-value">{{ReceiverRole}}</div>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" valign="top" style="padding: 6px 0;">
                                                                            <div class="detail-label">Username / Email</div>
                                                                            <div class="detail-value" style="word-break: break-all;">{{ReceiverUserName}}</div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <!-- Call to Action -->
                                                    <div style="margin: 30px 0; text-align: center;">
                                                        <a href="{{AppUrl}}" class="action-button" target="_blank">
                                                            Set Your Password
                                                        </a>

                                                        <div class="link-fallback" style="text-align: left;">
                                                            <span class="label">Or use the link below</span>
                                                            <a href="{{AppUrl}}" target="_blank">{{AppUrl}}</a>
                                                        </div>
                                                    </div>

                                                    <!-- Steps -->
                                                    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%"
                                                           style="margin: 25px 0; background-color: #e8f4f8; border-radius: 8px;">
                                                        <tr>
                                                            <td width="4" style="background-color: #2c5aa0; border-top-left-radius: 8px; border-bottom-left-radius: 8px;">&nbsp;</td>
                                                            <td style="padding: 20px; font-family: 'Segoe UI', sans-serif;">
                                                                <h4 style="margin: 0 0 12px 0; color: #2c5aa0; font-size: 15px;">How to get started</h4>
                                                                <ol style="margin: 0; padding-left: 22px; color: #333; font-size: 14px; line-height: 1.5;">
                                                                    <li style="margin-bottom: 8px;">Click the <strong>Set Your Password</strong> button above.</li>
                                                                    <li style="margin-bottom: 8px;">Create a strong password that meets the security requirements.</li>
                                                                    <li style="margin-bottom: 8px;">Log in to <strong>{{AppName}}</strong> using your email and new password.</li>
                                                                    <li>Explore your dashboard and start using the system.</li>
                                                                </ol>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <!-- Security Notice -->
                                                    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%"
                                                           style="margin: 25px 0; background-color: #fff8e1; border-radius: 8px;">
                                                        <tr>
                                                            <td width="4" style="background-color: #ff9800; border-top-left-radius: 8px; border-bottom-left-radius: 8px;">&nbsp;</td>
                                                            <td style="padding: 18px; font-family: 'Segoe UI', sans-serif;">
                                                                <h4 style="margin: 0 0 10px 0; color: #e65100; font-size: 15px;">⚠️ Security Notice</h4>
                                                                <ul style="margin: 0; padding-left: 20px; color: #333; font-size: 13px; line-height: 1.5;">
                                                                    <li style="margin-bottom: 6px;"><strong>Never share</strong> your password with anyone.</li>
                                                                    <li style="margin-bottom: 6px;">Our team will <strong>never</strong> ask for your password via email or phone.</li>
                                                                    <li>If you did not expect this email, please contact your administrator immediately.</li>
                                                                </ul>
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <!-- Support -->
                                                    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%"
                                                           style="margin: 25px 0; background-color: #e8f4f8; border-radius: 8px;">
                                                        <tr>
                                                            <td width="4" style="background-color: #2c5aa0; border-top-left-radius: 8px; border-bottom-left-radius: 8px;">&nbsp;</td>
                                                            <td style="padding: 18px; text-align: center; font-size: 15px; color: #2c5aa0; font-family: 'Segoe UI', sans-serif;">
                                                                <strong>Need help?</strong> Contact us at
                                                                <a href="mailto:{{SupportEmail}}" style="color: #2c5aa0; text-decoration: underline;">{{SupportEmail}}</a>
                                                                or call {{SupportPhone}}
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <!-- Closing -->
                                                    <p style="margin: 25px 0 5px 0; font-size: 16px;">
                                                        We're excited to have you on board.
                                                    </p>
                                                    <p style="margin: 0 0 5px 0; font-size: 16px;">
                                                        Best regards,
                                                    </p>
                                                    <p style="margin: 0 0 20px 0; font-size: 16px; font-weight: 600; color: #2c5aa0;">
                                                        {{SupportName}}
                                                    </p>

                                                    <!-- Footer -->
                                                    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%"
                                                           style="margin-top: 30px; border-top: 2px solid #e9ecef;">
                                                        <tr>
                                                            <td style="padding-top: 20px; text-align: center; font-size: 12px; color: #999; font-family: 'Segoe UI', sans-serif;">
                                                                <p style="margin: 0 0 10px 0;">
                                                                    {{CompanyName}} | {{CompanyPhone}} | {{SupportEmail}}
                                                                </p>
                                                                <p style="margin: 0;">
                                                                    This email was sent to {{ReceiverUserName}}
                                                                </p>
                                                                <p style="margin: 20px 0 0 0; font-weight: 600; color: #666;">
                                                                    Powered by Abibeck Software Solutions
                                                                </p>
                                                            </td>
                                                        </tr>
                                                    </table>

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


        private string GetDepositConfirmationTemplate()
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
                .header-banner {
                    background: linear-gradient(135deg, #1e7e34 0%, #155724 100%);
                    color: #ffffff;
                    padding: 25px 30px;
                    border-radius: 8px 8px 0 0;
                    text-align: center;
                    margin: -40px -40px 25px -40px;
                }

                .status-badge {
                    display: inline-block;
                    padding: 5px 16px;
                    background-color: rgba(255,255,255,0.2);
                    border-radius: 50px;
                    font-size: 12px;
                    font-weight: 600;
                    letter-spacing: 0.5px;
                    margin-top: 8px;
                    color: #ffffff;
                }

                .amount-highlight {
                    background: linear-gradient(135deg, #1e7e34 0%, #155724 100%);
                    color: white;
                    padding: 20px;
                    border-radius: 8px;
                    text-align: center;
                    margin: 20px 0;
                }

                .amount-highlight .label { font-size: 14px; opacity: 0.9; }
                .amount-highlight .amount { font-size: 36px; font-weight: 700; margin: 5px 0; }

                .details-box {
                    background-color: #f8f9fa;
                    padding: 20px;
                    border-radius: 8px;
                    margin: 20px 0;
                    border: 1px solid #e9ecef;
                }

                .details-box h3 {
                    color: #1e7e34;
                    margin: 0 0 15px 0;
                    font-size: 16px;
                }

                .detail-row {
                    display: flex;
                    justify-content: space-between;
                    padding: 10px 0;
                    border-bottom: 1px dashed #dee2e6;
                    font-size: 15px;
                }

                .detail-row:last-child { border-bottom: none; }
                .detail-row .label { color: #666; }
                .detail-row .value { color: #333; font-weight: 600; }

                .verification-box {
                    background-color: #e8f5e9;
                    padding: 20px;
                    border-radius: 8px;
                    margin: 25px 0;
                    border-left: 4px solid #1e7e34;
                }

                .verification-box h4 {
                    margin: 0 0 10px 0;
                    color: #155724;
                    font-size: 15px;
                }

                .verification-box ul { margin: 0; padding-left: 20px; }
                .verification-box li { margin-bottom: 8px; color: #333; font-size: 14px; }

                .support-box {
                    background-color: #e8f4f8;
                    padding: 20px;
                    border-radius: 8px;
                    margin: 25px 0;
                    border-left: 4px solid #2c5aa0;
                    text-align: center;
                }

                .highlight-text { font-weight: 700; color: #1e7e34; }

                .items-table {
                    width: 100%;
                    border-collapse: collapse;
                    margin: 15px 0;
                    font-size: 14px;
                }

                .items-table th {
                    background-color: #1e7e34;
                    color: #ffffff;
                    padding: 10px 12px;
                    text-align: left;
                    font-weight: 600;
                    font-size: 14px;
                }

                .items-table td {
                    padding: 10px 12px;
                    border-bottom: 1px solid #e9ecef;
                    color: #333;
                    font-size: 14px;
                    word-wrap: break-word;
                    word-break: break-word;
                }

                .items-table tr:nth-child(even) { background-color: #f8f9fa; }
                .items-table td:last-child { text-align: right !important; }

                @media (max-width: 600px) {
                    .sm-w-full { width: 100% !important; }
                    .sm-px-24 { padding-left: 24px !important; padding-right: 24px !important; }
                    .header-banner { padding: 20px; margin: -20px -20px 20px -20px; }
                    .header-banner h1 { font-size: 20px; }
                    .amount-highlight .amount { font-size: 28px; }
                    .detail-row { font-size: 13px; }
                    .items-table { font-size: 11px !important; }
                    .items-table th { font-size: 10px !important; padding: 6px 8px !important; }
                    .items-table td { font-size: 10px !important; padding: 6px 8px !important; }
                }
            </style>
        </head>

        <body style="margin: 0; padding: 0; width: 100%; word-break: break-word; -webkit-font-smoothing: antialiased; background-color: #eceff1;">
            <div role="article" aria-roledescription="email" aria-label="Deposit Acknowledgment" lang="en">
                <table style="font-family: 'Segoe UI', -apple-system, BlinkMacSystemFont, sans-serif; width: 100%;" width="100%"
                       cellpadding="0" cellspacing="0" role="presentation">
                    <tr>
                        <td align="center" style="background-color: #eceff1; font-family: 'Segoe UI', sans-serif;">
                            <table class="sm-w-full" style="font-family: 'Segoe UI', sans-serif; width: 600px;" width="600"
                                   cellpadding="0" cellspacing="0" role="presentation">
                                <tr>
                                    <td align="center" style="font-family: 'Segoe UI', sans-serif;">
                                        <table style="font-family: 'Segoe UI', sans-serif; width: 100%;" width="100%"
                                               cellpadding="0" cellspacing="0" role="presentation">
                                            <tr>
                                                <td class="sm-px-24"
                                                    style="background-color: #ffffff; border-radius: 8px; font-family: 'Segoe UI', sans-serif; font-size: 16px; line-height: 1.6; padding: 40px; text-align: left; color: #333333; box-shadow: 0 2px 10px rgba(0,0,0,0.1);"
                                                    align="left">

                                                    <!-- Header Banner -->
                                                    <div class="header-banner">
                                                        <h1 style="margin: 0 0 8px 0; font-size: 24px; font-weight: 700;">
                                                            Deposit Acknowledgment
                                                        </h1>
                                                        <div class="status-badge">✓ PAYMENT RECEIVED</div>
                                                    </div>

                                                    <!-- Greeting -->
                                                    <p style="margin: 0 0 20px; font-size: 16px;">
                                                        Dear <strong>{{ReceiverName}}</strong>,
                                                    </p>

                                                    <p style="margin: 0 0 20px; font-size: 16px;">
                                                        This is to acknowledge and confirm that a deposit of
                                                        <strong class="highlight-text">{{Currency}} {{Amount}}</strong>
                                                        was successfully received on
                                                        <strong class="highlight-text">{{Date}}</strong>.
                                                    </p>

                                                    <!-- Amount -->
                                                    <div class="amount-highlight">
                                                        <div class="label">Amount Received</div>
                                                        <div class="amount">{{Currency}} {{Amount}}</div>
                                                        <div style="font-size: 14px; opacity: 0.9;">
                                                            Reference: {{Reference}}
                                                        </div>
                                                    </div>

                                                    <!-- Transaction Details -->
                                                    <div class="details-box">
                                                        <h3>Transaction Details</h3>
                                                        <div class="detail-row">
                                                            <span class="label">Date &amp; Time</span>
                                                            <span class="value">{{Date}}</span>
                                                        </div>
                                                        <div class="detail-row">
                                                            <span class="label">Reference No.</span>
                                                            <span class="value">{{Reference}}</span>
                                                        </div>
                                                        <div class="detail-row">
                                                            <span class="label">Total Cost</span>
                                                            <span class="value">{{Currency}} {{Cost}}</span>
                                                        </div>
                                                    </div>

                                                    <!-- Items (optional, only if Items present) -->
                                                    {{#if Items}}
                                                    <div>
                                                        <h3 style="color: #1e7e34; margin: 0 0 15px 0; font-size: 17px;">
                                                            Items
                                                        </h3>
                                                        <table class="items-table">
                                                            <thead>
                                                                <tr>
                                                                    <th>Item Name</th>
                                                                    <th>Price</th>
                                                                    <th style="text-align: center;">Qty</th>
                                                                    <th style="text-align: right;">Amount</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                {{#each Items}}
                                                                <tr>
                                                                    <td>{{this.name}}</td>
                                                                    <td>{{this.price}}</td>
                                                                    <td style="text-align: center;">{{this.quantity}}</td>
                                                                    <td style="text-align: right;">{{this.amount}}</td>
                                                                </tr>
                                                                {{/each}}
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                    {{/if}}

                                                    <!-- Verification Message -->
                                                    <div class="verification-box">
                                                        <h4>⚠️ Please Verify This Acknowledgment</h4>
                                                        <ul>
                                                            <li><strong>Confirm the amount received matches your records</strong></li>
                                                            <li><strong>Report any discrepancies immediately</strong></li>
                                                            <li><strong>Retain this email as an official transaction record</strong></li>
                                                        </ul>
                                                    </div>

                                                    <!-- Support -->
                                                    <div class="support-box">
                                                        <p style="margin: 0; font-size: 15px; color: #2c5aa0;">
                                                            <strong>Need assistance?</strong> Contact us at
                                                            <a href="mailto:{{CompanyEmail}}" style="color: #2c5aa0; text-decoration: underline;">{{CompanyEmail}}</a>
                                                            or call {{CompanyPhone}}
                                                        </p>
                                                    </div>

                                                    <!-- Closing -->
                                                    <p style="margin: 25px 0 5px 0; font-size: 16px;">
                                                        Thank you for your continued partnership.
                                                    </p>

                                                    <p style="margin: 0 0 5px 0; font-size: 16px;">
                                                        Best regards,
                                                    </p>
                                                    <p style="margin: 0 0 20px 0; font-size: 16px; font-weight: 600; color: #1e7e34;">
                                                        {{CompanyName}}
                                                    </p>

                                                    <!-- Footer -->
                                                    <div style="margin-top: 30px; padding-top: 20px; border-top: 2px solid #e9ecef; text-align: center; font-size: 12px; color: #999;">
                                                        <p style="margin: 0 0 10px 0;">
                                                            {{CompanyName}} | {{CompanyAddress}} | {{CompanyPhone}} | {{CompanyEmail}}
                                                        </p>
                                                        <p style="margin: 0;">
                                                            This email was sent to {{PrimaryEmail}}
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
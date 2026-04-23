using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.Emails.Exceptions
{
    internal class FailedToGenerateQrCodeException : Exception
    {
        public string Message { get; set; }
        public FailedToGenerateQrCodeException(string message) : base(message)
        {
            Message = message;
        }
    }
}

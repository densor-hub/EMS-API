using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Services.QrCodeService
{
    internal interface IQrCodeService
    {
        byte[] GenerateQrCode(string data, int width = 300, int height = 300);

        string GenerateQrCodeBase64(string data, int width = 300, int height = 300);
    }
}

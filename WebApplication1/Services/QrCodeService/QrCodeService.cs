using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using WebApplication1.Services.QrCodeService;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

using FormatException = ZXing.FormatException;

namespace WebApplication1.Services.QrCodeService
{
    internal class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQrCode(string data, int width = 300, int height = 300)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public string GenerateQrCodeBase64(string data, int width = 300, int height = 300)
        {
            var qrCodeBytes = GenerateQrCode(data, width, height);
            return Convert.ToBase64String(qrCodeBytes);
        }

        //public string DecodeBase64QrCode(string base64Image)
        //{
        //    try
        //    {
        //        // Clean the base64 string
        //        var cleanBase64 = base64Image?.Trim();

        //        if (string.IsNullOrEmpty(cleanBase64))
        //            return null;

        //        // Remove data URI prefix if present
        //        if (cleanBase64.Contains("base64,"))
        //        {
        //            cleanBase64 = cleanBase64.Substring(cleanBase64.IndexOf("base64,") + 7);
        //        }

        //        // Convert to bytes
        //        var bytes = Convert.FromBase64String(cleanBase64);

        //        // Decode - Use BarcodeReaderGeneric instead
        //        using var stream = new MemoryStream(bytes);
        //        var reader = new BarcodeReaderGeneric
        //        {
        //            Options = new DecodingOptions
        //            {
        //                TryHarder = true,
        //                PossibleFormats = new[] { BarcodeFormat.QR_CODE }
        //            }
        //        };

        //        var result = reader.Decode(stream);
        //        return result?.Text;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }

}

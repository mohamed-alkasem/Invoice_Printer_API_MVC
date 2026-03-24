using Invoice_printer.Iservives;
using QRCoder;

namespace Invoice_printer.Services
{
    public class QrCodeService : IQrCodeService
    {
        public string GeneratePngDataUrl(string text, int pixelsPerModule = 20)
        {
            using var gen = new QRCodeGenerator();
            using var data = gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qr = new PngByteQRCode(data);
            var bytes = qr.GetGraphic(pixelsPerModule);

            var base64 = Convert.ToBase64String(bytes);
            return $"data:image/png;base64,{base64}";
        }
    }
}
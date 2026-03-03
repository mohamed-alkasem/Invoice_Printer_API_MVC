using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using Invoice_printer.Extensions;


namespace Invoice_printer.Services
{
    public class ReceiptRenderService(AppDbContext _db,
    IWebHostEnvironment _env,
    IQrCodeService _qr) : IReceiptRenderService
    {
        private static string GetMimeType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }

        private string? TryMakeDataUriFromWebRootPath(string? webRelativePath)
        {
            if (string.IsNullOrWhiteSpace(webRelativePath))
                return null;

            // LogoPath: "/uploads/logo.png" أو "uploads/logo.png"
            var rel = webRelativePath.TrimStart('/', '\\');
            var fullPath = Path.Combine(_env.WebRootPath, rel);

            if (!File.Exists(fullPath))
                return null;

            var bytes = File.ReadAllBytes(fullPath);
            var mime = GetMimeType(fullPath);
            var base64 = Convert.ToBase64String(bytes);

            return $"data:{mime};base64,{base64}";
        }

        public async Task<ReceiptPrintViewModel> BuildPrintModelAsync(string userId, int receiptId, string baseUrl)
        {
            var receipt = await _db.Receipts
                .Include(r => r.Items)
                .Include(r => r.Party)
                .Include(r => r.CompanyProfile)
                .Include(r => r.User)
                .Include(r => r.Template)
                .FirstOrDefaultAsync(r => r.Id == receiptId && r.UserId == userId);

            if (receipt is null)
                throw new Exception("Receipt not found.");

            // Currency
            var currency = string.IsNullOrWhiteSpace(receipt.Currency)
                ? (receipt.CompanyProfile?.DefaultCurrency ?? "TRY")
                : receipt.Currency;

            
            var logoUrl = TryMakeDataUriFromWebRootPath(receipt.CompanyProfile?.LogoPath) ?? "";

            var bgUrl = "";
            if (!string.IsNullOrWhiteSpace(receipt.Template?.BackgroundImagePath))
                bgUrl = $"{baseUrl.TrimEnd('/')}/{receipt.Template.BackgroundImagePath.TrimStart('/')}";

            var total = (receipt.Items is not null && receipt.Items.Count > 0)
                ? receipt.Items.Sum(x => x.LineTotal)
                : receipt.Amount;

            var currencyInfo = currency switch
            {
                "TRY" => Utils.Tafqeet.Currencies.TurkishLira,
                "USD" => Utils.Tafqeet.Currencies.USDollar,
                "SAR" => Utils.Tafqeet.Currencies.SaudiRiyal,
                "EUR" => Utils.Tafqeet.Currencies.Euro,
                "IQD" => Utils.Tafqeet.Currencies.IraqiDinar,
                "SYP" => Utils.Tafqeet.Currencies.SyrianPound,
                _ => Utils.Tafqeet.Currencies.TurkishLira
            };

            var totalInWords = total.ToArabicInvoiceText(currencyInfo);

            var qrPayload = receipt.Id.ToString();
            var qrDataUrl = _qr.GeneratePngDataUrl(qrPayload);


            return new ReceiptPrintViewModel
            {
                LogoUrl = logoUrl,
                BackgroundImageUrl = bgUrl,

                CompanyName = receipt.User?.CompanyName ?? "",
                CompanyAddres = receipt.User?.CompanyAddres ?? "",
                WebsiteUrl = receipt.User?.WebsiteUrl ?? "",

                Phone = receipt.CompanyProfile?.Phone ?? "",
                TaxNo = receipt.CompanyProfile?.TaxNo ?? "",
                Currency = currency,

                ReceiptNo = receipt.ReceiptNo,
                Date = receipt.Date,
                Type = receipt.Type.ToString(),
                Status = receipt.Status.ToString(),
                PartyName = receipt.Party?.Name ?? "",
                Description = receipt.Description ?? "",
                PaymentMethod = receipt.PaymentMethod.ToString(),
                SignatureName = receipt.SignatureName ?? "",

                Items = receipt.Items?.ToList() ?? new(),
                Total = total,

                // ✅ جديد
                TotalInWords = totalInWords,
                QrCodeDataUrl = qrDataUrl
            };
        }
    }
}
using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Extensions;
using Invoice_printer.Helpers;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Services
{
    /// <summary>
    /// Implements <see cref="IReceiptRenderService"/>.
    ///
    /// Responsibilities:
    ///   1. Load the full receipt graph from the database.
    ///   2. Build a <see cref="ReceiptPrintViewModel"/> (logo data-URI, QR, amounts in words…).
    ///   3. Decide WHICH rendering path to use and return the final HTML string.
    ///
    /// Rendering decision (inside <see cref="RenderHtmlAsync"/>):
    /// ┌─────────────────────────────────────────────────────────────────┐
    /// │  Template.TemplateMode == Html  AND  Template.HtmlContent ≠ "" │
    /// │   → TemplatePlaceholderEngine.Render(htmlContent, vm)          │
    /// ├─────────────────────────────────────────────────────────────────┤
    /// │  Anything else (Image mode, no HtmlContent, no Template)       │
    /// │   → RazorViewToStringRenderer("Receipt/Print", vm)  [fallback] │
    /// └─────────────────────────────────────────────────────────────────┘
    /// </summary>
    public class ReceiptRenderService(
        AppDbContext _db,
        IWebHostEnvironment _env,
        IQrCodeService _qr,
        IRazorViewToStringRenderer _viewRenderer   // injected for the Razor fallback path
    ) : IReceiptRenderService
    {
        // ── IReceiptRenderService.BuildPrintModelAsync ────────────────────────────

        /// <inheritdoc/>
        public async Task<ReceiptPrintViewModel> BuildPrintModelAsync(
            string userId, int receiptId, string baseUrl)
        {
            // Load full receipt graph in one round-trip.
            var receipt = await _db.Receipts
                .Include(r => r.Items)
                .Include(r => r.Party)
                .Include(r => r.CompanyProfile)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == receiptId && r.UserId == userId);

            if (receipt is null)
                throw new KeyNotFoundException($"Receipt {receiptId} not found for user {userId}.");

            // ── Currency ──────────────────────────────────────────────────────────
            var currency = string.IsNullOrWhiteSpace(receipt.Currency)
                ? (receipt.CompanyProfile?.DefaultCurrency ?? "TRY")
                : receipt.Currency;

            // ── Logo — convert file path to an embedded data-URI ──────────────────
            var logoUrl = TryMakeDataUri(receipt.CompanyProfile?.LogoPath) ?? string.Empty;

            // ── Template background image ─────────────────────────────────────────
            // With the template feature removed, we default to no dynamic background.
            var bgUrl = string.Empty;

            // ── Total — prefer sum of line items; fall back to receipt.Amount ─────
            var total = (receipt.Items is { Count: > 0 })
                ? receipt.Items.Sum(x => x.LineTotal)
                : receipt.Amount;

            // ── Total in words (Arabic tafqeet) ───────────────────────────────────
            var currencyInfo = currency switch
            {
                "TRY" => Utils.Tafqeet.Currencies.TurkishLira,
                "USD" => Utils.Tafqeet.Currencies.USDollar,
                "SAR" => Utils.Tafqeet.Currencies.SaudiRiyal,
                "EUR" => Utils.Tafqeet.Currencies.Euro,
                "IQD" => Utils.Tafqeet.Currencies.IraqiDinar,
                "SYP" => Utils.Tafqeet.Currencies.SyrianPound,
                _     => Utils.Tafqeet.Currencies.TurkishLira
            };

            var totalInWords = total.ToArabicInvoiceText(currencyInfo);

            // ── QR code — embed receipt ID ────────────────────────────────────────
            var qrDataUrl = _qr.GeneratePngDataUrl(receipt.Id.ToString());

            return new ReceiptPrintViewModel
            {
                LogoUrl            = logoUrl,

                // Company (AppUser)
                CompanyName    = receipt.User?.CompanyName    ?? string.Empty,
                CompanyAddres  = receipt.User?.CompanyAddres  ?? string.Empty,
                WebsiteUrl     = receipt.User?.WebsiteUrl     ?? string.Empty,

                // CompanyProfile
                Phone    = receipt.CompanyProfile?.Phone ?? string.Empty,
                TaxNo    = receipt.CompanyProfile?.TaxNo ?? string.Empty,
                Currency = currency,

                // Receipt core
                ReceiptNo     = receipt.ReceiptNo,
                Date          = receipt.Date,
                Type          = receipt.Type.ToString(),
                Status        = receipt.Status.ToString(),
                PartyName     = receipt.Party?.Name ?? string.Empty,
                Description   = receipt.Description ?? string.Empty,
                PaymentMethod = receipt.PaymentMethod.ToString(),
                SignatureName = receipt.SignatureName ?? string.Empty,

                // Financials
                Items        = receipt.Items?.ToList() ?? new List<ReceiptItem>(),
                Total        = total,
                TotalInWords = totalInWords,

                // QR
                QrCodeDataUrl = qrDataUrl
            };
        }

        // ── IReceiptRenderService.RenderHtmlAsync ─────────────────────────────────

        /// <inheritdoc/>
        public async Task<string> RenderHtmlAsync(string userId, int receiptId, string baseUrl)
        {
            var vm = await BuildPrintModelAsync(userId, receiptId, baseUrl);

            // ── Render standard Razor view ────────────────────────────────────────
            return await _viewRenderer.RenderViewToStringAsync("Receipt/Print", vm);
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        /// <summary>
        /// Converts a web-root-relative image path (e.g. <c>/uploads/logo.png</c>)
        /// to a Base64 data-URI so the image is fully self-contained in the HTML.
        /// Returns <c>null</c> if the file does not exist.
        /// </summary>
        private string? TryMakeDataUri(string? webRelativePath)
        {
            if (string.IsNullOrWhiteSpace(webRelativePath))
                return null;

            var rel      = webRelativePath.TrimStart('/', '\\');
            var fullPath = Path.Combine(_env.WebRootPath, rel);

            if (!File.Exists(fullPath))
                return null;

            var bytes  = File.ReadAllBytes(fullPath);
            var mime   = GetMimeType(fullPath);
            var base64 = Convert.ToBase64String(bytes);

            return $"data:{mime};base64,{base64}";
        }

        /// <summary>Maps a file extension to its MIME type.</summary>
        private static string GetMimeType(string path) =>
            Path.GetExtension(path).ToLowerInvariant() switch
            {
                ".png"  => "image/png",
                ".jpg"  => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".gif"  => "image/gif",
                ".svg"  => "image/svg+xml",
                _       => "application/octet-stream"
            };
    }
}
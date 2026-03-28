using Invoice_printer.Iservives;
using Microsoft.Playwright;

namespace Invoice_printer.Services
{
    /// <summary>
    /// Implements <see cref="IReceiptExportService"/>.
    ///
    /// This class is intentionally thin: it owns only the Playwright interaction
    /// (open page → navigate → capture → close).  ALL HTML generation has been
    /// delegated to <see cref="IReceiptRenderService.RenderHtmlAsync"/>, which
    /// applies the correct rendering strategy (custom template or Razor fallback)
    /// based on the receipt's linked template.
    ///
    /// This separation means:
    ///   • Adding a new template engine later requires only touching
    ///     <see cref="Services.ReceiptRenderService"/>, not this file.
    ///   • Export format logic (PDF vs PNG) lives here, not in the render layer.
    /// </summary>
    public class ReceiptExportService(
        IReceiptRenderService _render,          // owns HTML generation decision
        PlaywrightBrowserService _browserService
    ) : IReceiptExportService
    {
        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Encodes an HTML string to a <c>data:text/html;base64,…</c> URL.
        /// Playwright accepts data-URIs so the HTML is loaded without any HTTP
        /// round-trip and without needing a running local web server to serve
        /// static assets (images are already embedded as data-URIs by the renderer).
        /// </summary>
        private static string ToDataUrl(string html) =>
            "data:text/html;base64," +
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(html));

        // ── IReceiptExportService implementations ─────────────────────────────────

        /// <summary>
        /// Exports the receipt as a PDF byte array (A4, no margins, backgrounds printed).
        /// Uses the rendering strategy selected by the receipt's template.
        /// </summary>
        public async Task<byte[]> ExportPdfAsync(string userId, int receiptId, string baseUrl)
        {
            // Delegate ALL HTML building to the render service.
            // It decides: custom placeholder HTML  -OR-  Razor "Receipt/Print" view.
            var html = await _render.RenderHtmlAsync(userId, receiptId, baseUrl);

            var page = await _browserService.Browser.NewPageAsync();
            page.SetDefaultTimeout(30_000);

            try
            {
                await page.GotoAsync(ToDataUrl(html), new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });

                // Small settle delay for web-fonts and any JS-driven layouts.
                await page.WaitForTimeoutAsync(200);

                return await page.PdfAsync(new PagePdfOptions
                {
                    Format          = "A4",
                    PrintBackground = true,
                    PreferCSSPageSize = true,
                    Margin = new Margin
                    {
                        Top    = "0mm",
                        Bottom = "0mm",
                        Left   = "0mm",
                        Right  = "0mm"
                    }
                });
            }
            finally
            {
                // Always close the page to avoid browser tab leaks.
                await page.CloseAsync();
            }
        }

        /// <summary>
        /// Exports the receipt as a full-page PNG screenshot.
        /// Uses the same rendering strategy as <see cref="ExportPdfAsync"/>.
        /// </summary>
        public async Task<byte[]> ExportPngAsync(string userId, int receiptId, string baseUrl)
        {
            // Same delegation pattern — the render service picks the right HTML.
            var html = await _render.RenderHtmlAsync(userId, receiptId, baseUrl);

            var page = await _browserService.Browser.NewPageAsync(new BrowserNewPageOptions
            {
                // A4 at 96 dpi ≈ 794 × 1123 px; we use 1240 × 1754 for higher quality.
                ViewportSize = new ViewportSize { Width = 1240, Height = 1754 }
            });

            page.SetDefaultTimeout(30_000);

            try
            {
                await page.GotoAsync(ToDataUrl(html), new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });

                await page.WaitForTimeoutAsync(200);

                return await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    FullPage = true
                });
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}
using Invoice_printer.Iservives;
using Microsoft.Playwright;

namespace Invoice_printer.Services
{
    
    public class ReceiptExportService(
        IReceiptRenderService _render,          // owns HTML generation decision
        PlaywrightBrowserService _browserService
    ) : IReceiptExportService
    {
     
        private static string ToDataUrl(string html) =>
            "data:text/html;base64," +
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(html));

        // ── IReceiptExportService implementations ─────────────────────────────────

      
        public async Task<byte[]> ExportPdfAsync(string userId, int receiptId, string baseUrl)
        {
           
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
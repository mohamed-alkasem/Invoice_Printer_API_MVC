using Invoice_printer.Iservives;
using Microsoft.Playwright;
using System.Text;

namespace Invoice_printer.Services
{
    public class ReceiptExportService(
        IReceiptRenderService _render,
        IRazorViewToStringRenderer _viewRenderer,
        PlaywrightBrowserService _browserService
    ) : IReceiptExportService
    {
        private static string ToDataUrl(string html)
        {
            return "data:text/html;base64," +
                   Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
        }

        public async Task<byte[]> ExportPdfAsync(string userId, int receiptId, string baseUrl)
        {
            var vm = await _render.BuildPrintModelAsync(userId, receiptId, baseUrl);
            var html = await _viewRenderer.RenderViewToStringAsync("Receipt/Print", vm);

            var page = await _browserService.Browser.NewPageAsync();
            page.SetDefaultTimeout(30000);

            try
            {
                await page.GotoAsync(ToDataUrl(html), new()
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });

                await page.WaitForTimeoutAsync(200);

                return await page.PdfAsync(new()
                {
                    Format = "A4",
                    PrintBackground = true,
                    PreferCSSPageSize = true,
                    Margin = new() { Top = "0mm", Bottom = "0mm", Left = "0mm", Right = "0mm" }
                });
            }
            finally
            {
                await page.CloseAsync();
            }
        }

        public async Task<byte[]> ExportPngAsync(string userId, int receiptId, string baseUrl)
        {
            var vm = await _render.BuildPrintModelAsync(userId, receiptId, baseUrl);
            var html = await _viewRenderer.RenderViewToStringAsync("Receipt/Print", vm);

            var page = await _browserService.Browser.NewPageAsync(new()
            {
                ViewportSize = new() { Width = 1240, Height = 1754 }
            });

            page.SetDefaultTimeout(30000);

            try
            {
                await page.GotoAsync(ToDataUrl(html), new()
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });

                await page.WaitForTimeoutAsync(200);

                return await page.ScreenshotAsync(new()
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
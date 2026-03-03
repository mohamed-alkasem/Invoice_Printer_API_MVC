using Microsoft.Playwright;

namespace Invoice_printer.Services
{
    public class PlaywrightBrowserService : IAsyncDisposable
    {
        private readonly IPlaywright _playwright;
        public IBrowser Browser { get; }

        public PlaywrightBrowserService()
        {
            _playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
            Browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            }).GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await Browser.CloseAsync();
            _playwright.Dispose();
        }
    }
}

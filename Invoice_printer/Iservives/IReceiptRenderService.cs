using Invoice_printer.DTO_S;

namespace Invoice_printer.Iservives
{
    /// <summary>
    /// Contract for the receipt rendering layer.
    ///
    /// Two distinct responsibilities are modelled here:
    ///
    /// 1. <see cref="BuildPrintModelAsync"/> — assembles a <see cref="ReceiptPrintViewModel"/>
    ///    from the database (receipt, party, company, template, QR, etc.).
    ///    This view-model is used by BOTH rendering paths below and is also
    ///    available to callers that need raw data without HTML.
    ///
    /// 2. <see cref="RenderHtmlAsync"/> — produces the final, self-contained HTML
    ///    string that Playwright will consume to generate PDF / PNG.
    ///    Internally it branches on the linked Template's <c>TemplateMode</c>:
    ///      • <c>TemplateMode.Html</c>  → injects data into <c>Template.HtmlContent</c>
    ///        via <see cref="Helpers.TemplatePlaceholderEngine"/>.
    ///      • <c>TemplateMode.Image</c> or no HtmlContent → falls back to the
    ///        default <c>Views/Receipt/Print.cshtml</c> Razor view.
    /// </summary>
    public interface IReceiptRenderService
    {
        /// <summary>
        /// Builds a fully-populated <see cref="ReceiptPrintViewModel"/> for the
        /// given receipt.  Throws if the receipt does not exist or does not belong
        /// to <paramref name="userId"/>.
        /// </summary>
        Task<ReceiptPrintViewModel> BuildPrintModelAsync(string userId, int receiptId, string baseUrl);

        /// <summary>
        /// Returns a complete, self-contained HTML string for the receipt.
        /// The HTML is ready to be loaded directly in Playwright (as a data-URI).
        ///
        /// Rendering strategy:
        /// - When the receipt's template has <c>TemplateMode == Html</c> and a
        ///   non-empty <c>HtmlContent</c>, the placeholder engine is used.
        /// - Otherwise, the built-in <c>Views/Receipt/Print.cshtml</c> is rendered
        ///   via Razor and returned as the fallback.
        /// </summary>
        Task<string> RenderHtmlAsync(string userId, int receiptId, string baseUrl);
    }
}

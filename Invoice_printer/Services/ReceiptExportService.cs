using Invoice_printer.Iservives;
using Invoice_printer.PdfTemplates;
using QuestPDF.Fluent;

namespace Invoice_printer.Services
{
    
    public class ReceiptExportService(
        IReceiptRenderService _render,
        IQuestPdfService _questPdfService
    ) : IReceiptExportService
    {
        // ── IReceiptExportService implementations ─────────────────────────────────

      
        public async Task<byte[]> ExportPdfAsync(string userId, int receiptId, string baseUrl)
        {
            var model = await _render.BuildPrintModelAsync(userId, receiptId, baseUrl);
            return _questPdfService.GenerateReceiptPdf(model);
        }

   
        public async Task<byte[]> ExportPngAsync(string userId, int receiptId, string baseUrl)
        {
            var model = await _render.BuildPrintModelAsync(userId, receiptId, baseUrl);
            var images = new ReceiptDocument(model).GenerateImages();
            return images.FirstOrDefault() ?? Array.Empty<byte>();
        }
    }
}
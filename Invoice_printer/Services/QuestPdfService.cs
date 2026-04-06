using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Invoice_printer.PdfTemplates;
using QuestPDF.Fluent;

namespace Invoice_printer.Services
{
    public class QuestPdfService : IQuestPdfService
    {
        public byte[] GenerateReceiptPdf(ReceiptPrintViewModel model)
        {
            return new ReceiptDocument(model).GeneratePdf();
        }
    }
}

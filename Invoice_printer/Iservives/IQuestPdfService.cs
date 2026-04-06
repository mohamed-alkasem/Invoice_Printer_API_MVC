using Invoice_printer.DTO_S;

namespace Invoice_printer.Iservives
{
    public interface IQuestPdfService
    {
        byte[] GenerateReceiptPdf(ReceiptPrintViewModel model);
    }
}

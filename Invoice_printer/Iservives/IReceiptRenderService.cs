using Invoice_printer.DTO_S;

namespace Invoice_printer.Iservives
{
    
    public interface IReceiptRenderService
    {
       
        Task<ReceiptPrintViewModel> BuildPrintModelAsync(string userId, int receiptId, string baseUrl);
    }
}

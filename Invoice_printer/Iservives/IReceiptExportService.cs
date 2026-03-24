namespace Invoice_printer.Iservives
{
    public interface IReceiptExportService
    {
        Task<byte[]> ExportPdfAsync(string userId, int receiptId, string baseUrl);
        Task<byte[]> ExportPngAsync(string userId, int receiptId, string baseUrl);
    }

}

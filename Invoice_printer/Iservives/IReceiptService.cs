using Invoice_printer.DTO_S;
using Invoice_printer.Models;

namespace Invoice_printer.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<List<Receipt>> GetAllAsync(string userId, ReceiptType? type = null);
        Task<Receipt?> GetByIdAsync(string userId, int id);

        Task<int> CreateAsync(string userId, ReceiptCreateDto dto);

        Task<bool> DeleteAsync(string userId, int id);
        Task<bool> FinalizeAsync(string userId, int id);

        Task<int> AddExportRecordAsync(string userId, int receiptId, ExportFileType fileType, string filePath);
    }
}

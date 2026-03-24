using Invoice_printer.DTO_S;
using Invoice_printer.Models;

namespace Invoice_printer.Services.Interfaces
{
    public interface ITemplateService
    {
        Task<List<Template>> GetAllAsync(string userId, ReceiptType? type = null);
        Task<Template?> GetByIdAsync(string userId, int id);
        Task<int> CreateAsync(string userId, TemplateCreateDto dto);
        Task<bool> UpdateAsync(string userId, TemplateUpdateDto dto);
        Task<bool> DeleteAsync(string userId, int id);

        Task<bool> SetDefaultAsync(string userId, int templateId);
    }
}

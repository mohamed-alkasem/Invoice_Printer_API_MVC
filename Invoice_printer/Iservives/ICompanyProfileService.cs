using Invoice_printer.DTO_S;
using Invoice_printer.Models;

namespace Invoice_printer.Services.Interfaces
{
    public interface ICompanyProfileService
    {
        Task<CompanyProfile?> GetAsync(string userId);
        Task<int> CreateOrUpdateAsync(string userId, CompanyProfileCreateOrUpdateDto dto);
    }
}

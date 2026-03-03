using Invoice_printer.DTO_S;
using Invoice_printer.Models;

namespace Invoice_printer.Services.Interfaces
{
    public interface IPartyService
    {
        Task<List<Party>> GetAllAsync(string userId);
        Task<Party?> GetByIdAsync(string userId, int id);
        Task<int> CreateAsync(string userId, PartyCreateDto dto);
        Task<bool> UpdateAsync(string userId, PartyUpdateDto dto);
        Task<bool> DeleteAsync(string userId, int id);
    }
}

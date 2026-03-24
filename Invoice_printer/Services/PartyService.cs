using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Services
{
    public class PartyService(AppDbContext _db) : IPartyService
    {
        

        public async Task<List<Party>> GetAllAsync(string userId)
        {
            return await _db.Parties
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Party?> GetByIdAsync(string userId, int id)
        {
            return await _db.Parties
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task<int> CreateAsync(string userId, PartyCreateDto dto)
        {
            var party = new Party
            {
                UserId = userId,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Parties.Add(party);
            await _db.SaveChangesAsync();

            return party.Id;
        }

        public async Task<bool> UpdateAsync(string userId, PartyUpdateDto dto)
        {
            var party = await _db.Parties
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.UserId == userId);

            if (party == null)
                return false;

            party.Name = dto.Name;
            party.Phone = dto.Phone;
            party.Address = dto.Address;
            party.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string userId, int id)
        {
            var party = await _db.Parties
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (party == null)
                return false;

            _db.Parties.Remove(party);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}

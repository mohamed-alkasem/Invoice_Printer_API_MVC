using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Services
{
    public class CompanyProfileService(IWebHostEnvironment _env, AppDbContext _db) : ICompanyProfileService
    {
   
        public async Task<CompanyProfile?> GetAsync(string userId)
        {
            return await _db.CompanyProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<int> CreateOrUpdateAsync(string userId, CompanyProfileCreateOrUpdateDto dto)
        {
            var existing = await _db.CompanyProfiles
                .FirstOrDefaultAsync(x => x.UserId == userId);

            string? newLogoPath = null;
            if (dto.LogoFile is not null && dto.LogoFile.Length > 0)
            {
                newLogoPath = await SaveLogoAsync(dto.LogoFile);
            }

            if (existing is null)
            {
                var profile = new CompanyProfile
                {
                    UserId = userId,
                    Phone = dto.Phone,
                    TaxNo = dto.TaxNo,
                    DefaultCurrency = dto.DefaultCurrency,
                    LogoPath = newLogoPath, 
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _db.CompanyProfiles.Add(profile);
                await _db.SaveChangesAsync();
                return profile.Id;
            }

            existing.Phone = dto.Phone;
            existing.TaxNo = dto.TaxNo;
            existing.DefaultCurrency = dto.DefaultCurrency;
            existing.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(newLogoPath))
                existing.LogoPath = newLogoPath;

            await _db.SaveChangesAsync();
            return existing.Id;
        }

        private async Task<string> SaveLogoAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".png", ".jpg", ".jpeg", ".webp" };
            if (!allowed.Contains(ext))
                throw new Exception("Logo format must be png/jpg/jpeg/webp.");

            var folder = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using var stream = File.Create(fullPath);
            await file.CopyToAsync(stream);

            return $"uploads/logos/{fileName}";
        }
    }
}

using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Services
{
    public class TemplateService(AppDbContext _db, IFileStorage _files) : ITemplateService
    {
        public async Task<int> CreateAsync(string userId, TemplateCreateDto dto)
        {
            if (!dto.Type.HasValue)
                throw new ArgumentException("Type is required.", nameof(dto.Type));

            if (!dto.TemplateMode.HasValue)
                throw new ArgumentException("TemplateMode is required.", nameof(dto.TemplateMode));

            // ✅ احفظ الصورة إذا موجودة
            string? bgPath = null;
            if (dto.BackgroundImageFile is not null && dto.BackgroundImageFile.Length > 0)
                bgPath = await _files.SaveImageAsync(dto.BackgroundImageFile, "template-bg");

            var template = new Template
            {
                UserId = userId,
                Type = dto.Type.Value,
                TemplateMode = dto.TemplateMode.Value,
                Name = dto.Name,

                BackgroundImagePath = bgPath,

                SettingsJson = dto.SettingsJson,

                HtmlContent = dto.HtmlContent,
                IsDefault = dto.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _db.Templates.AddAsync(template);

            if (dto.IsDefault)
            {
                await ResetDefaultForUserAndTypeAsync(userId, dto.Type.Value);
                template.IsDefault = true;
            }

            await _db.SaveChangesAsync();
            return template.Id;
        }

        public async Task<bool> DeleteAsync(string userId, int id)
        {
            var existing = await _db.Templates
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (existing is null)
                return false;

            await _files.DeleteIfExistsAsync(existing.BackgroundImagePath);

            _db.Templates.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<List<Template>> GetAllAsync(string userId, ReceiptType? type = null)
        {
            var query = _db.Templates
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            if (type.HasValue)
                query = query.Where(x => x.Type == type.Value);

            return await query
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Template?> GetByIdAsync(string userId, int id)
        {
            return await _db.Templates
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task<bool> SetDefaultAsync(string userId, int templateId)
        {
            var template = await _db.Templates
                .FirstOrDefaultAsync(x => x.Id == templateId && x.UserId == userId);

            if (template is null)
                return false;

            await ResetDefaultForUserAndTypeAsync(userId, template.Type);

            template.IsDefault = true;
            template.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(string userId, TemplateUpdateDto dto)
        {
            var existing = await _db.Templates
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.UserId == userId);

            if (existing is null)
                return false;

            existing.Type = dto.Type;
            existing.TemplateMode = dto.TemplateMode;
            existing.Name = dto.Name;
            existing.SettingsJson = dto.SettingsJson;
            existing.HtmlContent = dto.HtmlContent;
            existing.UpdatedAt = DateTime.UtcNow;

            // ✅ 1) إذا بدو يحذف الخلفية
            if (dto.RemoveBackground)
            {
                await _files.DeleteIfExistsAsync(existing.BackgroundImagePath);
                existing.BackgroundImagePath = null;
            }
            // ✅ 2) إذا رفع صورة جديدة
            else if (dto.BackgroundImageFile is not null && dto.BackgroundImageFile.Length > 0)
            {
                await _files.DeleteIfExistsAsync(existing.BackgroundImagePath);
                existing.BackgroundImagePath = await _files.SaveImageAsync(dto.BackgroundImageFile, "template-bg");
            }
            // ✅ 3) إذا ما رفع شي وما طلب حذف: لا تغيّر BackgroundImagePath

            if (dto.IsDefault)
            {
                await ResetDefaultForUserAndTypeAsync(userId, dto.Type);
                existing.IsDefault = true;
            }
            else
            {
                existing.IsDefault = false;
            }

            await _db.SaveChangesAsync();
            return true;
        }




        private async Task ResetDefaultForUserAndTypeAsync(string userId, ReceiptType type)
        {
            var defaults = await _db.Templates
                .Where(x => x.UserId == userId && x.Type == type && x.IsDefault)
                .ToListAsync();

            foreach (var t in defaults)
            {
                t.IsDefault = false;
                t.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}

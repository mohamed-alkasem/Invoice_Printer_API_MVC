using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Services
{
    public class ReceiptService(AppDbContext _db) : IReceiptService
    {
        public async Task<List<Receipt>> GetAllAsync(string userId, ReceiptType? type = null)
        {
            IQueryable<Receipt> query = _db.Receipts
                .Where(x => x.UserId == userId)
                .Include(x => x.Party);

            if (type.HasValue)
                query = query.Where(x => x.Type == type.Value);

            return await query
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Receipt?> GetByIdAsync(string userId, int id)
        {
            return await _db.Receipts
                .Include(x => x.Party)
                .Include(x => x.CompanyProfile)
                .Include(x => x.Items)
                .Include(x => x.Exports)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task<int> CreateAsync(string userId, ReceiptCreateDto dto)
        {
            var type = dto.Type;
            var partyId = dto.PartyId;

            var company = await _db.CompanyProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.CompanyProfileId && x.UserId == userId);

            if (company is null)
                throw new InvalidOperationException("CompanyProfile not found for this user.");

            var partyExists = await _db.Parties
                .AsNoTracking()
                .AnyAsync(x => x.Id == partyId && x.UserId == userId);

            if (!partyExists)
                throw new InvalidOperationException("Party not found for this user.");

            var receiptNo = await GenerateReceiptNoAsync(userId, type);

            var receipt = new Receipt
            {
                UserId = userId,
                CompanyProfileId = dto.CompanyProfileId,
                PartyId = partyId,
                Type = type,
                ReceiptNo = receiptNo,
                Date = dto.Date,
                Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "TRY" : dto.Currency,
                Description = dto.Description,
                PaymentMethod = dto.PaymentMethod,
                SignatureName = dto.SignatureName,

                Status = ReceiptStatus.Draft,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.Items is not null && dto.Items.Count > 0)
            {
                receipt.Items = new List<ReceiptItem>();

                foreach (var itemDto in dto.Items)
                {
                    var lineTotal = itemDto.Quantity * itemDto.UnitPrice;

                    receipt.Items.Add(new ReceiptItem
                    {
                        Title = itemDto.Title,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        LineTotal = lineTotal
                    });
                }

                receipt.Amount = receipt.Items.Sum(x => x.LineTotal);
            }
            else
            {
                receipt.Amount = 0;
            }

            _db.Receipts.Add(receipt);
            await _db.SaveChangesAsync();

            return receipt.Id;
        }

        public async Task<bool> DeleteAsync(string userId, int id)
        {
            var receipt = await _db.Receipts
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (receipt is null)
                return false;

            if (receipt.Status == ReceiptStatus.Final)
                return false;

            _db.Receipts.Remove(receipt);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FinalizeAsync(string userId, int id)
        {
            var receipt = await _db.Receipts
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (receipt is null)
                return false;

            if (receipt.Status == ReceiptStatus.Final)
                return true;

            receipt.Status = ReceiptStatus.Final;
            receipt.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddExportRecordAsync(string userId, int receiptId, ExportFileType fileType, string filePath)
        {
            var receiptExists = await _db.Receipts
                .AsNoTracking()
                .AnyAsync(x => x.Id == receiptId && x.UserId == userId);

            if (!receiptExists)
                throw new InvalidOperationException("Receipt not found for this user.");

            var export = new ReceiptExport
            {
                ReceiptId = receiptId,
                FileType = fileType,
                FilePath = filePath,
                CreatedAt = DateTime.UtcNow
            };

            _db.ReceiptExports.Add(export);
            await _db.SaveChangesAsync();

            return export.Id;
        }

        private async Task<string> GenerateReceiptNoAsync(string userId, ReceiptType type)
        {
            var year = DateTime.UtcNow.Year;
            var prefix = type == ReceiptType.Payment ? "PAY" : "COL";

            // Find the highest existing number for this user, type, and year
            // Format is PREFIX-YEAR-NUMBER (e.g., COL-2026-000005)
            var lastReceipt = await _db.Receipts
                .Where(x => x.UserId == userId && x.Type == type && x.Date.Year == year)
                .OrderByDescending(x => x.ReceiptNo)
                .Select(x => x.ReceiptNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastReceipt != null)
            {
                // Extract the last 6 digits
                var parts = lastReceipt.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            var numberStr = nextNumber.ToString
                ("D6");
            return $"{prefix}-{year}-{numberStr}";
        }
    }
}

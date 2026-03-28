using Invoice_printer.Models;

namespace Invoice_printer.DTO_S
{
    /// <summary>
    /// A flat, serialization-safe DTO returned by the API for Receipt objects.
    /// This avoids circular reference errors that occur when serializing raw EF entities
    /// with navigation properties (Receipt → Party → Receipts → ...).
    /// </summary>
    public class ReceiptResponseDto
    {
        public int    Id            { get; set; }
        public string ReceiptNo     { get; set; } = string.Empty;
        public string Type          { get; set; } = string.Empty;
        public string Status        { get; set; } = string.Empty;
        public DateTime Date        { get; set; }
        public decimal Amount       { get; set; }
        public string Currency      { get; set; } = string.Empty;
        public string? Description  { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? SignatureName { get; set; }
        public DateTime CreatedAt   { get; set; }
        public DateTime UpdatedAt   { get; set; }

        // Company
        public int     CompanyProfileId { get; set; }
        public string? CompanyPhone     { get; set; }
        public string? CompanyTaxNo     { get; set; }
        public string? CompanyCurrency  { get; set; }

        // Party
        public int     PartyId   { get; set; }
        public string? PartyName { get; set; }

        // Line items
        public List<ReceiptItemResponseDto> Items { get; set; } = new();
    }

    public class ReceiptItemResponseDto
    {
        public int     Id        { get; set; }
        public string  Title     { get; set; } = string.Empty;
        public decimal Quantity  { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    /// <summary>
    /// Lightweight summary used in list endpoints (GetAll) to reduce payload size.
    /// </summary>
    public class ReceiptSummaryDto
    {
        public int      Id          { get; set; }
        public string   ReceiptNo   { get; set; } = string.Empty;
        public string   Type        { get; set; } = string.Empty;
        public string   Status      { get; set; } = string.Empty;
        public DateTime Date        { get; set; }
        public decimal  Amount      { get; set; }
        public string   Currency    { get; set; } = string.Empty;
        public string?  PartyName   { get; set; }
        public string?  Description { get; set; }
    }

    /// <summary>
    /// Extension methods to map Receipt entity → response DTOs without AutoMapper.
    /// </summary>
    public static class ReceiptMappingExtensions
    {
        public static ReceiptSummaryDto ToSummaryDto(this Receipt r) => new()
        {
            Id          = r.Id,
            ReceiptNo   = r.ReceiptNo,
            Type        = r.Type.ToString(),
            Status      = r.Status.ToString(),
            Date        = r.Date,
            Amount      = r.Amount,
            Currency    = r.Currency,
            PartyName   = r.Party?.Name,
            Description = r.Description
        };

        public static ReceiptResponseDto ToResponseDto(this Receipt r) => new()
        {
            Id              = r.Id,
            ReceiptNo       = r.ReceiptNo,
            Type            = r.Type.ToString(),
            Status          = r.Status.ToString(),
            Date            = r.Date,
            Amount          = r.Amount,
            Currency        = r.Currency,
            Description     = r.Description,
            PaymentMethod   = r.PaymentMethod.ToString(),
            SignatureName   = r.SignatureName,
            CreatedAt       = r.CreatedAt,
            UpdatedAt       = r.UpdatedAt,

            CompanyProfileId = r.CompanyProfileId,
            CompanyPhone     = r.CompanyProfile?.Phone,
            CompanyTaxNo     = r.CompanyProfile?.TaxNo,
            CompanyCurrency  = r.CompanyProfile?.DefaultCurrency,

            PartyId   = r.PartyId,
            PartyName = r.Party?.Name,

            Items = r.Items?.Select(i => new ReceiptItemResponseDto
            {
                Id        = i.Id,
                Title     = i.Title,
                Quantity  = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal  = i.LineTotal
            }).ToList() ?? new()
        };
    }
}

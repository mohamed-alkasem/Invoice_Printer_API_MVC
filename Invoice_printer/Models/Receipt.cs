

using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.Models
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;

        [Required]
        public int CompanyProfileId { get; set; }
        public CompanyProfile CompanyProfile { get; set; } = default!;

        [Required]
        public int PartyId { get; set; }
        public Party Party { get; set; } = default!;

        [Required]
        public int TemplateId { get; set; }
        public Template Template { get; set; } = default!;

        [Required]
        public ReceiptType Type { get; set; } 

        [Required, MaxLength(50)]
        public string ReceiptNo { get; set; } = default!; 

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Range(0, 999999999)]
        public decimal Amount { get; set; }

        [MaxLength(10)]
        public string Currency { get; set; } = "TRY";

        [MaxLength(500)]
        public string? Description { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [MaxLength(100)]
        public string? SignatureName { get; set; }

        public string? SignatureImagePath { get; set; }

        public ReceiptStatus Status { get; set; } = ReceiptStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ReceiptItem>? Items { get; set; } 
        public ICollection<ReceiptExport>? Exports { get; set; } 
    }
}

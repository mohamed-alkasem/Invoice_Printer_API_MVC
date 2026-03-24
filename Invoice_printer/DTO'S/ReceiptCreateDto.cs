using System.ComponentModel.DataAnnotations;
using Invoice_printer.Models;

namespace Invoice_printer.DTO_S
{
    public class ReceiptCreateDto
    {
        [Required]
        public int CompanyProfileId { get; set; }

        [Required]
        public int PartyId { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [Required]
        public ReceiptType Type { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MaxLength(10)]
        public string Currency { get; set; } = "TRY";

        [MaxLength(500)]
        public string? Description { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        public string? SignatureName { get; set; }

        public List<ReceiptItemDto> Items { get; set; } = new();
    }
}

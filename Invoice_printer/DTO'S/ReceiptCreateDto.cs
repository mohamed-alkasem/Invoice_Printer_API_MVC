using System.ComponentModel.DataAnnotations;
using Invoice_printer.Models;

namespace Invoice_printer.DTO_S
{
    public class ReceiptCreateDto
    {
        [Required]
        public int CompanyProfileId { get; set; }

        [Required]
        [Display(Name = "Receipt_PartyId")]
        public int PartyId { get; set; }

        [Required]
        [Display(Name = "Receipt_TemplateId")]
        public int TemplateId { get; set; }

        [Required]
        [Display(Name = "Receipt_Type")]
        public ReceiptType Type { get; set; }

        [Display(Name = "Receipt_Date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MaxLength(10)]
        [Display(Name = "Receipt_Currency")]
        public string Currency { get; set; } = "TRY";

        [MaxLength(500)]
        [Display(Name = "Receipt_Description")]
        public string? Description { get; set; }

        [Display(Name = "Receipt_PaymentMethod")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [Display(Name = "Receipt_SignatureName")]
        public string? SignatureName { get; set; }

        public List<ReceiptItemDto> Items { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.Models
{
    public class ReceiptItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReceiptId { get; set; }
        public Receipt Receipt { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [Range(0, 999999999)]
        public decimal Quantity { get; set; } = 1;

        [Range(0, 999999999)]
        public decimal UnitPrice { get; set; }

        [Range(0, 999999999)]
        public decimal LineTotal { get; set; } 
    }
}

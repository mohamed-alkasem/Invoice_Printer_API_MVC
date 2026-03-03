using System.ComponentModel.DataAnnotations;
using Invoice_printer.Models;

namespace Invoice_printer.DTO_S
{
    public class ReceiptItemDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = default!;

        [Range(0, 999999999)]

        public decimal Quantity { get; set; } = 1;

        [Range(0, 999999999)]
        public decimal UnitPrice { get; set; }
    }

   
}

using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.DTO_S
{
    public class PartyCreateDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? TaxNo { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

  
}

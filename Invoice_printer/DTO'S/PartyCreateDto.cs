using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.DTO_S
{
    public class PartyCreateDto
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Party_Name")]
        public string Name { get; set; } = default!;

        [MaxLength(50)]
        [Display(Name = "Party_Phone")]
        public string? Phone { get; set; }

        [MaxLength(300)]
        [Display(Name = "Party_Address")]
        public string? Address { get; set; }

        [MaxLength(50)]
        [Display(Name = "Party_TaxNo")]
        public string? TaxNo { get; set; }

        [MaxLength(500)]
        [Display(Name = "Party_Notes")]
        public string? Notes { get; set; }
    }

  
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoice_printer.Models
{
    public class CompanyProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? TaxNo { get; set; }

        [MaxLength(400)]
        public string? LogoPath { get; set; }

        [MaxLength(10)]
        public string? DefaultCurrency { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

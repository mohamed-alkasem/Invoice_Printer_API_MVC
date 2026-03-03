using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.Models
{
    public class Party
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Receipt>? Receipts { get; set; } 
    }
}

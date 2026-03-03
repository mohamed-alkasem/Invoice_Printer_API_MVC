using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.Models
{
    public class Template
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;

        [Required]
        public ReceiptType Type { get; set; } 

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        [Required]
        public TemplateMode TemplateMode { get; set; } 

        public string? BackgroundImagePath { get; set; }

        public string? SettingsJson { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Receipt>? Receipts { get; set; }
        public string? HtmlContent { get; set; }
    }

}

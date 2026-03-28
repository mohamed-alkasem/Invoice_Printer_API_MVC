using System.ComponentModel.DataAnnotations;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Http;

namespace Invoice_printer.DTO_S
{
    public class TemplateCreateDto
    {
        [Required]
        public ReceiptType? Type { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        [Required]
        public TemplateMode? TemplateMode { get; set; }

        public IFormFile? BackgroundImageFile { get; set; }

        
        public string? BackgroundImagePath { get; set; }

        // ✅ إذا قررت لاحقاً تلغيها أو تخليها داخلياً
        public string? SettingsJson { get; set; }

        public bool IsDefault { get; set; }

        public string? HtmlContent { get; set; }

        public string? RequiredCustomFields { get; set; }
    }
}

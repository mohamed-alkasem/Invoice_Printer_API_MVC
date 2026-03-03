using System.ComponentModel.DataAnnotations;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Http;

namespace Invoice_printer.DTO_S
{
    public class TemplateCreateDto
    {
        [Required]
        [Display(Name = "Template_Type")]
        public ReceiptType? Type { get; set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Template_Name")]
        public string Name { get; set; } = default!;

        [Required]
        [Display(Name = "Template_TemplateMode")]
        public TemplateMode? TemplateMode { get; set; }

        [Display(Name = "Template_BackgroundImageFile")]
        public IFormFile? BackgroundImageFile { get; set; }

        
        public string? BackgroundImagePath { get; set; }

        // ✅ إذا قررت لاحقاً تلغيها أو تخليها داخلياً
        [Display(Name = "Template_SettingsJson")]
        public string? SettingsJson { get; set; }

        [Display(Name = "Template_IsDefault")]
        public bool IsDefault { get; set; }

        [Display(Name = "Template_HtmlContent")]
        public string? HtmlContent { get; set; }
    }
}

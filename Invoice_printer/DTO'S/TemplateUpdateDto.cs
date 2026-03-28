using System.ComponentModel.DataAnnotations;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Http;

namespace Invoice_printer.DTO_S
{
    public class TemplateUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public ReceiptType Type { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;

        [Required]
        public TemplateMode TemplateMode { get; set; }

        // ✅ رفع ملف جديد (اختياري)
        public IFormFile? BackgroundImageFile { get; set; }

        // ✅ المسار الحالي (للعرض فقط، السيرفر بيقرره)
        public string? BackgroundImagePath { get; set; }

        // ✅ (اختياري) إذا بدك زر "حذف الخلفية"
        public bool RemoveBackground { get; set; }

        public string? SettingsJson { get; set; }

        public bool IsDefault { get; set; }

        public string? HtmlContent { get; set; }

        public string? RequiredCustomFields { get; set; }
    }
}

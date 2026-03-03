using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Invoice_printer.DTO_S
{
    public class CompanyProfileCreateOrUpdateDto
    {
        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? TaxNo { get; set; }

        [MaxLength(10)]
        public string? DefaultCurrency { get; set; } = "TRY";

        public IFormFile? LogoFile { get; set; }

        [MaxLength(400)]
        public string? LogoPath { get; set; }
    }
}

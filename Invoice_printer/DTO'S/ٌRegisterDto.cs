using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.DTO_S
{
    public class RegisterDto
    {
        [Required]
        public string CompanyName { get; set; } 

        [Required]
        public string CompanyAddres { get; set; } 

        public string? WebsiteUrl { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } 

        [Required]
        public string UserName { get; set; } 

        [Required]
        public string Password { get; set; } 
    }
}

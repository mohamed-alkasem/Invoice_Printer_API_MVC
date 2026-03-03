using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.DTO_S
{
    public class RegisterDto
    {
        [Required]
        [Display(Name = "Register_CompanyName")]
        public string CompanyName { get; set; } 

        [Required]
        [Display(Name = "Register_CompanyAddres")]
        public string CompanyAddres { get; set; } 

        [Display(Name = "Register_WebsiteUrl")]
        public string? WebsiteUrl { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Register_Email")]
        public string Email { get; set; } 

        [Required]
        [Display(Name = "Register_UserName")]
        public string UserName { get; set; } 

        [Required]
        [Display(Name = "Register_Password")]
        public string Password { get; set; } 
    }
}

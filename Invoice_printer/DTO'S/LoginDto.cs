using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.DTO_S
{
    public class LoginDto
    {
        [Required]
        [Display(Name = "Login_UserNameOrEmail")]
        public string UserNameOrEmail { get; set; } 

        [Required]
        [Display(Name = "Login_Password")]
        public string Password { get; set; } 

        [Display(Name = "Login_RememberMe")]
        public bool RememberMe { get; set; }
    }
}

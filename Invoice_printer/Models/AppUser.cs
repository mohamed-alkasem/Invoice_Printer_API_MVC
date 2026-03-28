using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Invoice_printer.Models
{
    public class AppUser : IdentityUser
    {
        
        public string CompanyName { get; set; }
        public String CompanyAddres { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? NameSurname { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}

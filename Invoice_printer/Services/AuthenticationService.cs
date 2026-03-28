using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Identity;

namespace Invoice_printer.Services
{
    public class AuthenticationService : Iauthentication
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _db;

        public AuthenticationService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public async Task<IdentityResult> Register(RegisterDto register)
        {
            var user = new AppUser
            {
                UserName = register.UserName,
                Email = register.Email,
                CompanyName = register.CompanyName,
                CompanyAddres = register.CompanyAddres,
                WebsiteUrl = register.WebsiteUrl
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
                return result;

            // إنشاء ملف تعريف الشركة افتراضياً
            if (!_db.CompanyProfiles.Any(x => x.UserId == user.Id))
            {
                _db.CompanyProfiles.Add(new CompanyProfile
                {
                    UserId = user.Id,
                    DefaultCurrency = "TRY"
                });
                await _db.SaveChangesAsync();
            }

            return result;
        }

        public async Task<bool> Login(LoginDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserNameOrEmail) ??
                       await _userManager.FindByEmailAsync(login.UserNameOrEmail);

            if (user == null)
                return false;

            var ok = await _userManager.CheckPasswordAsync(user, login.Password);
            if (!ok) return false;

            await _signInManager.SignInAsync(user, login.RememberMe);
            return true;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
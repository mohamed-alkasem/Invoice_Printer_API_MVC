using System.Diagnostics.Eventing.Reader;
using Invoice_printer.Data;
using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Invoice_printer.Services
{
    public class AuthenticationService : Iauthentication
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        public AuthenticationService(SignInManager<AppUser> signInManager ,UserManager<AppUser> userManager, AppDbContext db)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._db = db;
        }
        public async Task<IdentityResult> Register(RegisterDto register)
        {
            var user = new AppUser
            {
                CompanyName = register.CompanyName,
                Email = register.Email,
                UserName = register.UserName,
                CompanyAddres = register.CompanyAddres,
                WebsiteUrl = register.WebsiteUrl,
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
                return result;

            try
            {
                var exists = _db.CompanyProfiles.Any(x => x.UserId == user.Id);
                if (!exists)
                {
                    _db.CompanyProfiles.Add(new CompanyProfile
                    {
                        UserId = user.Id,
                        DefaultCurrency = "TRY"
                    });

                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                await _userManager.DeleteAsync(user);

                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Şirket profili oluşturulamadı. Lütfen tekrar deneyin."
                });
            }

            return result;
        }

           
       
       public async Task<bool> Login(LoginDto login)
{
    AppUser? user = await _userManager.FindByNameAsync(login.UserNameOrEmail);
    if (user is null)
        user = await _userManager.FindByEmailAsync(login.UserNameOrEmail);

    if (user is null)
        return false;

    var ok = await _userManager.CheckPasswordAsync(user, login.Password);
    if (!ok)
        return false;

    await _signInManager.SignInAsync(user, login.RememberMe);
    return true;
}

   public async Task Logout()
                {
                    await _signInManager.SignOutAsync();
                }

    
    }
}

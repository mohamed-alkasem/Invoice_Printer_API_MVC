using Invoice_printer.DTO_S;
using Microsoft.AspNetCore.Identity;

namespace Invoice_printer.Iservives
{
    public interface Iauthentication
    {
        Task<IdentityResult> Register(RegisterDto register);
        Task<bool> Login(LoginDto login);
        Task Logout();
    }
}

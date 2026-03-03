using Azure.Messaging;
using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers
{
    public class AuthController(Iauthentication auth ) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto) {

            if (!ModelState.IsValid)
                return View(dto);

            var result = await auth.Register(dto);
            if (result.Succeeded) 
                return RedirectToAction("Login");

            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);
            return View(dto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await auth.Login(dto);

            if (ok)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Kullanıcı adı/e-posta veya şifre yanlış.");
            return View(dto);
        }


        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await auth.Logout();
            return RedirectToAction("Login");
        }
    }
}

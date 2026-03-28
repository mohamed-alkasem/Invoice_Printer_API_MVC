using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers
{
    public class AuthController : Controller
    {
        private readonly Iauthentication _auth;
        public AuthController(Iauthentication auth) => _auth = auth;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _auth.Login(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "اسم المستخدم أو كلمة المرور خاطئة.");
                return View(dto);
            }

            // Redirect واضح للصفحة الرئيسية
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _auth.Register(dto);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);

                return View(dto);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await _auth.Logout();
            return RedirectToAction("Login");
        }
    }
}
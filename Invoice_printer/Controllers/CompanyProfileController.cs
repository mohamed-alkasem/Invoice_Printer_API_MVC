using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers
{
    [Authorize]
    public class CompanyProfileController(ICompanyProfileService _companyProfileService) : Controller
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Edit()
        {
            var profile = await _companyProfileService.GetAsync(UserId);

            var dto = new CompanyProfileCreateOrUpdateDto
            {
                Phone = profile?.Phone,
                TaxNo = profile?.TaxNo,
                LogoPath = profile?.LogoPath,          
                DefaultCurrency = profile?.DefaultCurrency
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyProfileCreateOrUpdateDto dto)
        {
            TempData["dbg"] = "POST Edit hit ✅";

            if (!ModelState.IsValid)
            {
                var profile = await _companyProfileService.GetAsync(UserId);
                dto.LogoPath = profile?.LogoPath;

                TempData["dbg"] = "POST hit لكن ModelState INVALID ❌";
                return View(dto);
            }

            var id = await _companyProfileService.CreateOrUpdateAsync(UserId, dto);
            TempData["dbg"] = $"Saved ✅ Id={id}";

            return RedirectToAction(nameof(Edit));
        }
    }
}

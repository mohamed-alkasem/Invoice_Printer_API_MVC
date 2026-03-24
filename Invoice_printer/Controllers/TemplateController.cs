using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers
{
    [Authorize]
    public class TemplateController(ITemplateService _templateService) : Controller
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index(ReceiptType? type = null)
        {
            var templates = await _templateService.GetAllAsync(UserId, type);
            ViewBag.Type = type;
            return View(templates);
        }

        public IActionResult Create()
        {
            return View(new TemplateCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TemplateCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _templateService.CreateAsync(UserId, dto);
            return RedirectToAction(nameof(Index), new { type = dto.Type });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var template = await _templateService.GetByIdAsync(UserId, id);
            if (template is null) return NotFound();

            var dto = new TemplateUpdateDto
            {
                Id = template.Id,
                Name = template.Name,
                Type = template.Type,
                TemplateMode = template.TemplateMode,
                BackgroundImagePath = template.BackgroundImagePath,
                SettingsJson = template.SettingsJson,
                HtmlContent = template.HtmlContent, // ✅ مهم
                IsDefault = template.IsDefault
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] TemplateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var existing = await _templateService.GetByIdAsync(UserId, dto.Id);
                if (existing is not null)
                    dto.BackgroundImagePath = existing.BackgroundImagePath;

                return View(dto);
            }

            var ok = await _templateService.UpdateAsync(UserId, dto);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index), new { type = dto.Type });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await _templateService.GetByIdAsync(UserId, id);
            if (template is null) return NotFound();

            var ok = await _templateService.DeleteAsync(UserId, id);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index), new { type = template.Type });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var template = await _templateService.GetByIdAsync(UserId, id);
            if (template is null) return NotFound();

            var ok = await _templateService.SetDefaultAsync(UserId, id);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index), new { type = template.Type });
        }
    }
}

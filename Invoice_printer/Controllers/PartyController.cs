using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers
{
    [Authorize]
    public class PartyController(IPartyService _partyService) : Controller
    {
        
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var parties = await _partyService.GetAllAsync(UserId);
            return View(parties);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new PartyCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _partyService.CreateAsync(UserId, dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var party = await _partyService.GetByIdAsync(UserId, id);
            if (party is null) return NotFound();

            var dto = new PartyUpdateDto
            {
                Id = party.Id,
                Name = party.Name,
                Phone = party.Phone,
                Address = party.Address,
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PartyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _partyService.UpdateAsync(UserId, dto);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        { 
            var ok = await _partyService.DeleteAsync(UserId, id);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}

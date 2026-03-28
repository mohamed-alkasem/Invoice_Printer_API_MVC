using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers
{
    [Authorize(Policy = "CookiePolicy")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ReceiptController(
        IReceiptService _receiptService,
        IPartyService _partyService,
        ICompanyProfileService _companyProfileService,
        IReceiptExportService _exportService
    ) : Controller
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> Index(ReceiptType? type = null)
        {
            var receipts = await _receiptService.GetAllAsync(UserId, type);
            ViewBag.Type = type;
            return View(receipts);
        }

        [HttpGet("[controller]/Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(UserId, id);
            if (receipt is null) return NotFound();
            return View(receipt);
        }

        [HttpGet("[controller]/Create")]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("[controller]/Create/{type}")]
        public async Task<IActionResult> CreateByType(ReceiptType type)
        {
            var profile = await _companyProfileService.GetAsync(UserId);
            if (profile is null)
                return RedirectToAction("Edit", "CompanyProfile");

            ViewBag.Parties = await _partyService.GetAllAsync(UserId);

            var dto = new ReceiptCreateDto
            {
                CompanyProfileId = profile.Id,
                Type = type,
                Date = DateTime.UtcNow,
                Currency = profile.DefaultCurrency ?? "TRY"
            };

            return View("Create", dto);
        }

        [HttpPost("[controller]/Create/{type}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReceiptType type, ReceiptCreateDto dto)
        {
            var profile = await _companyProfileService.GetAsync(UserId);
            if (profile is null)
                return RedirectToAction("Edit", "CompanyProfile");

            dto.Type = type;

            dto.CompanyProfileId = profile.Id;

            async Task FillViewBags()
            {
                ViewBag.Parties = await _partyService.GetAllAsync(UserId);
            }

            if (!ModelState.IsValid)
            {
                await FillViewBags();
                return View("Create", dto);
            }

            try
            {
                var id = await _receiptService.CreateAsync(UserId, dto);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", message);
                await FillViewBags();
                return View("Create", dto);
            }
        }

        [HttpPost("[controller]/Finalize/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalize(int id)
        {
            var ok = await _receiptService.FinalizeAsync(UserId, id);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("[controller]/Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _receiptService.DeleteAsync(UserId, id);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("[controller]/Export/{id:int}")]
        public async Task<IActionResult> Export(int id, ExportFileType type, bool download = false)
        {
            var userId = UserId;
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            if (type == ExportFileType.Pdf)
            {
                var bytes = await _exportService.ExportPdfAsync(userId, id, baseUrl);

                if (download)
                    return File(bytes, "application/pdf", $"receipt-{id}.pdf");

                Response.Headers["Content-Disposition"] = $"inline; filename=receipt-{id}.pdf";
                return File(bytes, "application/pdf");
            }

            var png = await _exportService.ExportPngAsync(userId, id, baseUrl);

            if (download)
                return File(png, "image/png", $"receipt-{id}.png");

            Response.Headers["Content-Disposition"] = $"inline; filename=receipt-{id}.png";
            return File(png, "image/png");
        }
    }
}
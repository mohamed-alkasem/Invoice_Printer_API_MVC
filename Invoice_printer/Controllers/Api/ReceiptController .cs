using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Helpers;
using Invoice_printer.Iservives;
using Invoice_printer.Models;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers.Api
{
   
    [Authorize(Policy = "JwtPolicy", AuthenticationSchemes = "JwtBearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptApiController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly ICompanyProfileService _companyProfileService;
        private readonly IReceiptExportService _exportService;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public ReceiptApiController(
            IReceiptService receiptService,
            ICompanyProfileService companyProfileService,
            IReceiptExportService exportService)
        {
            _receiptService        = receiptService;
            _companyProfileService = companyProfileService;
            _exportService         = exportService;
        }

        // ── GET Api/Receipt ───────────────────────────────────────────────────────

       
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ReceiptSummaryDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] ReceiptType? type = null)
        {
            var receipts = await _receiptService.GetAllAsync(UserId, type);

            // Map to a flat DTO — avoids circular reference serialization errors
            var dtos = receipts.Select(r => r.ToSummaryDto()).ToList();

            return Ok(ApiResponse<List<ReceiptSummaryDto>>.Ok(dtos, "Receipts retrieved successfully."));
        }

        // ── GET Api/Receipt/{id} ──────────────────────────────────────────────────

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReceiptResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(UserId, id);

            if (receipt is null)
                return NotFound(ApiResponse<object>.NotFound($"Receipt with ID {id} was not found."));

            return Ok(ApiResponse<ReceiptResponseDto>.Ok(receipt.ToResponseDto(), "Receipt retrieved successfully."));
        }

        // ── POST Api/Receipt ──────────────────────────────────────────────────────

       
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Create([FromBody] ReceiptCreateDto dto)
        {
            // Resolve company profile — required for every receipt.
            var profile = await _companyProfileService.GetAsync(UserId);
            if (profile is null)
            {
                return BadRequest(ApiResponse<object>.BadRequest(
                    "No company profile found. Please create your company profile first."));
            }

            dto.CompanyProfileId = profile.Id;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<List<string>>.ValidationFailed(errors));
            }

            try
            {
                var id = await _receiptService.CreateAsync(UserId, dto);

                return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<object>.Created(new { Id = id }, "Receipt created successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.BadRequest(ex.Message));
            }
        }

        // ── POST Api/Receipt/Finalize/{id} ────────────────────────────────────────

        [HttpPost("Finalize/{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Finalize(int id)
        {
            var ok = await _receiptService.FinalizeAsync(UserId, id);

            if (!ok)
                return NotFound(ApiResponse<object>.NotFound($"Receipt with ID {id} was not found."));

            return Ok(ApiResponse<object>.Ok(new { Id = id }, "Receipt finalized successfully."));
        }

        // ── DELETE Api/Receipt/{id} ───────────────────────────────────────────────

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _receiptService.DeleteAsync(UserId, id);

            if (!ok)
                return NotFound(ApiResponse<object>.NotFound($"Receipt with ID {id} was not found."));

            return Ok(ApiResponse<object>.Ok(new { Id = id }, "Receipt deleted successfully."));
        }

        // ── GET Api/Receipt/Export/{id} ───────────────────────────────────────────

        
        [HttpGet("Export/{id:int}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> Export(
            int id,
            [FromQuery] ExportFileType type,
            [FromQuery] bool download = false)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            try
            {
                if (type == ExportFileType.Pdf)
                {
                    var bytes      = await _exportService.ExportPdfAsync(UserId, id, baseUrl);
                    var fileName   = $"receipt-{id}.pdf";
                    var contentType = "application/pdf";

                    // download=true  → browser saves file; false → browser renders inline.
                    return download
                        ? File(bytes, contentType, fileName)
                        : File(bytes, contentType);
                }
                else
                {
                    var bytes    = await _exportService.ExportPngAsync(UserId, id, baseUrl);
                    var fileName = $"receipt-{id}.png";

                    return download
                        ? File(bytes, "image/png", fileName)
                        : File(bytes, "image/png");
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.NotFound($"Receipt with ID {id} was not found."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ServerError(ex.Message));
            }
        }
    }
}
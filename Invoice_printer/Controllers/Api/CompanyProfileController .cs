using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Helpers;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers.Api
{
    // ── Task 1: Explicit JwtBearer scheme prevents cookie auth from being evaluated.
    //            Both [Authorize] attributes work together:
    //              • Policy = "JwtPolicy"                  → runs the named policy rules
    //              • AuthenticationSchemes = "JwtBearer"  → tells ASP.NET which scheme
    //                to use for this controller ONLY, ignoring Identity cookie defaults.
    // "JwtBearer" is the custom scheme name used in Program.cs: .AddJwtBearer("JwtBearer", ...)
    // Do NOT use JwtBearerDefaults.AuthenticationScheme (= "Bearer") — that scheme is not registered.
    [Authorize(Policy = "JwtPolicy", AuthenticationSchemes = "JwtBearer")]
    [Route("Api/CompanyProfile")]
    [ApiController]
    public class CompanyProfileApiController : ControllerBase
    {
        private readonly ICompanyProfileService _companyProfileService;

        public CompanyProfileApiController(ICompanyProfileService companyProfileService)
        {
            _companyProfileService = companyProfileService;
        }

        /// <summary>Extracts the authenticated user's ID from the JWT claims.</summary>
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ── GET Api/CompanyProfile ────────────────────────────────────────────────

        /// <summary>Returns the company profile for the authenticated user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<CompanyProfileCreateOrUpdateDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Get()
        {
            var profile = await _companyProfileService.GetAsync(UserId);

            if (profile is null)
                return NotFound(ApiResponse<object>.NotFound("Company profile not found."));

            var dto = new CompanyProfileCreateOrUpdateDto
            {
                Phone           = profile.Phone,
                TaxNo           = profile.TaxNo,
                LogoPath        = profile.LogoPath,
                DefaultCurrency = profile.DefaultCurrency
            };

            return Ok(ApiResponse<CompanyProfileCreateOrUpdateDto>.Ok(dto,
                "Company profile retrieved successfully."));
        }

        // ── POST Api/CompanyProfile ───────────────────────────────────────────────

        /// <summary>Creates or updates the company profile for the authenticated user.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Post([FromBody] CompanyProfileCreateOrUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<List<string>>.ValidationFailed(errors));
            }

            var id = await _companyProfileService.CreateOrUpdateAsync(UserId, dto);

            return Ok(ApiResponse<object>.Ok(new { Id = id }, "Company profile saved successfully."));
        }
    }
}
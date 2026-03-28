using System.Security.Claims;
using Invoice_printer.DTO_S;
using Invoice_printer.Helpers;
using Invoice_printer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers.Api
{
    // ── Task 1: Belt-and-suspenders auth scheme declaration.
    //            [Authorize(Policy)] enforces the named policy requirements.
    //            [Authorize(AuthenticationSchemes)] tells ASP.NET exactly WHICH
    //            scheme to authenticate against — preventing cookie auth fallback.
    // "JwtBearer" is the custom scheme name used in Program.cs: .AddJwtBearer("JwtBearer", ...)
    // Do NOT use JwtBearerDefaults.AuthenticationScheme (= "Bearer") — that scheme is not registered.
    [Authorize(Policy = "JwtPolicy", AuthenticationSchemes = "JwtBearer")]
    [Route("Api/Party")]
    [ApiController]
    public class PartyApiController : ControllerBase
    {
        private readonly IPartyService _partyService;

        public PartyApiController(IPartyService partyService)
        {
            _partyService = partyService;
        }

        /// <summary>Extracts the authenticated user's ID from the JWT claims.</summary>
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ── GET Api/Party ─────────────────────────────────────────────────────────

        /// <summary>Returns all parties belonging to the authenticated user.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PartyCreateDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var parties = await _partyService.GetAllAsync(UserId);

            return Ok(ApiResponse<object>.Ok(parties, "Parties retrieved successfully."));
        }

        // ── GET Api/Party/{id} ────────────────────────────────────────────────────

        /// <summary>Returns a single party by ID (must belong to the authenticated user).</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var party = await _partyService.GetByIdAsync(UserId, id);

            if (party is null)
                return NotFound(ApiResponse<object>.NotFound($"Party with ID {id} was not found."));

            return Ok(ApiResponse<object>.Ok(party, "Party retrieved successfully."));
        }

        // ── POST Api/Party ────────────────────────────────────────────────────────

        /// <summary>Creates a new party for the authenticated user.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Create([FromBody] PartyCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<List<string>>.ValidationFailed(errors));
            }

            var id = await _partyService.CreateAsync(UserId, dto);

            return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<object>.Created(new { Id = id }, "Party created successfully."));
        }

        // ── PUT Api/Party/{id} ────────────────────────────────────────────────────

        /// <summary>Updates an existing party. The route ID must match the DTO ID.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] PartyUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(ApiResponse<object>.BadRequest("Route ID does not match the body ID."));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<List<string>>.ValidationFailed(errors));
            }

            var ok = await _partyService.UpdateAsync(UserId, dto);

            if (!ok)
                return NotFound(ApiResponse<object>.NotFound($"Party with ID {id} was not found."));

            return Ok(ApiResponse<object>.Ok(new { Id = id }, "Party updated successfully."));
        }

        // ── DELETE Api/Party/{id} ─────────────────────────────────────────────────

        /// <summary>Deletes a party (must belong to the authenticated user).</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _partyService.DeleteAsync(UserId, id);

            if (!ok)
                return NotFound(ApiResponse<object>.NotFound($"Party with ID {id} was not found."));

            return Ok(ApiResponse<object>.Ok(new { Id = id }, "Party deleted successfully."));
        }
    }
}
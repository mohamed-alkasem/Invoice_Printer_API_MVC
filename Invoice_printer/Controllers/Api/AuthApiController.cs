using Invoice_printer.DTO_S;
using Invoice_printer.Iservives;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthApiController : ControllerBase
    {
        private readonly Iauthentication _auth;

        public AuthApiController(Iauthentication auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auth.Register(dto);

            if (result.Succeeded)
                return Ok(new { message = "User registered successfully" });

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _auth.Login(dto);

            if (!ok)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { message = "Login successful" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _auth.Logout();
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
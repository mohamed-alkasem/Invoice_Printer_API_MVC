using Invoice_printer.DTO_S;
using Invoice_printer.Helpers;
using Invoice_printer.Iservives;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_printer.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthApiService _auth;

        public AuthApiController(IAuthApiService auth)
        {
            _auth = auth;
        }

        // ✅ Register
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<List<string>>.ValidationFailed(errors));
            }

            var success = await _auth.Register(dto);
            if (!success)
                return BadRequest(ApiResponse<object>.BadRequest("Registration failed."));

            return Ok(ApiResponse<object>.Ok(new { }, "User registered successfully."));
        }

        // ✅ Login (Now returns Access + Refresh)
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<List<string>>.ValidationFailed(errors));
            }

            var response = await _auth.Login(dto);
            if (response == null)
                return Unauthorized(ApiResponse<object>.BadRequest("Invalid email or password."));

            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Login successful."));
        }

        // ✅ Refresh Token
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.BadRequest("Invalid input."));

            var response = await _auth.RefreshToken(dto);
            if (response == null) return BadRequest(ApiResponse<object>.BadRequest("Invalid or expired token."));

            return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Token refreshed successfully."));
        }

        // ✅ Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.BadRequest("Invalid email."));

            var token = await _auth.ForgotPassword(dto);
            if (token == null) return BadRequest(ApiResponse<object>.BadRequest("User not found correctly."));

            return Ok(ApiResponse<object>.Ok(new { Token = token }, "Reset token generated successfully."));
        }

        // ✅ Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.BadRequest("Invalid input."));

            var success = await _auth.ResetPassword(dto);
            if (!success) return BadRequest(ApiResponse<object>.BadRequest("Failed to reset password. Token may be invalid."));

            return Ok(ApiResponse<object>.Ok(new { }, "Password reset successfully."));
        }
    }
}
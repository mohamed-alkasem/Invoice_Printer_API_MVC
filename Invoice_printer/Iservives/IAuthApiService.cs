    using Invoice_printer.DTO_S;

    namespace Invoice_printer.Iservives
    {
        public interface IAuthApiService
        {
            Task<AuthResponseDto?> Login(LoginDto dto);
            Task<bool> Register(RegisterDto dto);
            Task<AuthResponseDto?> RefreshToken(RefreshTokenRequestDto dto);
            Task<string?> ForgotPassword(ForgotPasswordDto dto);
            Task<bool> ResetPassword(ResetPasswordDto dto);
        }
    }

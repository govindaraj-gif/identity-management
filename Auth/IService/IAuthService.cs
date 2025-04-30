using Auth.DTO;
using Auth.Models;
using Azure;
using System.Security.Claims;

namespace Auth.IService
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterDto registerDto);
        Task<LoginReponseDto> Login(LoginDto loginDto);
        Task<List<Person>> GetAllUsers();
        Task<bool> Enable2FA(Guid userId, bool enabled);
        Task<TwoFactorVerificationResponse> Verify2FA(TwoFactorVerificationDto dto);
        Task<TwoFactorDetailsDto> Get2FASetup(string userId);
        Task<string> ForgetPassword(ForgotPasswordRequestDto body);
        Task<bool> ResetPassword(ResetPasswordRequestDto body);
        Task<bool> ChangePassword(ClaimsPrincipal userClaim, ChangePasswordDto body);

    }
}
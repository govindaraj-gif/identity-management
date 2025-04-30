using Auth.DTO;
using Auth.IService;
using Auth.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _accountService;

        public AuthController(IAuthService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<bool> TenzingRegister(RegisterDto registerDto)
        {
            try
            {
                return await _accountService.Register(registerDto);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<LoginReponseDto> TenzingLogin(LoginDto loginDto)
        {
            try
            {
                return await _accountService.Login(loginDto);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("persons")]
        public async Task<List<Person>> GetAllUsers()
        {
            return await _accountService.GetAllUsers();
        }

        [HttpPut("Enable2FA")]
        [AllowAnonymous]
        public async Task<bool> Enable2FA(Guid userId, bool enabled)
        {
            return await _accountService.Enable2FA(userId , enabled);
        }

        [AllowAnonymous]
        [HttpPost("Verify2FA")]
        public async Task<TwoFactorVerificationResponse> Verify2FA(TwoFactorVerificationDto dto)
        {
            return await _accountService.Verify2FA(dto);
        }

        [HttpGet("Get2FASetup")]
        [AllowAnonymous]
        public async Task<TwoFactorDetailsDto> Get2FASetup(string userId)
        {
            return await _accountService.Get2FASetup(userId);
        }

        [AllowAnonymous]
        [HttpPost("ForgetPassword")]
        public async Task<string> ForgetPassword(ForgotPasswordRequestDto body)
        {
            return await _accountService.ForgetPassword(body);
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<bool> ResetPassword(ResetPasswordRequestDto body)
        {
            return await _accountService.ResetPassword(body);
        }

        [HttpPost("ChangePassword")]
        [AllowAnonymous]
        public async Task<bool> ChangePassword(ChangePasswordDto body)
        {
            return await _accountService.ChangePassword(User, body);
        }
    }
}

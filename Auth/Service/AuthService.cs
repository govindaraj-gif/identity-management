using Auth.DTO;
using Auth.IService;
using Auth.Models;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAccountRepo _account;
        private readonly string _signingKey;
        private readonly string _audience;
        private readonly string _issuer;
        private readonly AuthPracticeContext _context;

        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<AppSettings> appsettings, IAccountRepo accountRepo,
            AuthPracticeContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signingKey = appsettings?.Value?.JWT?.SigningKey ?? string.Empty;
            _audience = appsettings?.Value?.JWT?.Audience ?? string.Empty;
            _issuer = appsettings?.Value?.JWT?.Issuer ?? string.Empty;
            _context = context;
        }

        public async Task<LoginReponseDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email!);
            if (user is null)
            {
                throw new Exception("Invalid login attempt.");
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                throw new Exception("Your account is locked out, please try after some time.");
            }
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password!))
            {
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                    throw new Exception("Your account has been locked as you have reached max login attempts.");

                throw new Exception("Invalid login attempt.");
            }
            else
            {
                //string jwtString = await GenerateJwtToken(user);



                //var result = await _account.ValidateUser(new Guid(user.Id));
                var result = await ValidateUser(new Guid(user.Id));

                AuthReponseDto? response = UserValidationMapper.ToUserValidationReponseDto(result);
                if (response == null)
                {
                    throw new Exception("Failed to login");
                }
                //var employerClaim = await _context.UserClaims.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ClaimType == "Employer");
                //Employer? employer = null;
                //if (employerClaim != null)
                //{
                //    employer = await _context.Employers.FirstOrDefaultAsync(x => x.ErId == new Guid(employerClaim.ClaimValue!));
                //}
                LoginReponseDto dtoToReturn = new LoginReponseDto()
                {
                    MemberId = new Guid(user.Id),
                    UserName = response?.UserName,
                    Email = response?.Email,
                    UserRoles = response?.UserRoles,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    AccessToken = user.TwoFactorEnabled ? null : await GenerateJwtToken(user),
                };
                await _userManager.ResetAccessFailedCountAsync(user);

                return dtoToReturn;
            }
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var signingCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(_signingKey)),
                    SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
            claims.Add(new Claim(ClaimTypes.Email, user.Email!));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<string>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleClaims.Add(role.Id);
                }
            }
            claims.AddRange((roleClaims)
                    .Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtObject = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(300),
                signingCredentials: signingCredentials);
            var jwtString = new JwtSecurityTokenHandler()
                .WriteToken(jwtObject);
            return jwtString;
        }

        public async Task<bool> Register(RegisterDto registerDto)
        {
            var existedUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existedUser != null)
            {
                throw new Exception("User name is already taken");
            }
            foreach (var role in registerDto.Roles)
            {
                var roleExist = await _roleManager.FindByNameAsync(role);
                if (roleExist == null)
                    throw new Exception($"{role} does not exist");
            }

            var newUser = new IdentityUser();
            newUser.UserName = registerDto.UserName;
            newUser.Email = registerDto.Email;


            var result = await _userManager.CreateAsync(
                newUser, registerDto.Password!);
            if (result.Succeeded)
            {
                var roleAdded = await _userManager.AddToRolesAsync(newUser, registerDto.Roles);
                //if (registerDto.IsRestricted)
                //{
                //    await _userManager.AddClaimAsync(newUser, new Claim("Restricted", "True"));
                //}
                //if (registerDto.employerId != null)
                //{
                //    await _userManager.AddClaimAsync(newUser, new Claim("Employer", registerDto.employerId.ToString()!));
                //}
                return true;
            }
            else
                throw new Exception(
                    string.Format("Error: {0}", string.Join(" ",
                        result.Errors.Select(e => e.Description))));
        }

        public async Task<IdentityValidateResultDto> ValidateUser(Guid memberId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == memberId.ToString());
            //var role = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == memberId.ToString());
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == memberId.ToString());
            var role = userRole != null
                ? await _context.Roles.FirstOrDefaultAsync(x => x.Id == userRole.RoleId)
                : null;

            return new IdentityValidateResultDto
            {
                UserName = user?.UserName,
                Email = user?.Email,
                RoleId = role?.Id,
                RoleName = role?.Name,
                MemberNo = 0
            };

        }

        public async Task<List<Person>> GetAllUsers()
        {
            var result = await _context.People.ToListAsync();

            return result;

        }

        public async Task<bool> Enable2FA(Guid userId, bool enabled)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new Exception($"user does not exist");
            }
            var result = await _userManager.SetTwoFactorEnabledAsync(user, enabled);
            if (!result.Succeeded)
            {
                return false;
            }
            return true;
        }

        public async Task<TwoFactorVerificationResponse> Verify2FA(TwoFactorVerificationDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            TwoFactorVerificationResponse response = new TwoFactorVerificationResponse();
            //var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                response.IsVerified = false;
                throw new Exception("Invalid user id");
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                dto.Code);

            if (!isValid)
            {
                response.IsVerified = false;
                throw new Exception("Invalid Code");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            response.IsVerified = true;
            response.AccessToken = await GenerateJwtToken(user);
            return response;
        }

        public async Task<TwoFactorDetailsDto> Get2FASetup(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            //var user = await _userManager.GetUserAsync(appUser); // Or use userId if doing external setup

            var key = await _userManager.GetAuthenticatorKeyAsync(user!);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user!);
                key = await _userManager.GetAuthenticatorKeyAsync(user!);
            }

            var email = await _userManager.GetEmailAsync(user!);
            var appName = "Test"; // Will show up in Authenticator app

            var qrCodeUri = GenerateQrCodeUri(email!, key!, appName);

            var result = new TwoFactorDetailsDto()
            {
                SharedKey = FormatKey(key!),
                QRCodeUri = qrCodeUri,
            };

            return result;
        }

        private string GenerateQrCodeUri(string email, string unformattedKey, string appName)
        {
            return $"otpauth://totp/{appName}:{email}?secret={unformattedKey}&issuer={appName}&digits=6";
        }

        private string FormatKey(string key)
        {
            return string.Join(" ", Regex.Matches(key, ".{1,4}").Select(m => m.Value)).ToLowerInvariant();
        }

        public async Task<string> ForgetPassword(ForgotPasswordRequestDto body)
        {

            var user = await _userManager.FindByEmailAsync(body.Email!);
            if (user == null)
            {
                return "";
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"resetpassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";

            //var resetLink = _urlHelper.Action("ResetPassword", "Account", new
            //{
            //    token,
            //    email = user.Email
            //}, "https");

            // TODO: Send resetLink via email using an email service
            Console.WriteLine($"Reset link: {resetLink}");

            return resetLink;
        }

        public async Task<bool> ResetPassword(ResetPasswordRequestDto body)
        {
            var user = await _userManager.FindByEmailAsync(body.Email!);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var result = await _userManager.ResetPasswordAsync(user, body.Token!, body.NewPassword!);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ChangePassword(ClaimsPrincipal userClaim, ChangePasswordDto body)
        {
            var user = await _userManager.GetUserAsync(userClaim);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, body.CurrentPassword!, body.NewPassword!);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using Auth.DTO;
using Auth.IService;
using Auth.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service
{
    public class AccountRepo : IAccountRepo
    {
        private readonly AuthPracticeContext _context;
        public AccountRepo(AuthPracticeContext context)
        {
            _context = context;
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

    }
}

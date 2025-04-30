using Auth.DTO;

namespace Auth.IService
{
    public interface IAccountRepo
    {
        Task<IdentityValidateResultDto> ValidateUser(Guid memberId);
    }

}

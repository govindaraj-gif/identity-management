namespace Auth.DTO
{
    public record IdentityValidateResultDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public int MemberNo { get; set; }
    }

    public static class UserValidationMapper
    {
        public static AuthReponseDto? ToUserValidationReponseDto(this IdentityValidateResultDto resultDtos)
        {
            if (resultDtos == null)
            {
                return null;
            }

            var firstResult = resultDtos;

            return new AuthReponseDto
            {
                UserName = firstResult.UserName,
                Email = firstResult.Email,
                MemberNo = firstResult.MemberNo,
                UserRoles = new List<UserRolesDto>
                {
                    new UserRolesDto
                    {
                        RoleId = firstResult.RoleId,
                        RoleName = firstResult.RoleName
                    }
                }
            };
        }
    }
}

namespace Auth.DTO
{
    public record LoginReponseDto
    {
        public Guid MemberId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<UserRolesDto>? UserRoles { get; set; }
        public Guid? employerId { get; set; }
        public string? employerName { get; set; }
        public string? AccessToken { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }

    public record AuthReponseDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<UserRolesDto>? UserRoles { get; set; }
        public int MemberNo { get; set; }

    }


    public record UserRolesDto
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}

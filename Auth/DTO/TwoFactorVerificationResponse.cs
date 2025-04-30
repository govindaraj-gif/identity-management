namespace Auth.DTO
{
    public class TwoFactorVerificationResponse
    {
        public bool IsVerified { get; set; }
        public string? AccessToken { get; set; }
    }

    public class ExternalLoginDto
    {
        public string Provider { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;
    }

    public class TwoFactorVerificationDto
    {
        public string Code { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class TwoFactorDetailsDto
    {
        public string SharedKey { get; set; } = string.Empty;
        public string QRCodeUri { get; set; } = string.Empty;
    }
}

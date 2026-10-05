namespace WebApplication1.Domain.DTO
{
    public class AuthResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public IdAndNameDTO Company { get; set; }
        public List<DropDownDTO>? Locations { get; set; }
        public List<DropDownDTO>? Routes { get; set; }
        public bool IsCreator { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpires { get; set; }

    }

    public class AuthTokensDTO
    {
        public DateTime AccessTokenExpires { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class SetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

}

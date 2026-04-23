namespace WebApplication1.Domain.DTO
{
    public class AuthTokensDTO
    {
        public DateTime AccessTokenExpires { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

namespace WebApplication1.Services.TokenService
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        public int TokenExpirationMinutes { get; set; } = 15;
        public int ClockSkewSeconds { get; set; } = 0;

        public int RefreshTokenExpirationDays { get; set; } = 7;
        public int? RefreshTokenAbsoluteExpirationDays { get; set; } = 30;
        public int RefreshTokenRetentionDays { get; set; } = 30;

        public RefreshCookieOptions RefreshCookie { get; set; } = new();
        public string RefreshHeaderName { get; set; } = "X-Refresh-Token";
    }
    
}
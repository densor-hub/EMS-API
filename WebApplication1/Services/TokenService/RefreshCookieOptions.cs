namespace WebApplication1.Services.TokenService
{
    public class RefreshCookieOptions
    {
        public string Name { get; set; } = "refresh_token";
        public bool? Secure { get; set; }        // null = infer from request
        public string SameSite { get; set; } = "Lax";
        public string Path { get; set; } = "/api/auth";
        public bool HttpOnly { get; set; } = true;
        public string? Domain { get; set; }
    }
}
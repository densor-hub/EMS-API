namespace WebApplication1.Services.TokenService
{
    public class RefreshCookieOptions
    {
        public string       Name     { get; set; } = "refresh_token";
        public string       Path     { get; set; } = "/";
        public string?      Domain   { get; set; }
        public bool         HttpOnly { get; set; } = true;
        public bool         Secure   { get; set; } = true;
        public SameSiteMode SameSite { get; set; } = SameSiteMode.Strict;
    }
}
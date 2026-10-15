namespace WebApplication1.Services.TokenService
{
    public static class CookieNames
    {
        public const string RefreshToken = "refresh_token";
        public const string AccessToken = "access_token";
        public const string RefreshHeaderName = "X-Refresh-Token";
        public const string ClientTypeHeaderName   = "X-Client-Type";
        public const string  AuthorizationHeaderName = "Authorization";
    }
}
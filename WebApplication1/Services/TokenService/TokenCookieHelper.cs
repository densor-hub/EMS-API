using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Services.TokenService
{
    public class TokenCookieHelper
    {
        private const int DefaultRefreshDays   = 7;
        private const int DefaultAccessMinutes = 15;

        private readonly int _refreshDays;
        private readonly int _accessMinutes;

        public TokenCookieHelper(IConfiguration config)
        {
            _refreshDays   = config.GetValue<int?>("Jwt:RefreshTokenExpirationDays") ?? DefaultRefreshDays;
            _accessMinutes = config.GetValue<int?>("Jwt:TokenExpirationMinutes")     ?? DefaultAccessMinutes;
        }

        // ---------------------------------------------------------------------
        // Options
        // ---------------------------------------------------------------------
        private static CookieOptions BuildOptions(DateTimeOffset? expires = null) => new()
        {
            HttpOnly = true,
            Secure   = true,
            SameSite = SameSiteMode.Strict,   // Lax if SPA + API are on sibling subdomains
            Path     = "/",
            Expires  = expires
        };

        // ---------------------------------------------------------------------
        // Refresh token
        // ---------------------------------------------------------------------
        public void SetRefreshToken(HttpResponse response, string token)
            => response.Cookies.Append(
                CookieNames.RefreshToken,
                token,
                BuildOptions(DateTimeOffset.UtcNow.AddDays(_refreshDays)));

        public string? GetRefreshToken(HttpRequest request)
        {
            var header = request.Headers[CookieNames.RefreshHeaderName].FirstOrDefault();
            if (!string.IsNullOrEmpty(header)) return header;

            return request.Cookies[CookieNames.RefreshToken];
        }

        public void ClearRefreshToken(HttpResponse response)
            => response.Cookies.Append(
                CookieNames.RefreshToken,
                string.Empty,
                BuildOptions(DateTimeOffset.UtcNow.AddDays(-1)));

        // ---------------------------------------------------------------------
        // Access token
        // ---------------------------------------------------------------------
        public void SetAccessToken(HttpResponse response, string token)
            => response.Cookies.Append(
                CookieNames.AccessToken,
                token,
                BuildOptions(DateTimeOffset.UtcNow.AddMinutes(_accessMinutes)));

        public string? GetAccessToken(HttpRequest request)
        {
            var authHeader = request.Headers[CookieNames.AuthorizationHeaderName].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader["Bearer ".Length..].Trim();
            }

            return request.Cookies[CookieNames.AccessToken];
        }

        public void ClearAccessToken(HttpResponse response)
            => response.Cookies.Append(
                CookieNames.AccessToken,
                string.Empty,
                BuildOptions(DateTimeOffset.UtcNow.AddDays(-1)));

        // ---------------------------------------------------------------------
        // Convenience
        // ---------------------------------------------------------------------
        public void ClearAll(HttpResponse response)
        {
            ClearRefreshToken(response);
            ClearAccessToken(response);
        }

        // ---------------------------------------------------------------------
        // Mobile transport
        // ---------------------------------------------------------------------
        public void ExposeRefreshTokenToMobile(HttpResponse response, string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;

            response.Headers[CookieNames.RefreshHeaderName] = refreshToken;
            response.Headers.CacheControl                   = "no-store";
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WebApplication1.Services.TokenService
{
    public class TokenCookieHelper
    {
        private const int DefaultRefreshDays   = 7;
        private const int DefaultAccessMinutes = 15;

        private readonly int _refreshDays;
        private readonly int _accessMinutes;
        private readonly JwtOptions _jwt;
        private readonly IHostEnvironment _env;

        public TokenCookieHelper(
            IConfiguration config,
            IOptions<JwtOptions> jwt,
            IHostEnvironment env)
        {
            _jwt           = jwt.Value;
            _env           = env;
            _refreshDays   = _jwt.RefreshTokenExpirationDays > 0
                             ? _jwt.RefreshTokenExpirationDays : DefaultRefreshDays;
            _accessMinutes = _jwt.TokenExpirationMinutes > 0
                             ? _jwt.TokenExpirationMinutes : DefaultAccessMinutes;
        }

        // Build from JwtOptions.RefreshCookie — the single source of truth.
        private CookieOptions BuildOptions(DateTimeOffset? expires = null) => new()
        {
            HttpOnly = _jwt.RefreshCookie.HttpOnly,
            Secure   = _jwt.RefreshCookie.Secure && !_env.IsDevelopment(),
            SameSite = _jwt.RefreshCookie.SameSite,
            Path     = _jwt.RefreshCookie.Path,
            Domain   = _jwt.RefreshCookie.Domain,
            Expires  = expires,
        };

        // ---------- refresh ----------
        public void SetRefreshToken(HttpResponse response, string token)
            => response.Cookies.Append(
                _jwt.RefreshCookie.Name,
                token,
                BuildOptions(DateTimeOffset.UtcNow.AddDays(_refreshDays)));

        public string? GetRefreshToken(HttpRequest request)
        {
            var header = request.Headers[_jwt.RefreshHeaderName].FirstOrDefault();
            if (!string.IsNullOrEmpty(header)) return header;

            return request.Cookies[_jwt.RefreshCookie.Name];
        }

        public void ClearRefreshToken(HttpResponse response)
        {
            var opts = BuildOptions(DateTimeOffset.UtcNow.AddDays(-1));
            response.Cookies.Append(_jwt.RefreshCookie.Name, string.Empty, opts);
            response.Cookies.Delete(_jwt.RefreshCookie.Name, opts);   // belt-and-suspenders
        }

        // ---------- access ----------
        public void SetAccessToken(HttpResponse response, string token)
            => response.Cookies.Append(
                CookieNames.AccessToken,
                token,
                BuildOptions(DateTimeOffset.UtcNow.AddMinutes(_accessMinutes)));

        public string? GetAccessToken(HttpRequest request)
        {
            var auth = request.Headers[CookieNames.AuthorizationHeaderName].FirstOrDefault();
            if (!string.IsNullOrEmpty(auth) &&
                auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return auth["Bearer ".Length..].Trim();

            return request.Cookies[CookieNames.AccessToken];
        }

        public void ClearAccessToken(HttpResponse response)
        {
            var opts = BuildOptions(DateTimeOffset.UtcNow.AddDays(-1));
            response.Cookies.Append(CookieNames.AccessToken, string.Empty, opts);
            response.Cookies.Delete(CookieNames.AccessToken, opts);
        }

        // ---------- convenience ----------
        public void ClearAll(HttpResponse response)
        {
            ClearRefreshToken(response);
            ClearAccessToken(response);
        }

        // ---------- mobile ----------
        public void ExposeRefreshTokenToMobile(HttpResponse response, string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;
            response.Headers[_jwt.RefreshHeaderName] = refreshToken;
            response.Headers.CacheControl            = "no-store";
        }
    }
}
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
// using WebApplication1.Extensions;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Middleware
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RefreshTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
            var cookieHelper = context.RequestServices.GetRequiredService<TokenCookieHelper>();

            // 1. Public endpoints: pass through
            if (IsPublicEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // 2. Try access token
            var accessToken = cookieHelper.GetAccessToken(context.Request);
            if (!string.IsNullOrEmpty(accessToken))
            {
                var principal = tokenService.ValidateAccessToken(accessToken);
                if (principal is not null)
                {
                    context.User = principal;
                    await _next(context);
                    return;
                }
            }

            // 3. Try refresh token
            var refreshToken = cookieHelper.GetRefreshToken(context.Request);
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var result = await tokenService.TryRefreshAsync(refreshToken);
                if (result is not null)
                {
                    cookieHelper.SetRefreshToken(context.Response, result.NewRefreshToken);
                    cookieHelper.SetAccessToken(context.Response,  result.NewAccessToken);

                    if (context.Request.IsMobileClient())
                        cookieHelper.ExposeRefreshTokenToMobile(context.Response, result.NewRefreshToken);

                    var principal = tokenService.ValidateAccessToken(result.NewAccessToken);
                    if (principal is not null)
                    {
                        context.User = principal;
                        await _next(context);
                        return;
                    }
                }

                await WriteUnauthorized(context, "Invalid or expired refresh token");
                return;
            }

            await WriteUnauthorized(context, "Authentication required");
        }

        // ---------------------------------------------------------------------
        private static async Task WriteUnauthorized(HttpContext context, string message)
        {
            if (context.Response.HasStarted) return;
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = message });
        }

        private static bool IsPublicEndpoint(PathString path)
        {
            var publicEndpoints = new[]
            {
                "/Auth/Login",
                "/Auth/Register",
                "/Auth/Refresh-Token",
                "/Auth/Account/Confirm",
                "/Auth/Set-Password",
                "/Auth/Forgot-Password",
                "/swagger",
                "/health",
                "/error"
            };

            return publicEndpoints.Any(endpoint =>
                path.StartsWithSegments(endpoint, StringComparison.OrdinalIgnoreCase));
        }
    }
}
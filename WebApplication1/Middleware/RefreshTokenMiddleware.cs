using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.TokenService;

namespace WebApplication1.Middleware
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        // ✅ Only singleton services in constructor
        public RefreshTokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ✅ Resolve scoped services here
            var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
            var userService = context.RequestServices.GetRequiredService<IUserRepository>();

            // Skip token validation for public endpoints
            if (IsPublicEndpoint(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Try to get tokens from cookies or Authorization header
            var accessToken = GetAccessTokenFromRequest(context);
            var refreshToken = GetRefreshTokenFromRequest(context);

            // Check if access token is valid
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Check if token is blacklisted
                //if (await tokenService.IsTokenBlacklistedAsync(accessToken))
                //{
                //    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //    await context.Response.WriteAsJsonAsync(new { error = "Token has been revoked" });
                //    return;
                //}

                var principal = ValidateAccessToken(accessToken);
                if (principal != null)
                {
                    context.User = principal;
                    await _next(context);
                    return;
                }
            }

            // Access token is invalid/missing, try to refresh using refresh token
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // ✅ Use resolved tokenService here
                var user = await userService.GetUserByRefreshTokenAsync();
                if (user is not null)
                {
                    // Generate new access token
                    var newAccessToken = await tokenService.GenerateJwtTokenAsync(user);

                    // Set new access token in response header
                    context.Response.Headers["Authorization"] = $"Bearer {newAccessToken}";

                    // Optional: Set in cookie as well
                    SetAccessTokenInCookie(context, newAccessToken);

                    // Create principal from new token
                    var principal = ValidateAccessToken(newAccessToken);
                    context.User = principal;

                    await _next(context);
                    return;
                }

                // Invalid refresh token
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or expired refresh token" });
                return;
            }

            // No valid tokens found
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Authentication required" });
        }

        private bool IsPublicEndpoint(PathString path)
        {
            var publicEndpoints = new[]
            {
                "/auth/login",
                "/auth/register",
                "/auth/refresh-token",
                "/swagger",
                "/health",
                "/error"
            };

            return publicEndpoints.Any(endpoint =>
                path.StartsWithSegments(endpoint, StringComparison.OrdinalIgnoreCase));
        }

        private string GetAccessTokenFromRequest(HttpContext context)
        {
            // Check Authorization header first
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            // Check cookies
            return context.Request.Cookies["access_token"];
        }

        private string GetRefreshTokenFromRequest(HttpContext context)
        {
            // Check custom header
            var refreshTokenHeader = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(refreshTokenHeader))
            {
                return refreshTokenHeader;
            }

            // Check cookies
            return context.Request.Cookies["refresh_token"];
        }

        private ClaimsPrincipal ValidateAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private void SetAccessTokenInCookie(HttpContext context, string accessToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(15) // Short-lived
            };

            context.Response.Cookies.Append("access_token", accessToken, cookieOptions);
        }
    }
}
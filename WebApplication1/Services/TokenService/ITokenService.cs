using System.Security.Claims;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        Task<AuthTokensDTO> GenerateAuthResponseAsync(ApplicationUser user);
        Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken);
        Task RevokeRefreshTokenAsync(ApplicationUser user);
        Task RevokeAccessTokenAsync(string accessToken);
        string GetAccessTokenFromRequest(HttpContext context);
        string GetRefreshTokenFromRequest(HttpContext context);
        ClaimsPrincipal ValidateAccessToken(string accessToken);
        void SetRefreshTokenInResponse(HttpContext context, string refreshToken);
        //Task BlacklistTokenAsync(string token, DateTime expiry);
        //Task<bool> IsTokenBlacklistedAsync(string token);
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.TokenService.Handlers
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        // ---------- JWT ----------

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,   user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Name,  user.FullName),
                new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new("fullName", user.FullName),
                new("status",   user.Status.ToString())
            };

            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                Issuer             = _configuration["Jwt:Issuer"],
                Audience           = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // ---------- Refresh token ----------

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<AuthTokensDTO> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var accessToken  = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            // Persist refresh token on the user record
            user.RefreshToken            = refreshToken;
            user.RefreshTokenExpiryTime  = DateTime.UtcNow.AddDays(RefreshTokenDays);

            await _userManager.UpdateAsync(user);

            return new AuthTokensDTO
            {
                AccessToken          = accessToken,
                RefreshToken         = refreshToken,
                AccessTokenExpires   = DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                RefreshTokenExpires  = user.RefreshTokenExpiryTime.Value
            };
        }

        public Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            var isValid =
                !string.IsNullOrEmpty(user.RefreshToken) &&
                user.RefreshToken == refreshToken &&
                user.RefreshTokenExpiryTime > DateTime.UtcNow;

            return Task.FromResult(isValid);
        }

        public async Task<RefreshResult?> TryRefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        return null;

    // Look up the user holding this refresh token
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null)
                return null;

            if (user.RefreshTokenExpiryTime is null ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            // Rotate refresh token
            var newRefreshToken = GenerateRefreshToken();
            var newExpiry       = DateTime.UtcNow.AddDays(RefreshTokenDays);

            user.RefreshToken           = newRefreshToken;
            user.RefreshTokenExpiryTime = newExpiry;

            var updated = await _userManager.UpdateAsync(user);
            if (!updated.Succeeded)
                return null;

            // Issue a fresh access token
            var newAccessToken = await GenerateJwtTokenAsync(user);

            return new RefreshResult(user, newAccessToken, newRefreshToken, newExpiry);
        }

        public async Task RevokeRefreshTokenAsync(ApplicationUser user)
        {
            user.RefreshToken           = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
        }


        // ---------- Access token validation ----------

        public ClaimsPrincipal? ValidateAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ValidateIssuer           = true,
                    ValidIssuer              = _configuration["Jwt:Issuer"],
                    ValidateAudience         = true,
                    ValidAudience            = _configuration["Jwt:Audience"],
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };

                  return tokenHandler.ValidateToken(accessToken, validationParameters, out _);
            }
             catch (SecurityTokenExpiredException)
            {
                return null;   // expected — not worth logging
            }
            catch (Exception ex)
            {
                // _logger.LogWarning(ex, "Access token validation failed");
                return null;
            }
        }

        public Task RevokeAccessTokenAsync(string accessToken)
        {
            // TODO: wire up the distributed-cache blacklist when ready
            // var jwt   = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            // var expiry = jwt.ValidTo;
            // await BlacklistTokenAsync(accessToken, expiry);
            return Task.CompletedTask;
        }

        // ---------- Config helpers ----------

        private int    AccessTokenMinutes  => _configuration.GetValue<int?>("Jwt:TokenExpirationMinutes")        ?? 15;
        private int    RefreshTokenDays    => _configuration.GetValue<int?>("Jwt:RefreshTokenExpirationDays")    ?? 7;
    }
}
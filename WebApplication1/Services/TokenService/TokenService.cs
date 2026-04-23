using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
      //  private readonly IDistributedCache _cache;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IDistributedCache cache)
        {
            _configuration = configuration;
            _userManager = userManager;
          //  _cache = cache;
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("fullName", user.FullName),
                new Claim("status", user.Status.ToString())
            };

            // Add role claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:TokenExpirationMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<AuthTokensDTO> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"]));

            await _userManager.UpdateAsync(user);

            //var roles = await _userManager.GetRolesAsync(user);


            return new AuthTokensDTO
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                AccessTokenExpires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:TokenExpirationMinutes"])),
                RefreshTokenExpires = user.RefreshTokenExpiryTime.Value,
            };
        }

        public async Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            if (user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return false;
            }
            return true;
        }

        public async Task RevokeRefreshTokenAsync(ApplicationUser user)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
        }

        public string GetAccessTokenFromRequest(HttpContext context)
        {
            // Check Authorization header first
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            // Check cookies
            //return context.Request.Cookies["AccessToken"];
            return string.Empty;
        }

        public string GetRefreshTokenFromRequest(HttpContext context)
        {
            // Check custom header
            var refreshTokenHeader = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(refreshTokenHeader))
            {
                return refreshTokenHeader;
            }

            // Check cookies
            return context.Request.Cookies["refresh-token"];
        }

        public ClaimsPrincipal ValidateAccessToken(string accessToken)
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

        public void SetRefreshTokenInResponse(HttpContext context, string refreshToken)
        {
            // Set in cookies
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(
                Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"]))
            };

            context.Response.Cookies.Append("refresh-token", refreshToken, cookieOptions);

            // Also set in response headers for mobile clients
            context.Response.Headers["X-Refresh-Token"] = refreshToken;
        }

        public async Task RevokeAccessTokenAsync(string accessToken)
        {
            // Blacklist the current access token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);
            var expiry = jwtToken.ValidTo;

          //  await BlacklistTokenAsync(accessToken, expiry);
        }

        //public async Task BlacklistTokenAsync(string token, DateTime expiry)
        //{
        //    var tokenHash = ComputeSha256Hash(token);
        //    var timeUntilExpiry = expiry - DateTime.UtcNow;

        //    if (timeUntilExpiry > TimeSpan.Zero)
        //    {
        //        await _cache.SetStringAsync(
        //            $"blacklisted_token:{tokenHash}",
        //            "revoked",
        //            new DistributedCacheEntryOptions
        //            {
        //                AbsoluteExpiration = expiry
        //            }
        //        );
        //    }
        //}

        //public async Task<bool> IsTokenBlacklistedAsync(string token)
        //{
        //    var tokenHash = ComputeSha256Hash(token);
        //    return await _cache.GetStringAsync($"blacklisted_token:{tokenHash}") != null;
        //}

        //private string ComputeSha256Hash(string rawData)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        //        return Convert.ToBase64String(bytes);
        //    }
        //}
    }
}
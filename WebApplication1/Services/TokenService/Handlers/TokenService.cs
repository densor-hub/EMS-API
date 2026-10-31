// WebApplication1/Services/TokenService/Handlers/TokenService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.TokenService.Handlers
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IOptions<JwtOptions> options,
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            ILogger<TokenService> logger)
        {
            _options = options.Value;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // ---------- Access token ----------

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key     = Encoding.UTF8.GetBytes(_options.Secret);

            var roles  = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,   user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Name,  user.FullName),
                new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new("fullName", user.FullName),
                new("status",   user.Status.ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var descriptor = new SecurityTokenDescriptor
            {
                Subject            = new ClaimsIdentity(claims),
                Expires            = DateTime.UtcNow.AddMinutes(_options.TokenExpirationMinutes),
                Issuer             = _options.Issuer,
                Audience           = _options.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return handler.WriteToken(handler.CreateToken(descriptor));
        }

        // ---------- Refresh token ----------

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<AuthTokensDTO> GenerateAuthResponseAsync(
            ApplicationUser user, string? deviceInfo = null, string? ip = null)
        {
            var accessToken  = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            var now       = DateTime.UtcNow;
            var expiresAt = now.AddDays(_options.RefreshTokenExpirationDays);

            if (_options.RefreshTokenAbsoluteExpirationDays is int absDays && absDays > 0)
                expiresAt = expiresAt < now.AddDays(absDays) ? expiresAt : now.AddDays(absDays);

            _context.RefreshTokens.Add(
                RefreshToken.Create(refreshToken, user.Id, expiresAt, deviceInfo, ip));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to persist refresh token for user {UserId}", user.Id);
                throw;
            }

            return new AuthTokensDTO
            {
                AccessToken         = accessToken,
                RefreshToken        = refreshToken,
                AccessTokenExpires  = now.AddMinutes(_options.TokenExpirationMinutes),
                RefreshTokenExpires = expiresAt
            };
        }

        public async Task<RefreshResult?> TryRefreshAsync(
            string refreshToken, string? deviceInfo = null, string? ip = null)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return null;

            var stored = await _context.RefreshTokens
                .AsNoTracking()
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (stored is null)
            {
                _logger.LogWarning("Refresh token not found (prefix {Prefix})",
                    refreshToken[..Math.Min(6, refreshToken.Length)]);
                return null;
            }

            if (!stored.IsActive)
            {
                // Reuse-detection opportunity: someone is replaying a revoked/expired token.
                _logger.LogWarning("Refresh token inactive for user {UserId} (revoked={Revoked}, expired={Expired})",
                    stored.UserId, stored.RevokedAt != null, stored.ExpiresAt <= DateTime.UtcNow);
                return null;
            }

            if (stored.User is null || !stored.User.Status)
            {
                _logger.LogWarning("User missing/inactive for refresh token {UserId}", stored.UserId);
                return null;
            }

            // ── Atomic rotate ────────────────────────────────────────
            var now          = DateTime.UtcNow;
            var newToken     = GenerateRefreshToken();
            var newExpiresAt = now.AddDays(_options.RefreshTokenExpirationDays);

            if (_options.RefreshTokenAbsoluteExpirationDays is int absDays && absDays > 0)
                newExpiresAt = newExpiresAt < now.AddDays(absDays) ? newExpiresAt : now.AddDays(absDays);

            var rotated = await _context.RefreshTokens
                .Where(t => t.Token == refreshToken && t.RevokedAt == null && t.ExpiresAt > now)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));

            if (rotated == 0)
            {
                _logger.LogWarning("Refresh token rotation lost the race for user {UserId}", stored.UserId);
                return null;
            }

            _context.RefreshTokens.Add(
                RefreshToken.Create(newToken, stored.UserId, newExpiresAt, deviceInfo, ip));

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to persist rotated refresh token for user {UserId}", stored.UserId);
                return null;
            }

            var newAccessToken = await GenerateJwtTokenAsync(stored.User);

            return new RefreshResult(stored.User, newAccessToken, newToken, newExpiresAt);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return;
            var now = DateTime.UtcNow;
            await _context.RefreshTokens
                .Where(t => t.Token == refreshToken && t.RevokedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));
        }

        public async Task RevokeAllRefreshTokensAsync(string userId)
        {
            var now = DateTime.UtcNow;
            await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.RevokedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now));
        }

        // ---------- Access-token validation ----------

        public ClaimsPrincipal? ValidateAccessToken(string accessToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key     = Encoding.UTF8.GetBytes(_options.Secret);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ValidateIssuer           = true,
                    ValidIssuer              = _options.Issuer,
                    ValidateAudience         = true,
                    ValidAudience            = _options.Audience,
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.FromSeconds(_options.ClockSkewSeconds)
                };

                return handler.ValidateToken(accessToken, parameters, out _);
            }
            catch (SecurityTokenExpiredException) { return null; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Access token validation failed");
                return null;
            }
        }

        public Task RevokeAccessTokenAsync(string accessToken)
        {
            // TODO: distributed-cache blacklist keyed by JTI, expiring at ValidTo.
            return Task.CompletedTask;
        }
    }
}
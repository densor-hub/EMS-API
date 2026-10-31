// WebApplication1/DAL/Repository/UserRepository.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.Services.TokenService;

namespace WebApplication1.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IOptions<JwtOptions> jwtOptions,
            ILogger<UserRepository> logger)
        {
            _userManager    = userManager;
            _contextAccessor = contextAccessor;
            _jwtOptions     = jwtOptions.Value;
            _logger         = logger;
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync()
        {
            var ctx    = _contextAccessor.HttpContext;
            var header = ctx?.Request.Headers[_jwtOptions.RefreshHeaderName].FirstOrDefault();
            var cookie = ctx?.Request.Cookies[_jwtOptions.RefreshCookie.Name];

            var candidates = new[] { header, cookie }
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t!.Trim())
                .Distinct()
                .ToList();

            _logger.LogInformation(
                "[UserRepo] Refresh candidates → header={HasHeader}, cookie={HasCookie}, distinct={Count}",
                !string.IsNullOrEmpty(header), !string.IsNullOrEmpty(cookie), candidates.Count);

            if (candidates.Count == 0)
            {
                _logger.LogWarning("[UserRepo] No refresh token supplied (header/cookie both empty)");
                return null;
            }

            var now = DateTime.UtcNow;
            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t =>
                        candidates.Contains(t.Token) &&
                        t.RevokedAt == null &&
                        t.ExpiresAt > now));

            if (user is null)
                _logger.LogWarning("[UserRepo] No active refresh token matched ({Count} candidates)", candidates.Count);

            return user;
        }

        public async Task<Guid> GetCurrentUserId()
        {
            var id = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? _contextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
            return Guid.TryParse(id, out var g) ? g : Guid.Empty;
        }

        public Task<ApplicationUser?> GetUserByEmailAsync(string email)
            => _userManager.FindByEmailAsync(email);

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
            }
        }

        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync()
        {
            var refreshTokenHeader = _contextAccessor.HttpContext?.Request.Headers["X-Refresh-Token"].FirstOrDefault();
            var refreshTokenCookie = _contextAccessor.HttpContext?.Request.Cookies["refresh_token"];

            _logger.LogInformation(
                "[UserRepo] Header token present: {HasHeader}, Cookie token present: {HasCookie}",
                !string.IsNullOrEmpty(refreshTokenHeader),
                !string.IsNullOrEmpty(refreshTokenCookie)
            );

            // Cookie takes priority — it's the freshest value set by the server.
            var refreshToken = !string.IsNullOrEmpty(refreshTokenCookie)
                ? refreshTokenCookie
                : refreshTokenHeader;

            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("[UserRepo] No refresh token supplied by client");
                return null;
            }

            var prefix = refreshToken.Substring(0, Math.Min(6, refreshToken.Length));
            _logger.LogInformation("[UserRepo] Using token prefix: {Prefix}...", prefix);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                _logger.LogWarning("[UserRepo] No user matched token prefix {Prefix}", prefix);

                var tokensInDb = await _userManager.Users
                    .Where(u => u.RefreshToken != null && u.RefreshToken != "")
                    .Select(u => new { u.Email, Prefix = u.RefreshToken!.Substring(0, 6) })
                    .ToListAsync();
                _logger.LogInformation("[UserRepo] Tokens in DB: {@Tokens}", tokensInDb);
            }

            return user;
        }

        public async Task<Guid> GetCurrentUserId()
        {
            var userIdClaim = _contextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                _contextAccessor.HttpContext?.User?
                .FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        public Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                );
            }
        }
    }
}
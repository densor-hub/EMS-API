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

        public UserRepository(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;

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
            var refreshToken = "";
            // Check custom header
            var refreshTokenHeader = _contextAccessor.HttpContext?.Request.Headers["X-Refresh-Token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(refreshTokenHeader))
            {
                refreshToken = refreshTokenHeader;
            } else
            {
                refreshToken = _contextAccessor.HttpContext?.Request.Cookies["refresh_token"];

            }
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            return await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task <Guid> GetCurrentUserId()
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
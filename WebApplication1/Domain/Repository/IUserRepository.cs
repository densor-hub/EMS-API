using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByRefreshTokenAsync();
        Task UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(ApplicationUser user);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<Guid> GetCurrentUserId();
    }
}

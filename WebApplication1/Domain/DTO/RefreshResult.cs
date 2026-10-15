using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.DTO
{
    public record RefreshResult(
        ApplicationUser User,
        string NewAccessToken,
        string NewRefreshToken,
        DateTime NewRefreshTokenExpires);
}
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.DTO
{
    public record RefreshResult(
        ApplicationUser User,
        string AccessToken,
        string RefreshToken,
        DateTime RefreshTokenExpires);
}
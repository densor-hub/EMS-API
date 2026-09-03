// Services/IAuthService.cs
using WebApplication1.Domain.DTO;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO request);
        Task<AuthResponseDTO> LoginAsync(LoginRequest request);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId, string accessToken);
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<object> GetProfileAsync(string userId);
        Task DeleteUserAsync(string userId, string currentUserId);
        Task<bool> ConfirmAccount(string token, string email);
        Task<object> ForgotPassword(ForgotPasswordRequest request);
        Task SetPassword(SetPasswordRequest request);
    }
}
using System.Transactions;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.TokenService
{
    public interface IPaymentTokenService
    {
        Task<ApprovalTokenResult> GenerateTokenAsync(string userEmail, Payment payment);
        Task<TokenValidationResult> ValidateAndUseTokenAsync(string token, string ipAddress, string userAgent);
    }
}

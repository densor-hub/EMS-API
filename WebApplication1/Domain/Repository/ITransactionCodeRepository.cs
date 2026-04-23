
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionCodeRepository
    {
        Task<string> GenerateUniquePinAsync();
        Task<bool> CheckPinExistsAndValidAsync(string pin);
    }
}

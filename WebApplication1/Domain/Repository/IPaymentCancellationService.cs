// Services/Interfaces/IPaymentCancellationService.cs
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface IPaymentCancellationService
    {
        Task<PaymentCancellationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentCancellationDto>> GetAllAsync();
        Task<PaymentCancellationDto> CreateAsync(CreatePaymentCancellationDto createDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface IPaymentCancellationRepository
    {
        Task<PaymentCancellationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentCancellationDto>> GetAllAsync();
        Task<PaymentCancellationDto> CreateAsync(CreatePaymentCancellationDto createDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}

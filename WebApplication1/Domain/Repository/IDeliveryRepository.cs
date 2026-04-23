// Services/Interfaces/IDeliveryService.cs
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface IDeliveryRepository
    {
        Task<DeliveryDto> GetByIdAsync(Guid id);
        Task<IEnumerable<DeliveryDto>> GetAllAsync();
        Task<IEnumerable<DeliveryDto>> GetByTransactionIdAsync(Guid transactionId);
        Task<DeliveryDto> CreateAsync(CreateDeliveryDto createDto, Guid userId);
        Task<DeliveryDto> UpdateAsync(Guid id, UpdateDeliveryDto updateDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
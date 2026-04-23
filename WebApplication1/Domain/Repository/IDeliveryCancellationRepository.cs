// Services/Interfaces/IDeliveryCancellationService.cs
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface IDeliveryCancellationRepository
    {
        Task<DeliveryCancellationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<DeliveryCancellationDto>> GetAllAsync();
        Task<DeliveryCancellationDto> CreateAsync(CreateDeliveryCancellationDto createDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
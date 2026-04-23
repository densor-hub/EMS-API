using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface IPurchaseCancellationRepository
    {
        Task<PurchaseCancellationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<PurchaseCancellationDto>> GetAllAsync();
        Task<PurchaseCancellationDto> CreateAsync(CreatePurchaseCancellationDto createDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}

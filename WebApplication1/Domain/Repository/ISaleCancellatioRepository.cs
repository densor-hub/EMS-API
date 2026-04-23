using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ISaleCancellatioRepository
    {
        Task<SaleCancellationDto> GetByIdAsync(Guid id);
        Task<IEnumerable<SaleCancellationDto>> GetAllAsync();
        Task<SaleCancellationDto> CreateAsync(CreateSaleCancellationDto createDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}

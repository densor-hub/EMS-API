using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ISupplierRepository
    {
        IQueryable<Supplier> GetAllAsync(Guid compnayId);
        Task<Supplier> GetByIdAsync(Guid id);
        Task<Supplier> CreateAsync(Supplier createDto);
        Task<Supplier> UpdateAsync(Supplier updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateCodeAsync(Guid locationId);
        IQueryable<Supplier> GetAllByLocationAsync(Guid locationId);
    }
}

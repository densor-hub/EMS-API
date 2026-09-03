using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(Guid id);
        IQueryable<Location> GetAll();
        Task<IEnumerable<Location>> GetAllAsync();
        Task<IEnumerable<Location>> GetByCompanyIdAsync(Guid companyId);
        Task<Location> CreateAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
        IQueryable<Location> ExistingLocations(List<Guid> locations);
    }
}
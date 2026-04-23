using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ISupplierLocationRepository
    {
         Task AddRangeAsync(List<SupplierLocation> SupplierLocation);
         IQueryable <SupplierLocation> GetAllByLocationId(Guid item);
         IQueryable<SupplierLocation> GetAllBySupplierId(Guid item);
        Task DeleteRangeAsync(List<SupplierLocation> SupplierLocation);
        Task ManageLocationAccess(List<Guid> locations, Guid supplierId, Guid userId);
        Task<bool> Exists(Guid supplierId, Guid locationId);
    }
}

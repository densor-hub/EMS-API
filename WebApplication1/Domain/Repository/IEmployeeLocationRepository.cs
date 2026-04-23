using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IEmployeeLocationRepository
    {
         Task AddRangeAsync(List<EmployeeLocation> employeeLocation);
         IQueryable <EmployeeLocation> GetAllByLocationId(Guid locationId);
         IQueryable<EmployeeLocation> GetAllByEmployeeId(Guid employeeId);
        Task DeleteRangeAsync(List<EmployeeLocation> employeeLocation);
        Task ManageLocationAccess(List<Guid> locations, Guid employeeId, Guid userId);
    }
}

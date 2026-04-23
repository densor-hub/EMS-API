using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ILocationManagementsRepository
    {
        //IQueryable<LocationManangement> GetAll();
        Task AddRangeAsync(List<LocationManangement> employeeLocation);
         IQueryable <LocationManangement> GetAllByLocationId(Guid locationId);
        IQueryable<LocationManangement> GetAllByManagerIdId(Guid managerId);
        Task DeleteRangeAsync(List<LocationManangement> employeeLocation);
    }
}

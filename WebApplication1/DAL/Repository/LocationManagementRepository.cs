using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class LocationManagementRepository : ILocationManagementsRepository
    {
        private readonly AppDbContext _context;
        
        public LocationManagementRepository(AppDbContext context)
        {
            _context = context;
            
        }
        public async Task AddRangeAsync(List<LocationManangement> LocationManangement)
        {
            await _context.LocationManangement.AddRangeAsync(LocationManangement);
             await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<LocationManangement> LocationManangement)
        {
            _context.LocationManangement.RemoveRange(LocationManangement);
            await _context.SaveChangesAsync();
        }

        public IQueryable<LocationManangement> GetAllByManagerIdId(Guid managerId)
        {
            return _context.LocationManangement.Where(x => x.ManagerId == managerId);
        }

        public IQueryable<LocationManangement> GetAllByLocationId(Guid locId)
        {
            return _context.LocationManangement.Where(x => x.LocationId == locId);
        }
    }
}

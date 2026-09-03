using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using System.Security.Claims;

namespace WebApplication1.DAL.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocationRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations
                .Include(l => l.Company)
                    //.ThenInclude(sm => sm.CreatedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Include(l => l.Company)
                .Where(l => l.Status)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Locations
                .Include(l => l.EmployeeLocations)
                .Where(l => l.CompanyId == companyId && l.Status)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Location> CreateAsync(Location location)
        {
            var currentUserId = GetCurrentUserId();

            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();

            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
         

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            return location;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
                return false;

            //// Check if location has active shop managements
            //if (location.EmployeeLocations != null || location.EmployeeDisbursements !=null )
            //{
            //    throw new InvalidOperationException("Cannot delete location with active shop managements");
            //}

            location.SoftDelete(GetCurrentUserId());

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Locations.AnyAsync(l => l.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _context.Locations.Where(l => l.Code == code);

            if (excludeId.HasValue)
                query = query.Where(l => l.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                _httpContextAccessor.HttpContext?.User?
                .FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        public IQueryable<Location> GetAll()
        {
            return _context.Locations;
        }

      
        public   IQueryable<Location> ExistingLocations (List<Guid> locations)
        {
            var queriableLocations = GetAll();

           // await _employeeLocationRepository.HasAccessToLocation()

            queriableLocations = queriableLocations.Where(x => locations.Contains(x.Id));

            return queriableLocations;
        }

        
    }
}
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using System.Security.Claims;
using WebApplication1.Helpers;

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
                .Include(l => l.LocationManangements)  // ✅ Fixed: Plural, includes all shop managements
                    .ThenInclude(sm => sm.Manager)  // ✅ Then include the manager of each shop management
                .Include(l => l.LocationManangements)    // ✅ Also include creator if needed
                    //.ThenInclude(sm => sm.CreatedByUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Include(l => l.Company)
                .Include(l => l.LocationManangements)
                    .ThenInclude(sm => sm.Manager)
                .Where(l => l.Status)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Locations
                .Include(l => l.LocationManangements)
                    .ThenInclude(sm => sm.Manager)
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
                .Include(l => l.LocationManangements)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
                return false;

            // Check if location has active shop managements
            if (location.LocationManangements != null && location.LocationManangements.Any(sm => sm.Status == true))
            {
                throw new InvalidOperationException("Cannot delete location with active shop managements");
            }

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



        public async Task<string> GenerateCodeAsync(Guid? companyId)
        {
            var locationName = _context.Locations.Where(x => x.Id == companyId).Select(x => x.Name).FirstOrDefault();
            var initials = StringExtensions.GetInitials(locationName);

            var totalCount = await _context.Locations.Where(x => x.CompanyId == companyId).CountAsync();

            var number = totalCount + 1;
            return $"LOC-{initials.ToUpper().Trim()}-{number:D3}";
        }

        public  IQueryable<Location> ValidateLocations (List<Guid> locations)
        {
            var queriableLocations = GetAll();

            queriableLocations = queriableLocations.Where(x => locations.Contains(x.Id));

            return queriableLocations;
        }
    }
}
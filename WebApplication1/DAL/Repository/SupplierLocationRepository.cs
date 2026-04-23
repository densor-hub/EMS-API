using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class SupplierLocationRepository : ISupplierLocationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILocationRepository _locationRepository;
        public SupplierLocationRepository(AppDbContext context, ILocationRepository locationRepository)
        {
            _context = context;
            _locationRepository = locationRepository;   
        }
        public async Task AddRangeAsync(List<SupplierLocation> supplierLocation)
        {
            await _context.SupplierLocations.AddRangeAsync(supplierLocation);
             await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<SupplierLocation> supplierLocation)
        {
            _context.SupplierLocations.RemoveRange(supplierLocation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(Guid supplierId, Guid locationId)
        {
            return await _context.SupplierLocations.AnyAsync(x => x.LocationId == locationId && x.SupplierId == supplierId && x.Status == true);
        }

        public IQueryable<SupplierLocation> GetAllByLocationId(Guid locId)
        {
            return _context.SupplierLocations.Where(x=> x.LocationId == locId);
        }

        public IQueryable<SupplierLocation> GetAllBySupplierId(Guid supplierId)
        {
            return _context.SupplierLocations.Where(x => x.SupplierId == supplierId);
        }

        public async Task ManageLocationAccess(List<Guid> locations, Guid supplierId, Guid userId)
        {
            if (supplierId == Guid.Empty)
                throw new ArgumentException("Supplier cannot be empty", nameof(supplierId));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));



            if (locations.Count > 0)
            {
                var validSubmittedLocations = _locationRepository.ValidateLocations(locations);
                var validLocationIds = new HashSet<Guid>(validSubmittedLocations.Select(x => x.Id));

                var currentSupliereLocations = _context.SupplierLocations.Where(x => x.SupplierId == supplierId);
                var currentLocationIds = new HashSet<Guid>(currentSupliereLocations.Select(x => x.LocationId));

                // Track if any changes were made
                bool hasChanges = false;


                // Update existing locations
                foreach (var location in currentSupliereLocations)
                {
                    bool shouldHaveAccess = validLocationIds.Contains(location.LocationId);

                    if (shouldHaveAccess && !location.Status)
                    {
                        location.ActivateAccess();
                        hasChanges = true;
                    }
                    else if (!shouldHaveAccess && location.Status == true)
                    {
                        location.RemoveAccess();
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    _context.SupplierLocations.UpdateRange(currentSupliereLocations);
                }

                // Add new locations
                var newLocations = validSubmittedLocations
                    .Where(x => !currentLocationIds.Contains(x.Id))
                    .Select(x => SupplierLocation.Create(
                        Guid.NewGuid(),
                        x.Id,
                        supplierId,
                        DateTime.UtcNow,
                        userId,
                        true));

                if (newLocations.Any())
                {
                    await _context.SupplierLocations.AddRangeAsync(newLocations);
                }

                // Only update if something actually changed
                if (hasChanges || newLocations.Any())
                {
                    await _context.SaveChangesAsync();
                }

            }
            else
            {
                var employeeLocations = _context.EmployeeLocations.Where(x => x.EmployeeId == supplierId);
                foreach (var location in employeeLocations)
                {
                    location.RemoveAccess();
                }
                _context.EmployeeLocations.UpdateRange(employeeLocations);
                await _context.SaveChangesAsync();


            }


        }
    }
}

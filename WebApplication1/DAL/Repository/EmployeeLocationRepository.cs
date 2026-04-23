using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class EmployeeLocationRepository : IEmployeeLocationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILocationRepository _locationRepository;
        public EmployeeLocationRepository(
            AppDbContext context,
            ILocationRepository locationRepository)
        {
            _context = context;
            _locationRepository = locationRepository;
        }
        public async Task AddRangeAsync(List<EmployeeLocation> EmployeeLocations)
        {
            await _context.EmployeeLocations.AddRangeAsync(EmployeeLocations);
             await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<EmployeeLocation> EmployeeLocations)
        {
             _context.EmployeeLocations.UpdateRange(EmployeeLocations);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<EmployeeLocation> EmployeeLocations)
        {
            _context.EmployeeLocations.RemoveRange(EmployeeLocations);
            await _context.SaveChangesAsync();
        }

        public IQueryable<EmployeeLocation> GetAllByEmployeeId(Guid employeeId)
        {
            return _context.EmployeeLocations.Where(x=> x.EmployeeId == employeeId);
        }

        public IQueryable<EmployeeLocation> GetAllByLocationId(Guid locId)
        {
            return _context.EmployeeLocations.Where(x => x.LocationId == locId);
        }

        public async Task ManageLocationAccess(List<Guid> locations, Guid employeeId, Guid userId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("EmployeeId cannot be empty", nameof(employeeId));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));


           
            if (locations.Count > 0)
            {
                var validSubmittedLocations = _locationRepository.ValidateLocations(locations);
                var validLocationIds = new HashSet<Guid>(validSubmittedLocations.Select(x => x.Id));

                var currentEmployeeLocations = _context.EmployeeLocations.Where(x=> x.EmployeeId == employeeId);
                var currentLocationIds = new HashSet<Guid>(currentEmployeeLocations.Select(x => x.LocationId));

                // Track if any changes were made
                bool hasChanges = false;


                // Update existing locations
                foreach (var location in currentEmployeeLocations)
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
                    _context.EmployeeLocations.UpdateRange(currentEmployeeLocations);
                }

                // Add new locations
                var newLocations = validSubmittedLocations
                    .Where(x => !currentLocationIds.Contains(x.Id))
                    .Select(x => EmployeeLocation.Create(
                        Guid.NewGuid(),
                        x.Id,
                        employeeId,
                        DateTime.UtcNow,
                        userId,
                        true));

                if (newLocations.Any())
                {
                    await _context.EmployeeLocations.AddRangeAsync(newLocations);
                }

                // Only update if something actually changed
                if (hasChanges || newLocations.Any())
                {
                    await _context.SaveChangesAsync();
                }

            }
            else {
                var employeeLocations = _context.EmployeeLocations.Where(x=> x.EmployeeId == employeeId);
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

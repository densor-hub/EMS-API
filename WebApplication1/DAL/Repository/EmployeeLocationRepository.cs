using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class EmployeeLocationRepository : IEmployeeLocationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        public EmployeeLocationRepository(
            AppDbContext context,
            ILocationRepository locationRepository,
            IUserRepository userRepository)
        {
            _context = context;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
        }
        public async Task AddRangeAsync(List<EmployeeLocation> EmployeeLocations)
        {
            await _context.EmployeeLocations.AddRangeAsync(EmployeeLocations);
        }

        public async Task UpdateRangeAsync(List<EmployeeLocation> EmployeeLocations)
        {
             _context.EmployeeLocations.UpdateRange(EmployeeLocations);
            await Task.CompletedTask;
        }

        public async Task DeleteRangeAsync(List<EmployeeLocation> EmployeeLocations)
        {
            _context.EmployeeLocations.RemoveRange(EmployeeLocations);
            await Task.CompletedTask;
        }

        public IQueryable<EmployeeLocation> GetAllByEmployeeId(Guid employeeId)
        {
            return _context.EmployeeLocations.Where(x=> x.EmployeeId == employeeId);
        }

        public IQueryable<EmployeeLocation> GetAllByLocationId(Guid locId)
        {
            return _context.EmployeeLocations
                .Include(x=> x.Employee)
                .Include(x=> x.Location)
                .Where(x => x.LocationId == locId);
        }

        public async Task ManageLocationAccess(List<LocationManagement> locations, Guid employeeId, Guid userId)
        {
            if (employeeId == Guid.Empty)
                throw new ArgumentException("EmployeeId cannot be empty", nameof(employeeId));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            var submittedLocations = locations.Select(x => x.LocationId).ToList();

           
            if (locations.Count > 0)
            {
                var validSubmittedLocations = _locationRepository.ExistingLocations(submittedLocations);
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
                    .Select(loc => EmployeeLocation.Create(
                        Guid.NewGuid(),
                        loc.Id,
                        employeeId,
                        DateTime.UtcNow,
                        userId,
                        true,
                        false
                        ));

                if (newLocations.Any())
                {
                    await _context.EmployeeLocations.AddRangeAsync(newLocations);
                }

                // Only update if something actually changed
                if (hasChanges || newLocations.Any())
                {
                }

            }
            else {
                var employeeLocations = _context.EmployeeLocations.Where(x=> x.EmployeeId == employeeId);
                foreach (var location in employeeLocations)
                {
                    location.RemoveAccess();
                }
                _context.EmployeeLocations.UpdateRange(employeeLocations);
            }

            await Task.CompletedTask;
        }

        public async Task AddAsync(EmployeeLocation employeeLocation)
        {
            await _context.AddAsync(employeeLocation);

        }

        public async Task UpdateAync(EmployeeLocation employeeLocation)
        {
             _context.Update(employeeLocation);
            await Task.CompletedTask;
        }

        public async Task<Location?> HasAccessToLocation(Guid locationId)
        {
            if (locationId == Guid.Empty) { throw new Exception("Access to shop not found"); }
            var user =await _userRepository.GetUserByRefreshTokenAsync();
            if(user == null) { throw new Exception("User not found"); }
            //throw new NotImplementedException();
            var adminUser = await _context.Users.Where(x => x.Id == user.Id && x.UserRight == UserRight.ADMIN).FirstOrDefaultAsync();

            if (adminUser is not null)
            {
                var companyLocations = _context.Companies
                    .Include(x => x.Locations)
                    .Include(x => x.Employees)
                        .ThenInclude(x => x.UserAccount)
                    .Where(x => x.Employees.Any(x => x.Id.ToString() == user.Id) && x.Locations.Any(x => x.Id == locationId));

                if (companyLocations == null) { throw new Exception("Access to shop not found"); }
            }
            else
            {
                var employeeLocations = await _context.EmployeeLocations.Where(x => x.EmployeeId.ToString() == user.Id && x.LocationId == locationId).FirstOrDefaultAsync();

                if (employeeLocations == null) { throw new Exception("Access to shop not found"); }
            }

            await Task.CompletedTask;

            return await _context.Locations.Where(x => x.Id == locationId).FirstOrDefaultAsync();
        }
    }
}

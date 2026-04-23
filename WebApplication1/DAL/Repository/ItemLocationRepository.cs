using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class ItemLocationRepository : IItemLocationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILocationRepository _locationRepository;
        public ItemLocationRepository(AppDbContext context, ILocationRepository locationRepository)
        {
            _context = context;
            _locationRepository = locationRepository;
        }
        public async Task AddRangeAsync(List<ItemLocation> itemLoactions)
        {
            await _context.ItemLocations.AddRangeAsync(itemLoactions);
             await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<ItemLocation> itemLoactions)
        {
            _context.ItemLocations.RemoveRange(itemLoactions);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ItemLocation> GetAllByItemId(Guid item)
        {
            return _context.ItemLocations.Where(x=> x.ItemId == item);
        }

        public IQueryable<ItemLocation> GetAllByLocationId(Guid locId)
        {
            return _context.ItemLocations.Where(x => x.LocationId == locId);
        }

        public async Task ManageLocationAccess(List<Guid> locations, Guid itemId, Guid userId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("Supplier cannot be empty", nameof(itemId));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));



            if (locations.Count > 0)
            {
                var validSubmittedLocations = _locationRepository.ValidateLocations(locations);
                var validLocationIds = new HashSet<Guid>(validSubmittedLocations.Select(x => x.Id));

                var currentItemLocations = _context.ItemLocations.Where(x => x.ItemId == itemId);
                var currentLocationIds = new HashSet<Guid>(currentItemLocations.Select(x => x.LocationId));

                // Track if any changes were made
                bool hasChanges = false;


                // Update existing locations
                foreach (var location in currentItemLocations)
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
                    _context.ItemLocations.UpdateRange(currentItemLocations);
                }

                // Add new locations
                var newLocations = validSubmittedLocations
                    .Where(x => !currentLocationIds.Contains(x.Id))
                    .Select(x => ItemLocation.Create(
                        Guid.NewGuid(),
                        x.Id,
                        itemId,
                        DateTime.UtcNow,
                        userId,
                        true));

                if (newLocations.Any())
                {
                    await _context.ItemLocations.AddRangeAsync(newLocations);
                }

                // Only update if something actually changed
                if (hasChanges || newLocations.Any())
                {
                    await _context.SaveChangesAsync();
                }

            }
            else
            {
                var employeeLocations = _context.ItemLocations.Where(x => x.ItemId == itemId);
                foreach (var location in employeeLocations)
                {
                    location.RemoveAccess();
                }
                _context.ItemLocations.UpdateRange(employeeLocations);
                await _context.SaveChangesAsync();


            }


        }
    }
}

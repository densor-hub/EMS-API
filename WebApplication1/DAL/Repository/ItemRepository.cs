using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using System.Security.Claims;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.DTO;
using WebApplication1.Helpers;

namespace WebApplication1.DAL.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public ItemRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<Item> GetByIdAsync(Guid id)
        {
            return await _context.Items
                 .Include(i => i.ItemLocations)
                    .ThenInclude(il => il.Location)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public  IQueryable<Item> GetAllAsync()
        {
            return _context.Items
                .Include(i => i.ItemLocations)
                    .ThenInclude(il => il.Location)
                .Where(i => i.Status) // Assuming you add Status property
                .OrderByDescending(i => i.Name);
        }

        public async Task<IEnumerable<Item>> GetByLocationIdAsync(Guid locationId)
        {
            return await _context.Items.Where(x => x.Status == true && x.ItemLocations.Any(x => x.LocationId == locationId))
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetByCategoryAsync(ItemsCategory category, Guid locationId)
        {
            return await _context.Items.Where(x => x.ItemLocations.Any(x=> x.LocationId == locationId))
                .Where(i => i.Category == category && i.Status)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public IQueryable<Item> GetLowStockItems( Guid locationId)
        {
            return _context.Items.Where(x => x.ItemLocations.Any(x => x.LocationId == locationId))
                .Include(i => i.ItemLocations)
                    .ThenInclude(il => il.Location)
                .Include(i => i.StockLevel)  // ✅ Singular, not plural
                .Where(i => (i.StockLevel.AvailableQuanity <= i.ReorderLevel) || (i.StockLevel.ActualQuantity <= i.ReorderLevel) && i.Status)
                .OrderBy(i => i.StockLevel.ActualQuantity);
        }

        public async Task<IEnumerable<Item>> GetLowStockItemsActualQtyAsync(Guid locationId)
        {
            return await _context.Items.Where(x => x.ItemLocations.Any(x => x.LocationId == locationId))
                .Include(i => i.ItemLocations)
                    .ThenInclude(il => il.Location)
                .Include(i => i.StockLevel)  // ✅ Singular, not plural
                .Where(i => i.StockLevel.ActualQuantity <= i.ReorderLevel && i.Status)
                .OrderBy(i => i.StockLevel.ActualQuantity)
                .ToListAsync();
        }

        public async Task<Item> CreateAsync(Item item)
        {

            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<Item> UpdateAsync(Item item)
        {
         
            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return false;

            // Soft delete
            var currentUserId =await _userRepository.GetCurrentUserId();
            item.SoftDelete(currentUserId);

            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Items.AnyAsync(i => i.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _context.Items.Where(i => i.Code == code);

            if (excludeId.HasValue)
                query = query.Where(i => i.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            //return await _context.Items
            //    .Include(i=> i.ItemLocations)
            //        .ThenInclude(il=> il.Location)
            //    .Where(i => i.Status)
            //    .Select(x=> x.Category.ToString())
            //    .Distinct()
            //    .OrderBy(c => c)
            //    .ToListAsync();
            List<string> categories = Enum.GetNames(typeof(ItemsCategory)).ToList();
            return categories;
        }


        public async Task<string> GenerateCodeAsync(Guid locationId)
        {
            var locationName = _context.Locations.Where(x => x.Id == locationId).Select(x => x.Name).FirstOrDefault();
            var initials = StringExtensions.GetInitials(locationName);

            var totalCount = await _context.Employees.Where(x => x.EmployeeLocations.Any(el => el.LocationId == locationId)).CountAsync();

            var number = totalCount + 1;
            return $"ITM-{initials.ToUpper().Trim()}-{number:D3}";
        }

        public async Task<bool> AllItemsAreValid(List<Guid> itemIds, Guid locationId)
        {

            var existingItems = await _context.Items
                .Where(i => itemIds.Contains(i.Id) && i.ItemLocations.Any(x => x.Id == locationId))
                .Select(i => i.Id)
                .ToListAsync();

            var missingItems = itemIds.Except(existingItems).ToList();
            return missingItems.Any();
        }


    }
}
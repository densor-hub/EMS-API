using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using System.Security.Claims;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.DTO;
using WebApplication1.Helpers;
using System.Linq;

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

        public async Task<Item?> GetByIdAsync(Guid id)
        {
            return await _context.Items
                .Include(x=> x.TransactionItems)
                .Include(x=> x.StockLockDownItems)
                    .ThenInclude(x=> x.Submissions)
                 .Include(i => i.ItemLocations)
                    .ThenInclude(il => il.Location)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public  IQueryable<Item> GetAllAsync(bool checkStatus = true)
        {
            return _context.Items
                .Include(x=> x.Company)
                .Include(x => x.StockLockDownItems)
                    .ThenInclude(x => x.Submissions)
                .Include(i => i.ItemLocations)
                    .ThenInclude(il => il.Location)
                .Where(i => checkStatus ?  i.Status : true) // Assuming you add Status property
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
            return _context.StockLevels
                .Include(x => x.Location)
                .Include(x => x.Item)
                .Where(x => x.LocationId == locationId && x.AvailableQuanity <= x.Item.ReorderLevel || x.ActualQuantity <= x.Item.ReorderLevel)
                .Select(x => x.Item);
                //.OrderBy(i => i.S);
        }

        public async Task<Item> CreateAsync(Item item)
        {

            await _context.Items.AddAsync(item);

            return item;
        }

        public async Task<Item> UpdateAsync(Item item)
        {
         
            _context.Items.Update(item);
            await Task.CompletedTask;
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


        public async Task<string> AllItemsAreValid(List<Guid> itemIds, Guid locationId)
        {
            if (itemIds == null || !itemIds.Any())
                return string.Empty;

            // Get the location name first
            var location = await _context.Locations
                .Where(l => l.Id == locationId)
                .Select(l => l.Name)
                .FirstOrDefaultAsync();

            var locationName = location ?? locationId.ToString();

            // Get all items with their locations
            var existingItems = await _context.Items
                .Include(x => x.ItemLocations)
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync();

            var existingItemsIds = existingItems.Select(x => x.Id).ToList();
            var missingItems = itemIds.Except(existingItemsIds).ToList();

            if (missingItems.Any())
                throw new Exception($"The following items do not exist in the system: {string.Join(", ", missingItems)}");

            // Find items that are NOT in the specified location
            var itemsNotInLocation = existingItems
                .Where(x => !x.ItemLocations.Any(l => l.LocationId == locationId))
                .Select(x => new { x.Id, x.Name })
                .ToList();

            if (itemsNotInLocation.Any())
            {
                var itemNames = string.Join(", ", itemsNotInLocation.Select(x => x.Name));
                return $"{locationName} does not have access to the following items : {itemNames}";
            }

            return "ALL-VALID";
        }

        public  async Task<StockLevel> GetItemStockLevel(Guid itemId, Guid locationId)
        {
            var item = await GetByIdAsync(itemId);
            
            if (item == null) item = await _context.TransactionItems
                    .Include(x => x.Item)
                    .Where(x => x.Id == itemId).Select(x=> x.Item).FirstOrDefaultAsync();

            if (item == null) throw new Exception("Item not found");

            var stockLevl = await _context.StockLevels
                    .Include(x=> x.Location)
                    .Include(x=> x.Item)
                    .Where(x => x.ItemId == item.Id && x.LocationId == locationId).FirstOrDefaultAsync();

            return stockLevl;
        }

        public async Task<bool> NameExist(string name, Guid ItemId)
        {
           return await _context.Items.AnyAsync(x=> x.Name.ToLower().Trim() == name.ToLower().Trim() && x.Id != ItemId);
        }
    }
} 
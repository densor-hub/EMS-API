using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Repository
{
    public interface IItemRepository
    {
        Task<Item> GetByIdAsync(Guid id);
        IQueryable<Item> GetAllAsync();
        Task<IEnumerable<Item>> GetByLocationIdAsync(Guid locationId);
        Task<IEnumerable<Item>> GetByCategoryAsync(ItemsCategory category, Guid locationId);
        IQueryable<Item> GetLowStockItems(Guid locationId);
        Task<IEnumerable<Item>> GetLowStockItemsActualQtyAsync( Guid locationId);
        Task<Item> CreateAsync(Item item);
        Task<Item> UpdateAsync(Item item);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
        Task<IEnumerable<string>> GetAllCategoriesAsync();
        Task<string> GenerateCodeAsync(Guid companyId);
        Task<bool> AllItemsAreValid(List<Guid> itemIds, Guid locationId);
    }
}
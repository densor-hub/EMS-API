using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IStockLockDownItemRepository
    {
        Task<StockLockDownItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<StockLockDownItem>> GetAllAsync();
        Task<StockLockDownItem> AddAsync(StockLockDownItem entity);
        Task<IEnumerable<StockLockDownItem>> AddRangeAsync(IEnumerable<StockLockDownItem> entities);
        Task<StockLockDownItem> UpdateAsync(StockLockDownItem entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        
    }
}

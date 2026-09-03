
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repositories
{
    public interface IStockLockDownRequestRepository
    {
        Task<StockLockDownRequest?> GetByIdAsync(Guid id);
        IQueryable<StockLockDownRequest> GetAllAsync(Guid? locationId, DateTime? startDate, DateTime? endDate);
        Task<StockLockDownRequest> AddAsync(StockLockDownRequest entity);
        Task<StockLockDownRequest> UpdateAsync(StockLockDownRequest entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> CountAsync();
    }
}
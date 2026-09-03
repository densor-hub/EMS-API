using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Repository
{
    public interface IStockTakeItemSubmissionRepository
    {
        Task<StockTakeItemSubmission?> GetByIdAsync(Guid id);
        IQueryable<StockTakeItemSubmission> GetAllAsync();
        Task<IEnumerable<StockTakeItemSubmission>> GetByStockLockDownItemIdAsync(Guid stockLockDownItemId);
        Task<StockTakeItemSubmission> AddAsync(StockTakeItemSubmission entity);
        Task<IEnumerable<StockTakeItemSubmission>> AddRangeAsync(IEnumerable<StockTakeItemSubmission> entities);
        Task<StockTakeItemSubmission> UpdateAsync(StockTakeItemSubmission entity);
        Task<bool> DeleteAsync(Guid id);
    }
}

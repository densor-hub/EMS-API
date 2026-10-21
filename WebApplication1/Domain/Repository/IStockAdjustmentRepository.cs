using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Repositories
{
    public interface IStockAdjustmentRepository
    {
        IQueryable<StockAdjustment> GetAll();
        Task<StockAdjustment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<StockAdjustment?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(StockAdjustment entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<StockAdjustment> entities, CancellationToken ct = default);
        void Update(StockAdjustment entity);
        void Delete(StockAdjustment entity);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
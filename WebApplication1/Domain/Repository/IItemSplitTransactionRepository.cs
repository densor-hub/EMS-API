using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Repositories
{
    public interface IItemSplitTransactionRepository
    {
        IQueryable<ItemSplitTransaction> GetAll();
        Task<ItemSplitTransaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ItemSplitTransaction?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(ItemSplitTransaction entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<ItemSplitTransaction> entities, CancellationToken ct = default);
        void Update(ItemSplitTransaction entity);
        void Delete(ItemSplitTransaction entity);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
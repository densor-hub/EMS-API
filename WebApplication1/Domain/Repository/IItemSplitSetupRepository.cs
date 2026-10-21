using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Repositories
{
    public interface IItemSplitSetupRepository
    {
        IQueryable<ItemSplitSetup> GetAll(Guid locationId);
        Task<ItemSplitSetup?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ItemSplitSetup?> GetByIdForUpdateAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(ItemSplitSetup entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<ItemSplitSetup> entities, CancellationToken ct = default);
        void Update(ItemSplitSetup entity);
        // void Delete(ItemSplitSetup entity);
        Task<ItemSplitSetup?> GetBySourceAndTargetAsync(
        Guid sourceItemId,
        Guid targetItemId,
        CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
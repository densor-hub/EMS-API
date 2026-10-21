using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Repositories
{
    public class ItemSplitTransactionRepository : IItemSplitTransactionRepository
    {
        private readonly AppDbContext _context;

        public ItemSplitTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        // Returns a composable queryable — callers add Include/Where/OrderBy.
        // No tracking; use GetByIdForUpdateAsync if you intend to mutate.
        public IQueryable<ItemSplitTransaction> GetAll()
        {
            return _context.ItemSplitTransactions
                .Include(x => x.ItemSplitSetup)
                .AsNoTracking();
        }

        // Read-only: no tracking, cheaper, safe for GET endpoints.
        public async Task<ItemSplitTransaction?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _context.ItemSplitTransactions
                .Include(x => x.ItemSplitSetup)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        // Tracked: use this in PUT/PATCH handlers so SaveChanges writes
        // only the columns that actually changed.
        public async Task<ItemSplitTransaction?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _context.ItemSplitTransactions
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        // Cheaper than GetByIdAsync when you only need existence
        // (e.g. DELETE handler validation).
        public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return _context.ItemSplitTransactions.AnyAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(
            ItemSplitTransaction entity,
            CancellationToken ct = default)
        {
            await _context.ItemSplitTransactions.AddAsync(entity, ct);
        }

        public async Task AddRangeAsync(
            IEnumerable<ItemSplitTransaction> entities,
            CancellationToken ct = default)
        {
            await _context.ItemSplitTransactions.AddRangeAsync(entities, ct);
        }

        // Marks entity Modified. Prefer GetByIdForUpdateAsync + domain
        // Update() on the tracked entity instead — it writes only the
        // columns that changed and is concurrency-safe.
        public void Update(ItemSplitTransaction entity)
        {
            _context.ItemSplitTransactions.Update(entity);
        }

        public void Delete(ItemSplitTransaction entity)
        {
            _context.ItemSplitTransactions.Remove(entity);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.DAL.Repositories
{
    public class ItemSplitSetupRepository : IItemSplitSetupRepository
    {
        private readonly AppDbContext _context;

        public ItemSplitSetupRepository(AppDbContext context)
        {
            _context = context;
        }

        // Returns a composable queryable — callers add Include/Where/OrderBy.
        // No tracking; use GetByIdForUpdateAsync if you intend to mutate.
        public IQueryable<ItemSplitSetup> GetAll(Guid locationId)
        {
            return _context.ItemSplitSetups
                .Include(x => x.SourceItem)
                    .ThenInclude(x => x.ItemLocations)
                .Include(x => x.TargetItem)
                    .ThenInclude(x=> x.ItemLocations)
                .Where(x=> x.SourceItem.ItemLocations.Any(x=> x.LocationId == locationId) || x.TargetItem.ItemLocations.Any(x=> x.LocationId == locationId))
                .AsNoTracking()
                ;
        }

        // Read-only: no tracking, cheaper, safe for GET endpoints.
        public async Task<ItemSplitSetup?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _context.ItemSplitSetups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        // Tracked: use this in PUT/PATCH handlers so SaveChanges writes
        // only the columns that actually changed.
        public async Task<ItemSplitSetup?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _context.ItemSplitSetups
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        // Cheaper than GetByIdAsync when you only need existence
        // (e.g. DELETE handler validation).
        public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return _context.ItemSplitSetups.AnyAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(
            ItemSplitSetup entity,
            CancellationToken ct = default)
        {
            await _context.ItemSplitSetups.AddAsync(entity, ct);
        }

        public async Task AddRangeAsync(
            IEnumerable<ItemSplitSetup> entities,
            CancellationToken ct = default)
        {
            await _context.ItemSplitSetups.AddRangeAsync(entities, ct);
        }

        // Marks entity Modified. Prefer GetByIdForUpdateAsync + domain
        // Update() on the tracked entity instead — it writes only the
        // columns that changed and is concurrency-safe.
        public void Update(ItemSplitSetup entity)
        {
            _context.ItemSplitSetups.Update(entity);
        }

        //public void Delete(ItemSplitSetup entity)
        //{
        //    _context.ItemSplitSetups.Remove(entity);
        //}

        public async Task<ItemSplitSetup?> GetBySourceAndTargetAsync(
            Guid sourceItemId,
            Guid targetItemId,
            CancellationToken ct = default)
        {
            return await _context.ItemSplitSetups
                .Include(x=> x.SourceItem)
                .Include(x=> x.TargetItem)
                .FirstOrDefaultAsync(
                    x => x.SourceItemId == sourceItemId
                      && x.TargetItemId == targetItemId
                      && x.GeneralStatus == GeneralStatus.Active,
                    ct);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
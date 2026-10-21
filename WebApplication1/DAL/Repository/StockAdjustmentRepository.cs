using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.DAL.Repositories
{
    public class StockAdjustmentRepository : IStockAdjustmentRepository
    {
        private readonly AppDbContext _context;

        public StockAdjustmentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Composable queryable — callers add Where/OrderBy/projections.
        // Include navs so callers can project item/location/supplier names
        // without a second round-trip.
        public IQueryable<StockAdjustment> GetAll()
        {
            return _context.StockAdjustments
                .Include(x => x.Item)
                .Include(x => x.Location)
                .Include(x => x.Supplier)
                .AsNoTracking();
        }

        // Read-only: no tracking, cheap, safe for GET endpoints.
        public async Task<StockAdjustment?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _context.StockAdjustments
                .Include(x => x.Item)
                .Include(x => x.Location)
                .Include(x => x.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        // Tracked: use in PUT/PATCH handlers so SaveChanges writes
        // only the columns that actually changed.
        public async Task<StockAdjustment?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _context.StockAdjustments
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        // Cheaper than GetByIdAsync when you only need existence.
        public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return _context.StockAdjustments.AnyAsync(x => x.Id == id, ct);
        }

        public async Task AddAsync(
            StockAdjustment entity,
            CancellationToken ct = default)
        {
            await _context.StockAdjustments.AddAsync(entity, ct);
        }

        public async Task AddRangeAsync(
            IEnumerable<StockAdjustment> entities,
            CancellationToken ct = default)
        {
            await _context.StockAdjustments.AddRangeAsync(entities, ct);
        }

        public void Update(StockAdjustment entity)
        {
            _context.StockAdjustments.Update(entity);
        }

        public void Delete(StockAdjustment entity)
        {
            _context.StockAdjustments.Remove(entity);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.DAL.Repository
{
    public class StockLevelRepository :IStockLevelRepository
    {
        private readonly AppDbContext _context;

        public StockLevelRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<StockLevel?> GetByItemAndLocationAsync(Guid itemId, Guid locationId, CancellationToken ct = default)
        {
            return await _context.StockLevels
                .FirstOrDefaultAsync(
                    x => x.ItemId == itemId && x.LocationId == locationId,
                    ct);
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repositories;

namespace WebApplication1.DAL.Repository
{
    public class StockLockDownRequestRepository : IStockLockDownRequestRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<StockLockDownRequest> _dbSet;

        public StockLockDownRequestRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<StockLockDownRequest>();
        }

        public async Task<StockLockDownRequest?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Location)
                .Include(r => r.StockLockDownItems)
                    .ThenInclude(x=> x.Item)
                 .Include(r => r.StockLockDownItems)
                    .ThenInclude(x => x.Submissions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public  IQueryable<StockLockDownRequest> GetAllAsync(Guid? locationId, DateTime? startDate, DateTime? endDate)
        {

            var data = _dbSet
                .Include(r => r.Location)
                //.Include(r => r.StockLockDownItems)
                //    .ThenInclude(x=> x.Submissions)
                .Where(r => r.LocationId == locationId);

            if (locationId.HasValue && locationId != Guid.Empty)
            {
                data = data.Where(x=> x.LocationId == locationId);
            };

            if (startDate.HasValue && startDate != DateTime.MinValue)
            {
                var specifiedStartDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);

                data = data.Where(x => x.CreatedAt >= specifiedStartDate);
            }

            if (endDate.HasValue && endDate != DateTime.MinValue)
            {
                var specifiedEndateDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
                data = data.Where(x => x.CreatedAt <= specifiedEndateDate);
            }
          
            return data;

        }

        public async Task<IEnumerable<StockLockDownRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(r => r.Location)
                .Include(r => r.StockLockDownItems)
                .Where(r => r.TransactionDate >= startDate && r.TransactionDate <= endDate)
                .OrderByDescending(r => r.TransactionDate)
                .ToListAsync();
        }

        public async Task<StockLockDownRequest> AddAsync(StockLockDownRequest entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<StockLockDownRequest> UpdateAsync(StockLockDownRequest entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
            await Task.CompletedTask;
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(r => r.Id == id);
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}

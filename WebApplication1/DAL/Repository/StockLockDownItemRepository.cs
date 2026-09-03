using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class StockLockDownItemRepository : IStockLockDownItemRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<StockLockDownItem> _dbSet;

        public StockLockDownItemRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<StockLockDownItem>();
        }

        public async Task<StockLockDownItem?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(i => i.Item)
                .Include(i => i.StockLockDownRequest)
                .Include(x=> x.Submissions)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<StockLockDownItem>> GetAllAsync()
        {
            return await _dbSet
                .Include(i => i.Item)
                .Include(i => i.StockLockDownRequest)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

      
        public async Task<StockLockDownItem> AddAsync(StockLockDownItem entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<StockLockDownItem>> AddRangeAsync(IEnumerable<StockLockDownItem> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            if (!entityList.Any())
                return entityList;

            await _dbSet.AddRangeAsync(entityList);
            return entityList;
        }

        public async Task<StockLockDownItem> UpdateAsync(StockLockDownItem entity)
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
            return await _dbSet.AnyAsync(i => i.Id == id);
        }

      
    }
}

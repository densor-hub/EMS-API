using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class StockTakeItemSubmissionRepository : IStockTakeItemSubmissionRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<StockTakeItemSubmission> _dbSet;

        public StockTakeItemSubmissionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<StockTakeItemSubmission>();
        }

        public async Task<StockTakeItemSubmission?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.StockLockDownItem)
                    .ThenInclude(i => i.Item)
                .Include(s => s.StockLockDownItem)
                    .ThenInclude(i => i.StockLockDownRequest)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public  IQueryable <StockTakeItemSubmission> GetAllAsync()
        {
            return  _dbSet
                .Include(x=> x.CreatedByUser)
                .Include(s => s.StockLockDownItem)
                    .ThenInclude(i => i.Item)
                .Include(s => s.StockLockDownItem)
                    .ThenInclude(i => i.StockLockDownRequest);
        }

        public async Task<IEnumerable<StockTakeItemSubmission>> GetByStockLockDownItemIdAsync(Guid stockLockDownItemId)
        {
            return await _dbSet
                .Include(s => s.StockLockDownItem)
                    .ThenInclude(i => i.Item)
                .Where(s => s.StockLockDownItemId == stockLockDownItemId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<StockTakeItemSubmission> AddAsync(StockTakeItemSubmission entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<StockTakeItemSubmission>> AddRangeAsync(IEnumerable<StockTakeItemSubmission> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            if (!entityList.Any())
                return entityList;

            await _dbSet.AddRangeAsync(entityList);
            return entityList;
        }

        public async Task<StockTakeItemSubmission> UpdateAsync(StockTakeItemSubmission entity)
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
    }
}

// Repositories/CommentRepository.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Currency> _dbSet;

        public CurrencyRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Currency>();
        }

        public async Task<Currency?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Currency> GetAllAsync()
        {
            return _dbSet.AsNoTracking();
        }


        public async Task AddAsync(Currency comment)
        {
            await _dbSet.AddAsync(comment);
        }

        public async Task Update(Currency comment)
        {
            _dbSet.Update(comment);
            await Task.CompletedTask;
        }

        public void Delete(Currency  comment)
        {
            _dbSet.Remove(comment);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<Currency?> GetByCurrencyCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Code.ToUpper().Trim() == code.ToUpper().Trim());
        }
    }
}
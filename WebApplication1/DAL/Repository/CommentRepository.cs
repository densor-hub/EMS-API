// Repositories/CommentRepository.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TransactionComment> _dbSet;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TransactionComment>();
        }

        public async Task<TransactionComment?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<TransactionComment>> GetAllAsync()
        {
            return await _dbSet
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionComment>> GetByTransactionIdAsync(Guid transactionId)
        {
            return await _dbSet
                .Where(c => c.TransactionId == transactionId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionComment>> GetByTransactionTypeAsync(TransactionType transactionType)
        {
            return await _dbSet
                .Where(c => c.TransactionType.ToUpper().Trim() == transactionType.ToString().ToUpper().Trim())
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TransactionComment comment)
        {
            await _dbSet.AddAsync(comment);
        }

        public async Task Update(TransactionComment comment)
        {
            _dbSet.Update(comment);
            await Task.CompletedTask;
        }

        public void Delete(TransactionComment comment)
        {
            _dbSet.Remove(comment);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
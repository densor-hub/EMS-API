using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Transaction> _dbSet;
        public TransactionRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Transactions;
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            return transaction;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions
                .Include(x => x.TransactionPayments)
                .Include(x => x.TransactionItems)
                    .ThenInclude(x => x.TransactionItemsDelivered)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public IQueryable<Transaction> GetAllAsync(Guid? locationId, string transactionType, Guid? userId)
        {
            var query = _context.Transactions
                .Include(x => x.TransactionPayments)
                .Include(x => x.TransactionItems)
                    .ThenInclude(x => x.TransactionItemsDelivered)
                .AsNoTracking();

            // Only apply filters if values are provided
            if (locationId.HasValue && locationId != Guid.Empty)
                query = query.Where(x => x.LocationId == locationId);

            if (!string.IsNullOrEmpty(transactionType))
                query = query.Where(x => x.TransactionType == transactionType);

            if (userId.HasValue && userId != Guid.Empty)
                query = query.Where(x => x.CreatedBy == userId);

            return query;
        }


        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {   
             _context.Transactions.Update(transaction);
            await Task.CompletedTask;
            return transaction;
        }
    }
}

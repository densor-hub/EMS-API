using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class TransactionItemsDeliveredRepository : ITransactionItemsDeliveredRepository
    {
        private readonly AppDbContext _context;
        public TransactionItemsDeliveredRepository(AppDbContext context)
        {

            _context = context;

        }
        public async Task<Guid> AddAsync(TransactionItemDelivered transactionPayment)
        {
            await _context.TransactionItemsDelivered.AddAsync(transactionPayment);
            await _context.SaveChangesAsync();

            return transactionPayment.Id;
        }

        public async Task AddRangeAsync(List<TransactionItemDelivered> transactionPayment)
        {
            await _context.TransactionItemsDelivered.AddRangeAsync(transactionPayment);
            await _context.SaveChangesAsync();

        }


        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var item = await _context.TransactionItemsDelivered.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null) { return false; }

            item.SoftDelete(userId, DateTime.UtcNow);
            await _context.SaveChangesAsync();

            return true;
        }

        public IQueryable<TransactionItemDelivered> GetAllAsync()
        {
            return _context.TransactionItemsDelivered;
        }

        public async Task<TransactionItemDelivered> GetByIdAsync(Guid id)
        {
            var item = await _context.TransactionItemsDelivered
                .Include(x=> x.TransactionItem)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return item;
        }

        public IQueryable<TransactionItemDelivered> GetByTransactionIdAsync(Guid transactionId)
        {
            return _context.TransactionItemsDelivered.Where(x=> x.Id == transactionId);
        }
    }
}

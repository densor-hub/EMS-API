using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class TransactionPaymentRepository : ITransactionPaymentRepository
    {
        private readonly AppDbContext _context;
        public TransactionPaymentRepository(AppDbContext context)
        {

            _context = context;

        }
        public async Task<Guid> AddAsync(TransactionPayment transactionPayment)
        {
            await _context.AddAsync(transactionPayment);
            await _context.SaveChangesAsync();

            return transactionPayment.Id;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var item = await _context.TransactionPayments.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (item != null) { return false; }

             _context.TransactionPayments.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        public IQueryable<TransactionPayment> GetAllAsync()
        {
            return _context.TransactionPayments;
        }

        public async Task<TransactionPayment> GetByIdAsync(Guid id)
        {
            var item = await _context.TransactionPayments
                .Include(x=> x.Transaction)
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return item;
        }

        public IQueryable<TransactionPayment> GetByTransactionIdAsync(Guid transactionId)
        {
            return _context.TransactionPayments.Where(x=> x.Id == transactionId);
        }
    }
}

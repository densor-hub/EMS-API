using WebApplication1.Domain.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(Transaction tranaction);
        Task<Transaction?> GetByIdAsync(Guid id);
        IQueryable<Transaction> GetAllAsync(Guid? locationId, string transactionType, Guid? userId);
        Task<Transaction> UpdateAsync (Transaction transaction);
    }
}

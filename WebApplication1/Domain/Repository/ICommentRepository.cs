// Repositories/ICommentRepository.cs
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public interface ICommentRepository
    {
        Task<TransactionComment?> GetByIdAsync(Guid id);
        Task<IEnumerable<TransactionComment>> GetAllAsync();
        Task<IEnumerable<TransactionComment>> GetByTransactionIdAsync(Guid transactionId);
        Task<IEnumerable<TransactionComment>> GetByTransactionTypeAsync(TransactionType transactionType);
        Task AddAsync(TransactionComment comment);
        Task Update(TransactionComment comment);
        void Delete(TransactionComment comment);
        Task<int> SaveChangesAsync();
    }
}
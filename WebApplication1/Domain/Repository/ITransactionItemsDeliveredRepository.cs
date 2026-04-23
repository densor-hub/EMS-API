// Services/Interfaces/IPaymentService.cs
using WebApplication1.Domain.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionItemsDeliveredRepository
    {
        Task<TransactionItemDelivered> GetByIdAsync(Guid id);
        IQueryable<TransactionItemDelivered> GetAllAsync();
        IQueryable<TransactionItemDelivered> GetByTransactionIdAsync(Guid transactionId);
        Task<Guid> AddAsync(TransactionItemDelivered TransactionItemsDelivered);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task AddRangeAsync(List<TransactionItemDelivered> transactionPayment);
    }
}
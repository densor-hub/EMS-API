// Services/Interfaces/IPaymentService.cs
using WebApplication1.Domain.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionPaymentRepository
    {
        Task<TransactionPayment> GetByIdAsync(Guid id);
        IQueryable<TransactionPayment> GetAllAsync();
        IQueryable<TransactionPayment> GetByTransactionIdAsync(Guid transactionId);
        Task<Guid> AddAsync(TransactionPayment transactionPayment);
        Task<bool> DeleteAsync(Guid id, Guid userId);

    }
}
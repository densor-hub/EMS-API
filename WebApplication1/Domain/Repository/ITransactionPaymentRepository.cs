// Services/Interfaces/IPaymentService.cs
using WebApplication1.Domain.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionPaymentRepository
    {
        Task<Payment> GetByIdAsync(Guid id);
        IQueryable<Payment> GetAllAsync();
        IQueryable<Payment> GetByTransactionIdAsync(Guid transactionId);
        Task<Guid> AddAsync(Payment transactionPayment);
        Task<bool> DeleteAsync(Guid id, Guid userId);

    }
}
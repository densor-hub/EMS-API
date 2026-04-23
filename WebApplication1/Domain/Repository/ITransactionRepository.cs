
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ITransactionRepository
    {
        Task<Guid> AddPayment(TransactionPaymentsDto createDto, Guid userId);
        Task CompleteTransaction(Transaction tranaction, CreateTransactionDto createDto, Guid userId);
        Task<IEnumerable<TransactionItemsReceivedDto>> GetAllDeliveredItemsToDate(Guid transactionId);
        Task DeliverItems(ConfirmTransactionDeliveryDTO createDto, Guid userId);
    }
}

using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Services.Emails.TemplateService.Enitities;

namespace WebApplication1.Services.ControllerServices
{
    public interface ITransactionService
    {
        Task<GetTransactionDto> GetTransactionDetails(Guid transactionId);
        //Task<Transaction> CreateGeneralTransactionAsync(CreateTransactionDto createDto, Location location);
        Task<(Transaction transaction, ApplicationUser user, Payment payment)> AddPaymentExternalCallAsync(Transaction transaction, TransactionPaymentsDto createDto, TransactionResultsType? transResult);
        //Task<string> CreateSpecificTypeOfTransaction(Transaction transaction, ApplicationUser user, Guid? BusinessPartnerId);
        Task ProcessTransactionItemsAsync(Transaction tranaction, CreateTransactionDto createDto, Guid userId);
        Task<IEnumerable<TransactionItemsReceivedDto>> GetAllDeliveredItemsToDate(Guid transactionId);
        Task<string> DeliverItems(ConfirmTransactionDeliveryDTO createDto, ApplicationUser user, SaleTransDeliveryRequest? saleTransDeliveryRequest, CancellationToken cancellationToken = default);
        Task SaveTransactionEmailTemplate(Transaction transaction, ApplicationUser user, Payment payment, Guid? BatchId, EmailReceiver? emailReceiver);
        Task<TransactionCreatedReturnDataDto> CompleteTransationProcess(CreateTransactionDto createDto, TransactionResultsType? transactionResultsType);
        Task<TransactionCreatedReturnDataDto> DeliveryRequest(Guid TransactionId, DateTime DeliveryDate);
        Task CancelAsync(TransactionCancellationDto createDto);
        Task<IReadOnlyList<IdAndNameQtyDTO>> GetTransactionItemsByType(Transaction? transaction, Guid? BatchId);
    }
}

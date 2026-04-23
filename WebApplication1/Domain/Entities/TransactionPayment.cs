using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class TransactionPayment : BaseEntity
    {
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethods PaymentMethod { get; private set; } //Mobile_Money, Cash, Cheque, Bank_Transfer

        public TransactionPayment()
        {
            
        }

        private TransactionPayment(Guid id, Guid transactionId, DateTime payementDate, decimal amount, decimal balance, PaymentMethods paymentMethod, DateTime createdAt, Guid createdBy)
        {
            Id = id;
            TransactionId = transactionId;
            PaymentDate = payementDate;
            Amount = amount;
            Balance = balance;
            PaymentMethod = paymentMethod;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static TransactionPayment Create(Guid id, Guid transactionId, DateTime payementDate, decimal amount, decimal balance, PaymentMethods paymentMethod, DateTime createdAt, Guid createdBy)
        => new TransactionPayment(id, transactionId, payementDate, amount, balance, paymentMethod, createdAt, createdBy);

        public void SoftDelete(DateTime updatedAt, Guid updatedBy, string reason)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            CancellationReason = reason;
        }
    }
}

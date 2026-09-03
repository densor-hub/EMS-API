using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal Balance { get; private set; }
        public string PaymentTransactionNumber { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; } // Pending, Partial, Completed
        public PaymentMethods PaymentMethod { get; private set; } //Mobile_Money, Cash, Cheque, Bank_Transfer
        public Guid? ConfirmationTokenId { get; private set; }
        public PaymentConfirmationToken ConfirmationToken { get; private set; }
        public Guid? CouponId { get; private set; }
        public virtual Coupon Coupon { get; private set;}
        public Guid CurrencyId { get; private set; }
        public Currency Currency { get; private set; }

        public Payment()
        {
            
        }

        private Payment(Guid id, Guid transactionId, string paymentTransactionNumber, DateTime payementDate, decimal amount, decimal balance, PaymentMethods paymentMethod, DateTime createdAt, Guid createdBy, PaymentStatus paymentStatus, Guid currency, Guid? couponId ) //, Guid? disbursementId
        {
            Id = id;
            TransactionId = transactionId;
            PaymentDate = payementDate;
            Amount = amount;
            Balance = balance;
            PaymentMethod = paymentMethod;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            PaymentStatus= paymentStatus;
            CouponId = couponId;
            CurrencyId = currency;
            PaymentTransactionNumber =paymentTransactionNumber;
        }

        public static Payment Create(Guid id, Guid transactionId, string paymentTransactionNumber, DateTime payementDate, decimal amount, decimal balance, PaymentMethods paymentMethod, DateTime createdAt, Guid createdBy, PaymentStatus paymentStatus, Guid currency, Guid? couponId) //, Guid? disbursementId
        => new Payment(id, transactionId,paymentTransactionNumber, payementDate, amount, balance, paymentMethod, createdAt, createdBy, paymentStatus, currency, couponId); //disbursementId


        public void Refund(DateTime updatedAt, Guid updatedBy, string reason)
        {
            PaymentStatus = PaymentStatus.Refunded;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        

    }
}

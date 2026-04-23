using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public string TransactionCode { get; private set; }
        public TransactinType TransactionType { get; private set; }
        public string TransactionAction { get; private set; }
        public DateTime Date { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; } // Pending, Partial, Completed
        public string? Notes { get; private set; } = null;
        // Foreign keys
        public ICollection<TransactionItem> TransactionItems { get; private set; }
        public ICollection<TransactionPayment> TransactionPayments { get; private set; }
      //  public ICollection<TransactionDelivery> TransactionDeliveries { get; private set; }

        private Transaction()
        {
            
        }

        private Transaction(Guid id, string transactionCode,DateTime date, decimal total, decimal taxAmount, decimal discount,  PaymentStatus paymentStatus, Guid createdBy, DateTime createdAt, TransactinType transactionType, string transactionAction)
        {
            Id  = id;
            TransactionCode = transactionCode;
            Date = date;
            TotalAmount = total;
            TaxAmount = taxAmount;
            DiscountAmount = discount;
            PaymentStatus = paymentStatus;
            CreatedAt = createdAt;
            CreatedBy   = createdBy;
            TransactionType = transactionType;
            TransactionAction = transactionAction;
        }

        public static Transaction Create(Guid id, string transactionCode, DateTime date, decimal total, decimal taxAmount, decimal discount, PaymentStatus paymentStatus, Guid createdBy, DateTime createdAt, TransactinType transactionType, string transactionAction)
       => new Transaction(id, transactionCode, date, total, taxAmount, discount,  paymentStatus, createdBy, createdAt, transactionType, transactionAction);

        public void Update(DateTime date, decimal total, decimal taxAmount, decimal discount, PaymentStatus paymentStatus, Guid updatedBy, DateTime updatedAt)
        {
            Date = date;
            TotalAmount = total;
            TaxAmount = taxAmount;
            DiscountAmount = discount;
            PaymentStatus = paymentStatus;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void SoftDelete(DateTime updatedAt, Guid updatedBy)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            UpdatedAt = updatedAt;
            UpdatedBy= updatedBy;
        }
    }
}

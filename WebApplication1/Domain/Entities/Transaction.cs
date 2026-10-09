using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public string TransactionNumber { get; private set; }
        public TransactionResultsType? TransactionResultsType { get; private set; }
        public string TransactionType { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public Guid? CouponId { get; private set; }
        public virtual Coupon? Coupon { get; private set; }
        public bool RequiresExternalApproval { get; set; }
       // public string CurrencyCode { get; private set; }
        public string? Notes { get; private set; } = null;
        // Foreign keys
        public ICollection<TransactionItem> TransactionItems { get; private set; }
        public ICollection<Payment> TransactionPayments { get; private set; }
        public ICollection<TransactionComment> CommentsAndLog { get; private set; }
        

        private Transaction()
        {
            
        }
      
        private Transaction(Guid id, string transactionNumber,DateTime transactionDate, decimal total, decimal taxAmount, decimal discount, Guid createdBy, DateTime createdAt, TransactionResultsType? transactionResult, string transactionType, Guid locationId, bool requiresExternalApproval)
        {
            Id  = id;
            TransactionNumber = transactionNumber;
            TransactionDate = transactionDate;
            TotalAmount = total;
            TaxAmount = taxAmount;
            //DiscountAmount = discount;
          //  PaymentStatus = paymentStatus;
            CreatedAt = createdAt;
            CreatedBy   = createdBy;
            TransactionResultsType = transactionResult;
            TransactionType = transactionType;
            LocationId = locationId;
            RequiresExternalApproval = requiresExternalApproval;
        }

        public static Transaction Create(Guid id, string transactionNumber, DateTime transactionDate, decimal total, decimal taxAmount, decimal discount,  Guid createdBy, DateTime createdAt, TransactionResultsType? transactionResult, string transactionType, Guid locationId, bool requiresExternalApproval)
       => new Transaction(id, transactionNumber, transactionDate, total, taxAmount, discount, createdBy, createdAt, transactionResult, transactionType, locationId, requiresExternalApproval);

        public void SetCoupon(Guid couponId)
        {
            CouponId = couponId;
        }
        public void UpdateTotalAmountWithCalculatedValue(decimal calculatedAmount)
        {
            TotalAmount = calculatedAmount;
        }

        public void Update( decimal total, decimal taxAmount,   Guid updatedBy, DateTime updatedAt)
        {
           // Date = date;
            TotalAmount = total;
            TaxAmount = taxAmount;
           // DiscountAmount = discount;
           // PaymentStatus = paymentStatus;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void Cancel(DateTime updatedAt, Guid updatedBy)
        {
            GeneralStatus = GeneralStatus.Cancelled;
            UpdatedAt = updatedAt;
            UpdatedBy= updatedBy;
        }

        public void ApplyCoupon(Guid couponId)
        {
            CouponId = couponId;
        }
    }
}

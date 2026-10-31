using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockAdjustment
    {
        public Guid Id { get; private set; }

        public Guid TransactionId {get; private set;}
        public Transaction Transaction {get; private set;}

        public Guid? SupplierId { get; private set; }
        public virtual Supplier? Supplier { get; private set; }

        public string? ApprovedById { get; private set; }
        public ApplicationUser? ApprovedBy { get; private set; }
        public DateTime ApprovedAt { get; private set;}

        private StockAdjustment() { }

        private StockAdjustment(
            Guid id,
            Guid transactionId,
             Guid? supplierId
            )
        {
            Id = id;
            TransactionId = transactionId;
             SupplierId = supplierId;
        }

        public static StockAdjustment Create(
            Guid id,
            Guid transactionId,
            Guid? supplierId
            )
            => new(id, transactionId, supplierId);

        public void Guard()
        {
           if (Transaction.GeneralStatus == GeneralStatus.SoftDeleted)
                throw new InvalidOperationException("Cannot approve a soft-deleted adjustment.");

            if (ApprovedById is not null)
                throw new InvalidOperationException("Adjustment is already approved.");
 
        }
        public void Approve(string approvedById, DateTime approvedAt)
        {
            Guard();
            ApprovedById = approvedById;
            ApprovedAt = approvedAt;
            Transaction.GeneralStatus = GeneralStatus.Approved;
        }

        public void Decline(string rejectedById, DateTime rejectedAt)
        {
            Guard();
            ApprovedById = rejectedById;
            ApprovedAt = rejectedAt;
            Transaction.GeneralStatus = GeneralStatus.Declined;
        }

       

        public void SoftDelete()
        {
            Transaction.GeneralStatus = GeneralStatus.SoftDeleted;
        }
    }
}
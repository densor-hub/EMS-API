namespace WebApplication1.Domain.Entities
{
    public class TransactionItemReceived : BaseEntity
    {
        public Guid TransactionItemId { get; private set; }
        public TransactionItem TransactionItem { get; private set; }
        public int Quantity { get; private set; }
        public DateTime DateReceived { get; private set; }
        public Guid BatchId { get; private set; }
        public ICollection<TransactionItemReversal> TransactionItemReversals { get; private set; }

        private TransactionItemReceived()
        {
            
        }

        private TransactionItemReceived(Guid id, Guid transactionItemId, Guid batchId , int quantity, DateTime deliveryDate)
        {
            Id = id;
            TransactionItemId = transactionItemId;
             Quantity = quantity;
            DateReceived = deliveryDate;
            BatchId = batchId;
        }

        public static TransactionItemReceived Create(Guid id, Guid transactionItemId, Guid batchId , int quantity, DateTime deliveryDate)
        => new TransactionItemReceived( id, transactionItemId, batchId, quantity , deliveryDate);

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = Enums.GeneralStatus.SoftDeleted;
            UpdatedAt   = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

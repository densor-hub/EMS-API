namespace WebApplication1.Domain.Entities
{
    public class TransactionItemDelivered : BaseEntity
    {
        public Guid TransactionItemId { get; private set; }
        public TransactionItem TransactionItem { get; private set; }
        public Guid? SaleTransDeliveryRequestId { get; private set; }
        public Guid BatchId { get; private set; }
        public virtual SaleTransDeliveryRequest SaleTransDeliveryRequest { get; private set; }
        public int Quantity { get; private set; }
        public DateTime DeliveryDate { get; private set; }
        public bool IsDelivered { get; private set; }
        public ICollection<TransactionItemReversal> TransactionItemReversals { get; private set; }


        private TransactionItemDelivered()
        {
            
        }

        private TransactionItemDelivered(Guid id, Guid transactionItemId, Guid? saleTransDeliveryRequestId, Guid batchId, int quantity, DateTime deliveryDate, bool isDelivered)
        {
            Id = id;
            TransactionItemId = transactionItemId;
            Quantity = quantity;
            DeliveryDate = deliveryDate;
            SaleTransDeliveryRequestId = saleTransDeliveryRequestId;
            BatchId = batchId;
            IsDelivered = isDelivered;

        }

        public static TransactionItemDelivered Create(Guid id, Guid transactionItemId,  Guid? saleTransDeliveryRequestId, Guid batchId, int quantity, DateTime deliveryDate, bool isDelivered)
        => new TransactionItemDelivered ( id, transactionItemId, saleTransDeliveryRequestId, batchId, quantity , deliveryDate, isDelivered);

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = Enums.GeneralStatus.SoftDeleted;
            UpdatedAt   = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void Delivered (Guid updatedBy, DateTime deliveredAt)
        {
            UpdatedBy = updatedBy;
            DeliveryDate = deliveredAt;
            IsDelivered = true;
        }
    }
}

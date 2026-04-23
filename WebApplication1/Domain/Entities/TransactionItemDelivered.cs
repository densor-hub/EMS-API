namespace WebApplication1.Domain.Entities
{
    public class TransactionItemDelivered : BaseEntity
    {
        public Guid TransactionItemId { get; private set; }
        public TransactionItem TransactionItem { get; private set; }
        public int Quanity { get; private set; }
        public DateTime DeliveryDate { get; private set; }

        private TransactionItemDelivered()
        {
            
        }

        private TransactionItemDelivered(Guid id, Guid transactionItemId, int quantity, DateTime deliveryDate)
        {
            Id = id;
            TransactionItemId = transactionItemId;
             Quanity = quantity;
            DeliveryDate = deliveryDate;
        }

        public static TransactionItemDelivered Create(Guid id, Guid transactionItemId, int quantity, DateTime deliveryDate)
        => new TransactionItemDelivered ( id, transactionItemId, quantity , deliveryDate);

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = Enums.GeneralStatus.SoftDeleted;
            UpdatedAt   = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

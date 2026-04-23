namespace WebApplication1.Domain.Entities
{
    public class TransactionItem : BaseEntity
    {
        
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Total { get; private set; }

        // Foreign keys
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }
        public ICollection <TransactionItemDelivered> TransactionItemsDelivered { get; private set; }

        public TransactionItem()
        {
            
        }

        private TransactionItem(Guid id, Guid tranasactionId, Guid itemId, int quantity, decimal unitPrice, decimal total, Guid createdBy, DateTime createdAt)
        {
            Id=id;
            TransactionId=tranasactionId;
            ItemId=itemId;
            Quantity=quantity;
            UnitPrice=unitPrice;
            Total=total;
            CreatedBy=createdBy;
            CreatedAt=createdAt;

        }

        public static TransactionItem Create(Guid id, Guid tranasactionId, Guid itemId, int quantity, decimal unitPrice, decimal total, Guid createdBy, DateTime createdAt)
        => new TransactionItem(id, tranasactionId, itemId, quantity, unitPrice, total, createdBy, createdAt);

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = Enums.GeneralStatus.SoftDeleted;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
    }
}

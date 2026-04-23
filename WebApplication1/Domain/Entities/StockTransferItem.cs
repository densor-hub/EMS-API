namespace WebApplication1.Domain.Entities
{
    public class StockTransferItem : BaseEntity
    {
        public int RequestedQuantity { get; private set; }
        public int TransferedQuantity { get; private set; }
        //public int ReceivedQuantity { get; private set; }
        // int Variance { get; private set; }
        public Guid StockTransferId { get; private set; }
        public  StockTransfer StockTransfer { get; private set; }
        public Guid ItemId { get; private set; }
        public  Item Item { get; private set; }
        public ICollection<StockTransferItemsDelivered>  stockTransferItemsDelivered { get; private set; }

        private StockTransferItem()
        {
            
        }

        private StockTransferItem(Guid id, Guid itemId, Guid stockTransferId, int requestedQuantity, DateTime createdAt, Guid createdBy )
        {
            Id = id;
            ItemId = itemId;
            StockTransferId = stockTransferId;
            RequestedQuantity = requestedQuantity;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static StockTransferItem Create(Guid id, Guid itemId, Guid stockTransferId, int requestedQuantity, DateTime createdAt, Guid createdBy)
        => new StockTransferItem(id, itemId, stockTransferId, requestedQuantity, createdAt, createdBy);

        public void UpdateAproval(int transferedQuantity, DateTime updatedAt, Guid updatedBy)
        {
            TransferedQuantity = transferedQuantity;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void UpdateReceival(int receivedQuanity, DateTime updatedAt, Guid updatedBy)
        {
            //ReceivedQuantity = receivedQuanity;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void UpdateVariance(int variance, DateTime updatedAt, Guid updatedBy)
        {
           // Variance = variance;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

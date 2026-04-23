using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockTransferItemsDelivered : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid StockTransferItemId { get; private set; }
        public StockTransferItem StockTransferItem { get; private set; }
        public int Quanity { get; private set; }
        public DateTime DeliveryDate { get; private set; }

        public StockTransferItemsDelivered()
        {
            
        }
        private StockTransferItemsDelivered(Guid id, Guid stockTransferItemId, int quantity, DateTime deliveryDate)
        {
            Id = id;
            StockTransferItemId = stockTransferItemId;
            Quanity = quantity;
            DeliveryDate = deliveryDate;
        }

        public static StockTransferItemsDelivered Create(Guid id, Guid stockTransferItemId, int quantity, DateTime deliveryDate)
        => new StockTransferItemsDelivered(id, stockTransferItemId, quantity, deliveryDate);

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = Enums.GeneralStatus.SoftDeleted;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

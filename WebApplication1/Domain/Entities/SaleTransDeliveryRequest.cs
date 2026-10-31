namespace WebApplication1.Domain.Entities
{
    public class SaleTransDeliveryRequest
    {
        public Guid Id { get; private set; }
        public Guid SaleId { get; private set; }
        public Sale Sale { get; private set; }
        public bool IsDelivered { get; private set; } 
        public DateTime CreatedAt { get; private set; }
        public string CreatedById { get; private set; }
        public ApplicationUser CreatedByUser { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedById { get; private set; }
        public DateTime DeliveryDate { get; private set; }
        public ApplicationUser UpdatedByUser { get; private set; }
        public DateOnly DateOnly { get; private set; }// dateOnly is for the daily count validation
        public int IncrementalId { get; private set; }
        public Location? Location {get; private set;}
        public Guid? LocationId {get; private set;}
        //REQUEST ITEMS
       // public ICollection<SaleTransDeliveryRequestItem> SaleTransDeliveryRequestItems { get; private set; }

        //FOR EASY REPORTING and data retrieval
        public ICollection<TransactionItemDelivered> TransactionItemDelivered { get; private set; }

        private SaleTransDeliveryRequest()
        {
            
        }
        private SaleTransDeliveryRequest(Guid id, Guid saleId, bool delivered, DateTime createdAt, string createdBy, DateTime deliveryDate, Guid locationId)
        {
            Id = id;
            SaleId = saleId;
            CreatedAt = createdAt;
            CreatedById = createdBy;
            IsDelivered = delivered;
            DeliveryDate = deliveryDate;
            LocationId = locationId;
            DateOnly = DateOnly.FromDateTime(createdAt);
        }

        public static SaleTransDeliveryRequest Create(Guid id, Guid saleId, bool delivered, DateTime createdAt, string createdBy, DateTime deliveryDate, Guid locationId)
       => new SaleTransDeliveryRequest(id, saleId, delivered, createdAt, createdBy, deliveryDate, locationId);

        public void Delivered(string updatedByUserId)
        {
            IsDelivered = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedByUserId;
        }

         public void SetIncrementalId(int incrementalId)
        {
            IncrementalId = incrementalId;
        }
    }
}

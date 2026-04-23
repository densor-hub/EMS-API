using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Purchase : BaseEntity
    {
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public string TransactionCode { get; set; }
        public Guid LocationId { get; set; }
        public  Location Location { get; set; }
        public Guid SupplierId { get; set; }
        public  Supplier Supplier { get; set; }
        public  string PurcasedById { get; set; }
        public ApplicationUser PurcasedBy { get; set; }

        private Purchase()
        {
            
        }

        private Purchase(Guid id, Guid locationId, Guid supplierId, Guid purchasedBy, string transactionCode , Guid transactionId, DateTime createdAt, Guid createdBy)
        {
            Id = id;
            LocationId = locationId;
            SupplierId = supplierId;
            PurcasedById = purchasedBy.ToString();
            TransactionCode = transactionCode;
            TransactionId = transactionId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static Purchase Create(Guid id, Guid locationId, Guid supplierId, Guid purchasedBy, string transactionCode, Guid transactionId, DateTime createdAt, Guid createdBy)
        => new Purchase(id, locationId, supplierId, purchasedBy, transactionCode, transactionId, createdAt, createdBy);

        public void SoftDelete(string reason, Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            CancellationReason = reason;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

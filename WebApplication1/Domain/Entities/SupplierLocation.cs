namespace WebApplication1.Domain.Entities
{
    public class SupplierLocation : BaseEntity
    {
        public Guid LocationId { get; private set; }
        public virtual Location Location { get; private set; }
        public Guid SupplierId { get; private set; }
        public virtual Supplier Supplier { get; private set; }
        public bool Status { get; private set; }

        public SupplierLocation()
        {
            
        }
        public SupplierLocation(Guid id, Guid locationId, Guid supplierId, DateTime createdAt, Guid createdBy, bool status )
        {
            Id = id;
            SupplierId = supplierId;
            LocationId = locationId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Status = status;

        }

        public static SupplierLocation Create(Guid id, Guid locationId, Guid supplierId, DateTime createdAt, Guid createdBy, bool status)
            => new SupplierLocation(id, locationId, supplierId, createdAt, createdBy, status);


        public void RemoveAccess()
        {
            Status = false;
        }
        public void ActivateAccess()
        {
            Status = true;
        }
    }
}

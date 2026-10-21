using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockAdjustment
    {
        public Guid Id { get; private set; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; } = null!;

        public Guid LocationId { get; private set; }
        public Location Location { get; private set; } = null!;

        public int Quantity { get; private set; }

        public Guid? SupplierId { get; private set; }
        public virtual Supplier? Supplier { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime TransactionDate { get; private set; }

        public GeneralStatus GeneralStatus { get; private set; }
        public string CreatedById { get; private set; } = null!;
        public ApplicationUser CreatedBy { get; private set; } = null!;
        public string? ApprovedById { get; private set; }
        public ApplicationUser? ApprovedBy { get; private set; }
        public DateTime ApprovedAt { get; private set;}

        private StockAdjustment() { }

        private StockAdjustment(
            Guid id,
            Guid itemId,
            Guid locationId,
            int quantity,
            Guid? supplierId,
            DateTime createdAt,
            DateTime transactionDate,
            string createdById)
        {
            Id = id;
            ItemId = itemId;
            LocationId = locationId;
            Quantity = quantity;
            SupplierId = supplierId;
            CreatedAt = createdAt;
            TransactionDate = transactionDate;
            CreatedById = createdById;
            GeneralStatus = GeneralStatus.Active;
        }

        public static StockAdjustment Create(
            Guid id,
            Guid itemId,
            Guid locationId,
            int quantity,
            Guid? supplierId,
            DateTime createdAt,
            DateTime transactionDate,
            string createdById)
            => new(id, itemId, locationId, quantity, supplierId, createdAt, transactionDate, createdById);

        public void Update(
            int quantity,
            Guid? supplierId,
            DateTime date,
            GeneralStatus generalStatus)
        {
            if (GeneralStatus == GeneralStatus.SoftDeleted)
                throw new InvalidOperationException("Cannot update a soft-deleted adjustment.");

            Quantity = quantity;
            SupplierId = supplierId;
            ApprovedAt = date;
            GeneralStatus = generalStatus;
        }

        public void Verify(string approvedById, DateTime approvedAt, GeneralStatus responseStatus)
        {
            if (GeneralStatus == GeneralStatus.SoftDeleted)
                throw new InvalidOperationException("Cannot approve a soft-deleted adjustment.");

            if (ApprovedById is not null)
                throw new InvalidOperationException("Adjustment is already approved.");

            GeneralStatus = responseStatus;
            ApprovedById = approvedById;
            ApprovedBy = null!; // EF will populate on read
            // If you want an explicit ApprovedAt timestamp, add it to the entity.
        }

        public void SoftDelete()
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
        }
    }
}
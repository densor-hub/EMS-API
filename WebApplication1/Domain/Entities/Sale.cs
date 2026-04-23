using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public string TransactionNumber { get; private set; }
        public Guid LocationId { get; set; }
        public virtual Location Location { get; set; }
        public Guid? CustomerId { get; private set; }
        public virtual Customer Customer { get; private set; }
        public string SalesPersonId { get; private set; }
        public ApplicationUser SalesPerson { get; private set; }

        private Sale()
        {
            
        }
        private Sale(Guid id, Guid transactionId, string transactionNumber, Guid locationId, Guid? customerId, string salesPersonId, DateTime createdAt, Guid createdBy )
        {
            Id = id;
            TransactionId = transactionId;
            TransactionNumber = transactionNumber;
            LocationId = locationId;
            CustomerId = customerId;
            SalesPersonId = salesPersonId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;

        }

        public  static Sale Create(Guid id, Guid transactionId, string transactionNumber, Guid locationId, Guid? customerId, string salesPersonId, DateTime createdAt, Guid createdBy)
        => new Sale(id, transactionId, transactionNumber, locationId, customerId, salesPersonId, createdAt, createdBy);

        public void SoftDelete(string reason, Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            CancellationReason = reason;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

    }
}

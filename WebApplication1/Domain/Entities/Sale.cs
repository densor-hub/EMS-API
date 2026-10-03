using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public Guid? CustomerId { get; private set; }
        public virtual Customer Customer { get; private set; }
        public string SalesPersonId { get; private set; }
        public ApplicationUser SalesPerson { get; private set; }
        public ICollection<SaleTransDeliveryRequest> SaleTransDeliveryRequests { get; private set; }
        public Guid? LocationId { get; private set; }
        public int IncrementalId { get; private set; }

        // ✅ New: the day this sale belongs to
        public DateOnly SaleDate { get; private set; }

        private Sale() { }

        private Sale(Guid id, Guid transactionId, Guid? customerId, string salesPersonId,
            DateTime createdAt, Guid createdBy, Guid locationId, DateOnly saleDate)
        {
            Id = id;
            TransactionId = transactionId;
            CustomerId = customerId;
            SalesPersonId = salesPersonId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            LocationId = locationId;      // ✅ don't forget this (was missing before)
            SaleDate = saleDate;
        }

        public static Sale Create(Guid id, Guid transactionId, Guid? customerId,
            string salesPersonId, DateTime createdAt, Guid createdBy,
            Guid locationId, DateOnly saleDate)
            => new Sale(id, transactionId, customerId, salesPersonId,
                        createdAt, createdBy, locationId, saleDate);

        public void SetIncrementalId(int incrementalId)
        {
            IncrementalId = incrementalId;
        }
    }
}

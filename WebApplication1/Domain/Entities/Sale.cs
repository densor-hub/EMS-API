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

        private Sale()
        {
            
        }
        private Sale(Guid id, Guid transactionId,  Guid? customerId, string salesPersonId, DateTime createdAt, Guid createdBy )
        {
            Id = id;
            TransactionId = transactionId;
            CustomerId = customerId;
            SalesPersonId = salesPersonId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;

        }

        public  static Sale Create(Guid id, Guid transactionId,   Guid? customerId, string salesPersonId, DateTime createdAt, Guid createdBy)
        => new Sale(id, transactionId, customerId, salesPersonId, createdAt, createdBy);

        public void SoftDelete( Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

    }
}

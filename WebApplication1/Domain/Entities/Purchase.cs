using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Purchase : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public Guid SupplierId { get; private set; }
        public  Supplier Supplier { get; private set; }
        public  string PurcasedById { get; private set; }
        public ApplicationUser PurcasedBy { get; private set; }
        //public PurchaseStatusEnum Status { get; private set; }

        private Purchase()
        {
            
        }

        private Purchase(Guid id,  Guid supplierId, Guid purchasedBy,  Guid transactionId, Guid createdBy, DateTime  createdAt)
        {
            Id = id;
            SupplierId = supplierId;
            PurcasedById = purchasedBy.ToString();
            TransactionId = transactionId;
            GeneralStatus = GeneralStatus.Initiated;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
           //DisbursementId = disbursementId;

        }

        public static Purchase Create(Guid id, Guid supplierId, Guid purchasedBy,  Guid transactionId, Guid createdBy, DateTime createdAt)
        => new Purchase(id,  supplierId, purchasedBy, transactionId, createdBy , createdAt );

        public void Update (GeneralStatus status, Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = status;    
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;

        }

    }
}

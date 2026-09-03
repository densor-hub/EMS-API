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

        private Purchase()
        {
            
        }

        private Purchase(Guid id,  Guid supplierId, Guid purchasedBy,  Guid transactionId)
        {
            Id = id;
            SupplierId = supplierId;
            PurcasedById = purchasedBy.ToString();
            TransactionId = transactionId;
           //DisbursementId = disbursementId;
           
        }

        public static Purchase Create(Guid id, Guid supplierId, Guid purchasedBy,  Guid transactionId)
        => new Purchase(id,  supplierId, purchasedBy, transactionId);

    }
}

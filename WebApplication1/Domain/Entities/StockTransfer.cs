using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockTransfer : BaseEntity
    {
        public string TransactionCode { get; private set; }
        public DateTime TransferDate { get; private set; }
        public StockTakeStatus Status { get; private set; } 
        public Guid? FromLocationId { get; private set; }
        public virtual Location FromLocation { get; private set; }
        public Guid? ToLocationId { get; private set; }
        public virtual Location ToLocation { get; private set; }
        public string InitiatedById { get; private set; }
        public ApplicationUser InitiatedBy { get; private set; }
        public string? ApprovedByUser { get; private set; } = null;
        public virtual ApplicationUser? ApprovedBy { get; private set; }
        public virtual List<StockTransferItem> StockTransferItems { get; private set; }

        private StockTransfer()
        {
            
        }
        private StockTransfer(Guid id, string trnasactionCode, DateTime transferDate, StockTakeStatus status,  Guid? fromLoaction, Guid? toLocation,
            string initiatedBy, Guid createdBy, DateTime createdAt)
        {
            Id = id;
            TransactionCode = trnasactionCode;
            TransferDate = transferDate;
            Status = status;
            FromLocationId = fromLoaction;
            ToLocationId = toLocation;
            InitiatedById = initiatedBy;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            ApprovedBy = null;
        }

        public static StockTransfer Create(Guid id, string trnasactionCode, DateTime transferDate, StockTakeStatus status, Guid? fromLoaction, Guid? toLocation,
            string initiatedBy, Guid createdBy, DateTime createdAt)
        => new StockTransfer(id, trnasactionCode, transferDate, status,  fromLoaction, toLocation, initiatedBy, createdBy, createdAt);

        public void UpdateApproval(StockTakeStatus status, DateTime updatedAt, Guid updatedBy)
        {
            Status = status;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

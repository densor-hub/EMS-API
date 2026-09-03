using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockTransfer : BaseEntity
    {
        public new Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public DateTime TransferDate { get; private set; }
        public StockTransferEnum Status { get; private set; } 
        public Guid RequesterId { get; private set; }
        public virtual Location Requester { get; private set; }
        public Guid ResponderId { get; private set; }
        public virtual Location Responder { get; private set; }
       // public DateTime CompletedAt { get; private set; }

        private StockTransfer()
        {
            
        }
        private StockTransfer(Guid id, Guid transactionId, DateTime transferDate, StockTransferEnum status,  Guid requesterId, Guid responderId, Guid createdBy, DateTime createdAt)
        {
            Id = id;
            TransferDate = transferDate;
            Status = status;
            RequesterId = requesterId;
            ResponderId = responderId;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            TransactionId = transactionId;
        }

        public static StockTransfer Create(Guid id, Guid transactionId, DateTime transferDate, StockTransferEnum status, Guid requesterId, Guid responderId, Guid createdBy, DateTime createdAt)
        => new StockTransfer(id, transactionId, transferDate, status,  requesterId, responderId,  createdBy, createdAt);

        public void UpdateApproval(StockTransferEnum status, DateTime updatedAt, Guid updatedBy)
        {
            Status = status;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }

        public void Completed()
        {
            Status = StockTransferEnum.Completed;
        }
    }
}

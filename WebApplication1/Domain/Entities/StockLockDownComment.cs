using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockLockDownComment
    {
        public Guid Id { get; private set; }
        public Guid StockLockDownRequestId { get; private set; }
        public StockLockDownRequest StockLockDownRequest { get; private set; }
        public string Stage { get; private set; }
        public string TransactionType { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedById { get; private set; }
        public ApplicationUser CreatedBy { get; private set; }

        private StockLockDownComment()
        {
            
        }
        private StockLockDownComment(Guid id, Guid transactionId, string transactionType, string stage, string comment, DateTime createdAt, string createdBy)
        {
            Id = id;
            StockLockDownRequestId = transactionId;
            Comment = comment;
            CreatedAt = createdAt;
            CreatedById = createdBy;
            Stage = stage;
            TransactionType = transactionType;
        }

        public static StockLockDownComment Create(Guid id, Guid transactionId, string transactionType,string stage, string comment, DateTime createdAt, string createdBy)
        => new StockLockDownComment(id, transactionId,transactionType,  stage, comment, createdAt, createdBy);  
    }
}

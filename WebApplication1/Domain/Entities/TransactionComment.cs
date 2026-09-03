using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class TransactionComment
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public string Stage { get; private set; }
        public string TransactionType { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedById { get; private set; }
        public string CreatedByName { get; private set; }
        public ApplicationUser CreatedByUser { get; private set; }

        private TransactionComment()
        {
            
        }
        private TransactionComment(Guid id, Guid transactionId, string transactionType, string stage, string comment, DateTime createdAt, string createdBy, string createdByName)
        {
            Id = id;
            TransactionId = transactionId;
            Comment = comment;
            CreatedAt = createdAt;
            CreatedById = createdBy;
            CreatedByName= createdByName;
            Stage = stage;
            TransactionType = transactionType;
        }

        public static  TransactionComment Create(Guid id, Guid transactionId, string transactionType,string stage, string comment, DateTime createdAt, string createdById, string createdByName)
        => new TransactionComment(id, transactionId,transactionType,  stage, comment, createdAt, createdById, createdByName);  
    }
}

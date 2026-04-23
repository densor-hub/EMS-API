namespace WebApplication1.Domain.Entities
{
    public class Comments
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public int Stage { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }

        private Comments()
        {
            
        }
        private Comments(Guid id, Guid transactionId, int stage, string comment, DateTime createdAt, Guid createdBy)
        {
            Id = id;
            TransactionId = transactionId;
            Comment = comment;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Stage = stage;
        }

        public static  Comments Create(Guid id, Guid transactionId, int stage, string comment, DateTime createdAt, Guid createdBy)
        => new Comments(id, transactionId, stage, comment, createdAt, createdBy);  
    }
}

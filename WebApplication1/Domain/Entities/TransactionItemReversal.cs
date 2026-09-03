namespace WebApplication1.Domain.Entities
{
    public class TransactionItemReversal
    {
        public Guid Id { get; private set; }
        public int Quantity { get; private set; }
        public Guid TransactionItemDeliveredId { get; private set; }
        public TransactionItemDelivered TransactionItemDelivered { get; private set; }
        public Guid BatchId { get; private set; }
        public DateTime ReversalDate { get; private set; }

        private TransactionItemReversal()
        {
            
        }

        private TransactionItemReversal(Guid id, Guid transactionItemId, Guid batchId, int quantity, DateTime reversalDate)
        {
            Id = id;
            TransactionItemDeliveredId = transactionItemId;
            ReversalDate = reversalDate;
            Quantity = quantity;
            BatchId = batchId;
        }

        public static TransactionItemReversal Create(Guid id, Guid transactionItemId, Guid batchId, int quantity, DateTime reversalDate)
        => new TransactionItemReversal(id, transactionItemId, batchId, quantity, reversalDate);
    }
}

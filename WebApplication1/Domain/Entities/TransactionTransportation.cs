namespace WebApplication1.Domain.Entities
{
    public class TransactionTransportation
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateTime { get; private set; }

        private TransactionTransportation()
        {
            
        }

        public TransactionTransportation(Guid id, Guid transactionId, decimal amount, DateTime date)
        {
            Id = id;
            TransactionId = transactionId;
            Amount = amount;
            DateTime = date;
        }

        public static TransactionTransportation Create(Guid id, Guid transactionId, decimal amount, DateTime date)
        => new TransactionTransportation(id, transactionId, amount, date);
    }
}

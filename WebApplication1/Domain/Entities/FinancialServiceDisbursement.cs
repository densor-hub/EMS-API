using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class FinancialServiceDisbursement 
    {
       public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }
        public Guid FinancialServiceProviderId { get; private set; }
        public FinancialServiceProvider FinancialServiceProvider { get; private set; }
        public Guid ContactPersonId { get; private set; }
        public FinancialServiceProviderContactPerson ContactPerson { get; private set; }
        

        private FinancialServiceDisbursement()
        {
        }

        private FinancialServiceDisbursement(Guid id, Guid contactPersonId,  Guid transactionId, Guid serviceProvider)
        {
            Id = id;
            ContactPersonId = contactPersonId;
            TransactionId = transactionId;
            FinancialServiceProviderId = serviceProvider;
        }

        public static FinancialServiceDisbursement Create(Guid id, Guid contactPersonId, Guid transactionId, Guid serviceProvider)
       => new FinancialServiceDisbursement(id, contactPersonId, transactionId, serviceProvider);


    }
}

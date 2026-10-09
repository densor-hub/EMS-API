namespace WebApplication1.Domain.Entities
{
    public class FinancialServiceDisbursementContactPerson
    {
        public Guid Id { get; private set; }
        public Guid DisbursementId { get; private set; }
        public FinancialServiceDisbursement Disbursement { get; private set; }
        public Guid ContactPersonId { get; private set; }
        public FinancialServiceProviderContactPerson ContactPerson { get;   private  set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        private FinancialServiceDisbursementContactPerson()
        {
            
        }

        private  FinancialServiceDisbursementContactPerson(Guid id, Guid disbursementId, Guid contactPersonId, DateTime createdAt, Guid createdBy)
        {
            Id = id;
            DisbursementId = disbursementId;
            ContactPersonId = contactPersonId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;

        }

        public static FinancialServiceDisbursementContactPerson Create(Guid id, Guid disbursementId, Guid contactPersonId, DateTime createdAt, Guid createdBy)
         => new FinancialServiceDisbursementContactPerson
         {
             Id = id,
             DisbursementId = disbursementId,
             ContactPersonId = contactPersonId,
             CreatedAt = createdAt,
             CreatedBy = createdBy
         };
    }
}

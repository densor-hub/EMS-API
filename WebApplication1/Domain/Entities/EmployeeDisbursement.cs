using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class EmployeeDisbursement 
    {
        public Guid Id { get; private set; }
        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        public Guid EmployeeId { get; private set; }
        public  Employee Employee { get; private  set; }
        public Guid TransactionId { get; private set; }
        public Transaction Transaction { get; private set; }


        private EmployeeDisbursement()
        {
            
        }

        private EmployeeDisbursement(Guid id, Guid locationId, Guid receivingEmployeeId, Guid transactionId)
        {
            Id = id;
            LocationId = locationId;
            EmployeeId = receivingEmployeeId;
            TransactionId = transactionId;
        }

        public static EmployeeDisbursement Create(Guid id, Guid locationId, Guid receivingEmployeeId,Guid transactionId)
        => new EmployeeDisbursement(id, locationId, receivingEmployeeId, transactionId);
    }
}

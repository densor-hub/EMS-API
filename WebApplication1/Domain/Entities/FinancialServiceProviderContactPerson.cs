
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class FinancialServiceProviderContactPerson : Person
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public int IncrementalId { get; private set; }
        public Guid FinancialServiceProviderId { get; private set; }
        public FinancialServiceProvider FinancialServiceProvider { get; private set; }
        public  ICollection<FinancialServiceDisbursementContactPerson> Disbursements { get; private set; }


        //location relation
        //contact persons relations
        private FinancialServiceProviderContactPerson()
        {
            
        }

        private FinancialServiceProviderContactPerson(Guid id, string code, string fullName, string email, string phoneNumber, Guid financialServiceProvider, Guid createdBy, DateTime createdAt, string address)
        {
            Id = id;
            Code = code;
            FullName = fullName;
            FinancialServiceProviderId = financialServiceProvider;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            FirstName = fullName;
            LastName = fullName;
            Phone = phoneNumber;
        }


        public static FinancialServiceProviderContactPerson Create(Guid id, string code, string fullName, string email, string phoneNumber, Guid bankId, Guid createdBy, DateTime createdAt, string address)
         => new FinancialServiceProviderContactPerson(id, code,fullName, email, phoneNumber, bankId, createdBy, createdAt, address);

        public void Update(string fullName, string email, string phoneNumber, GeneralStatus status, Guid updatedBy, string address)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            UpdatedBy = updatedBy;
            GeneralStatus = status;
            UpdatedAt = DateTime.UtcNow;
            Address = address;
        }

        public void UpdateCode(string code)
        {
            Code = code;
        }

    }
}

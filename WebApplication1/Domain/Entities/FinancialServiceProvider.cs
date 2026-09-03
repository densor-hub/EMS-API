
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class FinancialServiceProvider : BaseEntity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public Guid LocationId { get; private set; }
        public FinancialServiceProviderType Type { get; private set; }
        public int IncrementalId { get; private set; }
        public Location Location { get; private set; }
        public string Address { get; private set; }
        public virtual ICollection <FinancialServiceDisbursement> FinancialServiceDisbursements { get; private set; }
        public virtual ICollection<FinancialServiceProviderContactPerson> ContactPersons { get; private set; }
        //location relation
        //contact persons relations
        private FinancialServiceProvider()
        {

        }

        private FinancialServiceProvider(Guid id, string code, string name, FinancialServiceProviderType type, string address, Guid locationId, Guid createdBy, DateTime createdAt, GeneralStatus status)
        {
            Id = id;
            Code = code;
            Name = name;
            LocationId = locationId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            GeneralStatus = status;
            Address = address;
            Type = type;
        }


        public static FinancialServiceProvider Create(Guid id, string code, string name, FinancialServiceProviderType type, string address, Guid locationId, Guid createdBy, DateTime createdAt, GeneralStatus status)
         => new FinancialServiceProvider(id, code, name, type, address, locationId, createdBy, createdAt, status);

        public void Update(string name, FinancialServiceProviderType type, string address, GeneralStatus status, Guid updatedBy)
        {
            Name = name;
            UpdatedBy = updatedBy;
            GeneralStatus = status;
            UpdatedAt = DateTime.UtcNow;
            Address = address;
            Type = type;
        }

        public void UpdateCode(string code)
        {
            Code = code;
        }

    }
    
}

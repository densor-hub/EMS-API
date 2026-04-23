namespace WebApplication1.Domain.Entities
{
    public class Supplier : Person
    {
        public string Code { get; private set; }
        public string SupplierCompanyName { get; private set; }
        public string TIN { get; private set; }
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; }
        public bool Status { get; private set; }
        public ICollection<SupplierLocation> SupplierLocations { get; private set; }
        public ICollection<Purchase> Purchases { get; private set; }

        public Supplier()
        {
            
        }

        private Supplier(Guid id, string supplierCompanyName, Guid byCompany, string firstname, string lastName, string email, string phone, string tin, string code, bool status,  string address, 
            DateTime createdAt, Guid createdBy)
        {
            Id = id;
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            Code = code;
            TIN = tin;
            SupplierCompanyName = supplierCompanyName;
            Status = status;
            Address = address;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Phone = phone;
            CompanyId = byCompany;
        }

        public static Supplier Create(Guid id, string supplierCompanyName, Guid byCompany, string firstname, string lastName, string email, string phone, string tin, string code, bool status, string address,
            DateTime createdAt, Guid createdBy)
       => new Supplier(id, supplierCompanyName, byCompany, firstname, lastName, email,phone, tin, code,  status,  address, createdAt, createdBy);


        public void Update(string supplierCompanyName,string firstname, string lastName, string email, string phone, string tin,  bool status, string address, DateTime updatedAt, Guid updatedBy)
        {
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            Status = status;
            Address = address;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            SupplierCompanyName = supplierCompanyName;
            TIN = tin;
            Phone = phone;

        }
    }
}

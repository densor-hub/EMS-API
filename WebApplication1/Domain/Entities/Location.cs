using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Location : BaseEntity
    {
        [Required]
        public string Code { get; private set; }
        [Required]
        public string Name { get; private set; }
        public string? Address { get; private set; }
        [Required]
        public string Phone { get; private set; }
        public string? Email { get; private set; }
        public bool Status { get; private set; }
        public LocationType? TypeOfLocation { get; private set; } //shop, warehouse

        // Foreign keys
        [Required]
        public Guid CompanyId { get; private set; }
        public Guid HeadManagerId { get; private set; }

        // Navigation properties
        public virtual Company Company { get; private set; }
        public virtual ICollection<Supplier> Suppliers { get; private set; }
        public virtual ICollection <LocationManangement> LocationManangements { get; private set; } = null;
       /// public virtual ICollection<Employee> Employees { get; private set; }
        public virtual ICollection<ItemLocation> ItemLocations { get; private set; }
        public virtual ICollection<EmployeeLocation> EmployeeLocations { get; private set; }
        public virtual ICollection<SupplierLocation> SupplierLocations { get; private set; }
        public virtual ICollection<Customer> Customers { get; private set; }
        public virtual ICollection<Purchase> Purchases { get; private set; }
        public virtual ICollection<Sale> Sales { get; private set; }
        public virtual ICollection<StockTaking> StockTakings { get; private set; }
        public virtual ICollection<StockTransfer> StockTransfersFrom { get; private set; }
        public virtual ICollection<StockTransfer> StockTransfersTo { get; private set; }
        public virtual ICollection<StockLevel> StockLevels { get; private set; }
        public virtual ICollection<LocationPayments> LocationPayments { get; private set; }


        public Location()
        {
            
        }

        private Location(Guid id, string code, string name, bool status, string address, string phone, string email, Guid companyId, LocationType? type, DateTime createdAt, Guid createdBy)
        {
            Id = id; Code = code; Name = name; Status = status; Address = address; Phone = phone; Email = email; CompanyId = companyId; TypeOfLocation = type; CreatedAt = createdAt; CreatedBy = createdBy;
        }

        public static Location Create(Guid id, string code, string name, bool status, string address, string phone, string email, Guid companyId, LocationType? type, DateTime createdAt, Guid createdBy)
        => new Location(id, code, name, status, address, phone, email, companyId, type, createdAt, createdBy);


        public void SoftDelete (Guid deletedBy)
        {
            Status = false;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = deletedBy;
        }

        public void Update(string code, string name, bool status, string address, string phone, string email,LocationType? type, DateTime updatedAt, Guid updatedBy)
        {
            Code =code; Name = name; Status = status; Address =address; Phone = phone;  Email =email; TypeOfLocation = type; UpdatedAt = updatedAt; UpdatedBy = updatedBy;
        }
    }

}

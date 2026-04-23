namespace WebApplication1.Domain.Entities
{
    public class Company : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string? Address { get; private set; }
        public string Email { get; private set; }
        public string? RegistrationNumber { get; private set; }
        public string? TIN { get; private set; }
        public string? Website { get; private set; }
        public bool? Status { get; private set; } = true;
        public ICollection<Position> Positions { get; private set; }
        public ICollection<ApplicationUser> Users { get; private set; }
        public virtual ICollection<Location> Locations { get; private set; }
        public virtual ICollection<Supplier> Suppliers { get; private set; }
        public virtual ICollection<Item> Items { get; private set; }
       

        public Company()
        {
            
        }

        private Company(Guid id, string name, string phoneNumber, string email,  bool status, DateTime createdAt)
        {
            Id= id;
            Name= name;
            Email= email;
            PhoneNumber= phoneNumber;
            Status= status;
            CreatedAt= createdAt;
        }


    private Company(Guid id, string name, string phoneNumber, string email, bool status, DateTime createdAt, string tin, string registrationNumber, string address, string website, Guid createdBy)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            Email = email;
            Website = website;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            RegistrationNumber = registrationNumber;
            TIN = tin;
            Status = status;
        }

        public static Company Create(Guid id, string name, string phoneNumber, string email,  bool status, DateTime createdAt)
        => new Company( id, name, email, phoneNumber, status, createdAt);

        public static Company Create(Guid id, string name, string phoneNumber, string email, bool status, DateTime createdAt, string tin, string registrationNumber, string address,string website, Guid createdBy)
        => new Company(id, name, phoneNumber, email,status, createdAt, tin, registrationNumber, address, website,  createdBy); 
        
        public void Update(string name, string phoneNumber, string address, string email, string tin, string registrationNumber,  string website, DateTime updatedAt, Guid updatedBy)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Address = address;
            Email = email;
            Website = website;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            RegistrationNumber = registrationNumber;
            TIN = tin;
        }

        public Company Clone()
        {
            return (Company) MemberwiseClone();
        }
    }
}

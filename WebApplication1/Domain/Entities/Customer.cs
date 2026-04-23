using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Customer : Person
    {
        public string Code { get; private set; }
        public decimal? CreditLimit { get; private set; } = 0;

        // Foreign keys
        public Guid LocationId { get; private set; }
        public bool Status { get; private set; }

        // Navigation properties
        public virtual Location Location { get; private set; }
        public virtual ICollection<Sale> Sales { get; private set; } //sale, credit
        public string SecondaryPhone { get; private set; }
        public string SecondaryEmail { get; private set; }

        public Customer()
        {
            
        }

        private Customer(Guid id, string firstname, string lastName, string email, string phone, string code, bool status, Guid locationId, string address, decimal creditLimit, 
            DateTime createdAt, Guid createdBy, string secondaryEmail, string secondaryPhone)
        {
            Id = id;
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            Code = code;
            CreditLimit = creditLimit;
            Status = status;
            LocationId = locationId;
            Address = address;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Phone = phone;
            SecondaryEmail = secondaryEmail;
            SecondaryPhone = secondaryPhone;
        }

        public static Customer Create(Guid id, string firstname, string lastName, string email, string phone, string code, bool status, Guid locationId,
            string address, decimal creditLimit, DateTime createdAt, Guid createdBy, string secondaryEmail, string secondaryPhone)
       => new Customer(id, firstname, lastName, email, phone, code, status, locationId, address, creditLimit, createdAt, createdBy, secondaryEmail, secondaryPhone);


        public void Update(string firstname, string lastName, string email, string phone, decimal creditLimit,
            bool status, Guid locationId, string address, DateTime updatedAt, Guid updatedBy, string secondaryEmail, string secondaryPhone)
        {
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            CreditLimit = creditLimit;
            Status = status;
            LocationId = locationId;
            Address = address;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            Phone = phone;
            SecondaryPhone = secondaryPhone;
            SecondaryEmail= secondaryEmail;
        }
    }
}

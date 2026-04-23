using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
   public class Employee : Person
    {
        public string Code { get; private set; }
        public Guid PositionId { get; private set; }
        public  Position Position { get; private set; }
        public DateTime HireDate { get; private set; }
        public decimal Salary { get; private set; }
        public EmployeeStatus Status { get; private set; }
        public string Phone { get; private set; }
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; }
        //public Guid LocationId { get; private set; }
        // public Location Location { get; private set; }
        public ICollection<EmployeeLocation> EmployeeLocations { get; private set; }
        public ICollection<LocationPayments> LocationPaymentsReceiver { get; private set; }
        public ICollection<LocationManangement> ShopManangement { get; set; }

        public Employee()
        {
            
        }

        private Employee(Guid id, string  firstname, string lastName, string email, string code, Guid positionId,  string phone,
            DateTime hireDate, decimal salary, EmployeeStatus status, Guid companyId, string address, bool appUser, DateTime createdAt, Guid createdBy)
        {
            Id = id;
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            Code = code;
            PositionId = positionId;
            HireDate = hireDate;
            Salary = salary;
            Status = status;
            //LocationId = locationId;
            Address = address;
            IsAppUser = appUser;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Phone = phone;
            CompanyId = companyId;
        }

        public static Employee Create(Guid id, string firstname, string lastName, string email, string code, Guid positionId, string phone,
            DateTime hireDate, decimal salary, EmployeeStatus status, Guid companyId, string address, bool appUser, DateTime createdAt, Guid createdBy)
       => new Employee(id, firstname, lastName, email, code, positionId,phone, hireDate, salary, status, companyId, address, appUser, createdAt, createdBy);


        public void Update (string firstname, string lastName, string email, Guid positionId, string phone, DateTime hireDate, decimal salary,
            EmployeeStatus status, Guid companyId, string address, bool appUser, DateTime updatedAt, Guid updatedBy)
       {
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            PositionId = positionId;
            HireDate = hireDate;
            Salary = salary;
            Status = status;
            CompanyId = companyId;
            Address = address;
            IsAppUser = appUser;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            Phone = phone;
       }

        public void SoftDelete()
        {
            Status = EmployeeStatus.Terminated;
        }

    }
}

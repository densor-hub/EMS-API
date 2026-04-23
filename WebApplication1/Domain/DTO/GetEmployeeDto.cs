using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class GetEmployeeDto
    {
        public Guid Id { get; set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public  string Phone { get; set; }
        public string? Email { get; set; }
        public  string Address { get; set; }
        public bool IsAppUser { get; set; } = false;
        public Guid RoleId { get; set; }
        public string PositionName { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public EmployeeStatus Status { get; set; }
        public string? StatusName { get; set; }
        public string? Code { get; set; }
        public List<DropDownDTO>? Locations { get; set; }
        //public string LocationName { get; set; }
    }
}

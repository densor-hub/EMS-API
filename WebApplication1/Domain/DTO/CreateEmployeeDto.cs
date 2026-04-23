using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateEmployeeDto
    {
        public  string FirstName { get; set; }
        [Required]
        public  string LastName { get; set; }
        [Required]
        public  string Phone { get; set; }
        public string? Email { get; set; }
        public  string? Address { get; set; }
        [Required]
        public bool IsAppUser { get; set; } = false;
        public Guid PositionId { get; set; }
        public DateTime HireDate { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public EmployeeStatus Status { get; set; }
        [Required]
        public List<Guid> Locations { get; set; }
    }
}

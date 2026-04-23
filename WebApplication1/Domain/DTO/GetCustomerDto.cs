using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class GetCustomerDto
    {
        public Guid Id { get; set; }
        public  string FirstName { get; set; }
        public  string LastName { get; set; }
        public  string Phone { get; set; }
        public string? Email { get; set; }
        public  string Address { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
        public string Code { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public decimal CreditLimit { get; set; }
        public string NationalId { get; set; }
    }
}

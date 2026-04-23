using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class UpdateCustomerDTO
    {
        public Guid Id { get; set; } 
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; } 
        public decimal? CreditLimit { get; set; } = null;
        public string? NationalIdentificationNumber { get; set; }
    }
}

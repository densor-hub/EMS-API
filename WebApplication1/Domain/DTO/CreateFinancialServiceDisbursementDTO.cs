using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.DTO
{
    public class CreateFinancialServiceDisbursementDTO
    {
        public  Guid LocationId { get; set; }
        public Guid  FinancialServiceProviderId { get; set; }
        //public TransactionResultsType TransactionResultsType { get; set; }
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public string? CurrencyCode { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public string? Remarks { get; set; } = "";
        public List<ContactPersons> ContactPersons { get; set; }

    }

    public class ContactPersons
    {
        public Guid? Id { get; set; } = Guid.Empty;
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Addrress { get; set; }
    }


    public class GetFinancialServiceDisbursementDTO
    {
        public Guid Id { get; set; }
        public string? LocationName { get; set; }
        public string FinacialServiceProviderName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionCode { get; set; }
        public List<ContactPersons>?ContactPersons { get; set; }
    }
}

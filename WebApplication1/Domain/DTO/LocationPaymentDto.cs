// DTOs/EmployeeDisbursementDtos.cs
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateEmployeeDisbursementDto
    {
        public Guid LocationId { get; set; }
        public Guid EmployeeId { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public TransactionType TransactionType { get; set; }
        public string CurrencyCode { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateEmployeeDisbursementDto
    {
        public Guid? ReceiverId { get; set; }
    }

    public class EmployeeDisbursementResponseDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PaymentTypeName { get; set; }
    }
}
// DTOs/Payment/PaymentDto.cs
using WebApplication1.Domain.Enums;

namespace WebApplication1.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class CreatePaymentDto
    {
        public Guid TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
    }

    public class UpdatePaymentDto
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
    }
}
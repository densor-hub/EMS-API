// DTOs/Payment/PaymentCancellationDto.cs
namespace WebApplication1.DTOs
{
    public class PaymentCancellationDto
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }

    public class CreatePaymentCancellationDto
    {
        public Guid PaymentId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }
}
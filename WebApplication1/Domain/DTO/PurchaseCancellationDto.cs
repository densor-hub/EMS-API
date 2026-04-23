// DTOs/Purchase/PurchaseCancellationDto.cs
namespace WebApplication1.DTOs
{
    public class PurchaseCancellationDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }

    public class CreatePurchaseCancellationDto
    {
        public Guid PurchaseId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }
}
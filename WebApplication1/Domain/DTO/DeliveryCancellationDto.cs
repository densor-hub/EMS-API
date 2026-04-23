// DTOs/Delivery/DeliveryCancellationDto.cs
namespace WebApplication1.DTOs
{
    public class DeliveryCancellationDto
    {
        public Guid Id { get; set; }
        public Guid DeliveryId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }

    public class CreateDeliveryCancellationDto
    {
        public Guid DeliveryId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }
}
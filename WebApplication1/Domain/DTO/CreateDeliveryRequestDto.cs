namespace WebApplication1.Domain.DTO
{
    public class CreateDeliveryRequestDto
    {
      public  Guid TransactionId { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}

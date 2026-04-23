// DTOs/Delivery/DeliveryDto.cs
namespace WebApplication1.DTOs
{
    public class DeliveryDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<DeliveryItemDto> Items { get; set; }
    }

    public class DeliveryItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateDeliveryDto
    {
        public Guid TransactionId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public List<CreateDeliveryItemDto> Items { get; set; }
    }

    public class CreateDeliveryItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateDeliveryDto
    {
        public DateTime DeliveryDate { get; set; }
        public List<UpdateDeliveryItemDto> Items { get; set; }
    }

    public class UpdateDeliveryItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
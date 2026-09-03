// DTOs/Common/TransactionItemDto.cs
namespace WebApplication1.DTOs
{
    public class TransactionItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        //public int? DeliveredQuantity { get; set; } = 0;
        public string? Code { get; set; }
        public decimal CostPrice { get; set; }
        public decimal ItemPrice { get; set; }
        public List<GetTransactionItemsDeliveredDto>? Deliveries { get; set; }
    }

    public class CreateTransactionItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? DeliveredQuantity { get; set; } = 0;

    }
  
    public class GetTransactionItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string  Name { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } //PurchasePrice
        public List<GetTransactionItemsDeliveredDto>? ItemsDelivered { get; set; }
        public List<GetTransactionItemsReceivedDto>? ItemsReceived { get; set; }
    }

    public class UpdateTransactionItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }


    public class GetTransactionItemsReceivedDto
    {
        public Guid DeliveryId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public Guid TransactionDeliveryRequestId { get; set; }
        public List<TransactionItemsReversalDto>? ItemReversals { get; set; }
    }

    public class GetTransactionItemsDeliveredDto
    {
        public Guid DeliveryId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public Guid TransactionDeliveryRequestId { get; set; }
        public List<TransactionItemsReversalDto>?ItemReversals { get; set; }
    }

    public class TransactionItemsReversalDto
    {
        public Guid ReversalId { get; set; }
        public DateTime ReversalDate { get; set; }
        public int Quantity { get; set; }

    }

    public class TransactionCancellationDto
    {
        public Guid Id { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
    }
}
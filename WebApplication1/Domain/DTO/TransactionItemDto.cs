// DTOs/Common/TransactionItemDto.cs
namespace WebApplication1.DTOs
{
    public class TransactionItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int? DeliveredQuantity { get; set; } = 0;
        public string? Code { get; set; }
        public decimal CostPrice { get; set; }
        public decimal itemPrice { get; set; }
    }

    public class CreateTransactionItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int? DeliveredQuantity { get; set; } = 0;

    }

  
    public class GetTransactionItemDto
    {
        public Guid ItemId { get; set; }
        public string  ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } //PurchasePrice
    }

    public class UpdateTransactionItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
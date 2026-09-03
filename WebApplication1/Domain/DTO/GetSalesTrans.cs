using WebApplication1.DTOs;

namespace WebApplication1.Domain.DTO
{
    public class GetSalesTrans
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        //public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string? TransactionCode { get; set; }
        public List<GetTransactionItemDto>? Items { get; set; }
    }

    public class GetSalesReceiptDto
    {
        public string? LocationName { get; set; }
        public string? CustomerName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string? TransactionCode { get; set; }
        public List<GetSaleReceiptItemDto>? Items { get; set; }
    }

    public class GetSaleReceiptItemDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}

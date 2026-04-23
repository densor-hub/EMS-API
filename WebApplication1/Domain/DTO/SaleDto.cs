// DTOs/Sale/SaleDto.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;

namespace WebApplication1.DTOs
{

    public class GetSaleDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string TransactionCode { get; set; }
        public List <TransactionItemDto>? Items { get; set; }
        public List<TransactionPaymentsDto>? Payments { get; set; }
       

    }

    public class CreateSaleDto
    {
        public Guid LocationId { get; set; }
        public Guid? CustomerId { get; set; } = Guid.Empty;
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public TransactinType TransactinType { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public List<CreateTransactionItemDto> Items { get; set; }
        public bool ForceIt { get; set; }   
        
    }

    public class UpdateSaleDto
    {
        public Guid Id { get; set; }
        public string Notes { get; set; }
    }
}
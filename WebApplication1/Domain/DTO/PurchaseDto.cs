
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;

namespace WebApplication1.DTOs
{
    public class PurchaseDto 
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionCode { get; set; }
        //public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        //public string PurchasedById { get; set; }
        public string TransactionBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public List <TransactionItemDto>? Items { get; set; }
        public List<TransactionPaymentsDto>? Payments { get; set; }
    }

    public class CreateTransactionDto
    {
        public Guid LocationId { get; set; }
        public Guid BusinessPartnerId { get; set; }
        public TransactinType TransactinType { get; set; }
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public List<CreateTransactionItemDto> Items { get; set; }
        public string? TransactionCode { get; set; } = string.Empty;
    }

    public class UpdatePurchaseDto
    {
        public Guid PurchaseId { get; set; }
        public Guid LocationId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid PurchasedBy { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Notes { get; set; }
        public List<UpdateTransactionItemDto> Items { get; set; }
    }


    public class TransactionPaymentsDto
    {
        public Guid TransationId { get; set; } 
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethods PaymentMethod { get; set; }

    }

    public class TransactionItemsReceivedDto
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

    }

}
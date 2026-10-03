
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.Services.Emails.TemplateService.Enitities;

namespace WebApplication1.DTOs
{
    public class GetPurchaseDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        //public string PurchasedById { get; set; }
        public string TransactionBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }

    }

    //public class CreateBusinessPartnerTransactionDto
    //{
    //    public Guid LocationId { get; set; }
    //    public Guid BusinessPartnerId { get; set; }
    //    public TransactionResultsType TransactionResultsType { get; set; }
    //    public DateTime? Date { get; set; } = DateTime.UtcNow;
    //    public decimal TotalAmount { get; set; }
    //    public decimal TaxAmount { get; set; }
    //    public decimal DiscountAmount { get; set; }
    //    public decimal AmountPaid { get; set; }
    //    public PaymentMethods PaymentMethod { get; set; }
    //    public List<CreateTransactionItemDto> Items { get; set; }
    //    public string? TransactionCode { get; set; } = string.Empty;
    //    public string? CouponCode { get; set; } = string.Empty;
    //    public string? CurrencyCode { get; set; } = string.Empty;

    //    //NEW 
    //    public TransactionType TransactionType { get; set; }
    //    public string? Remarks { get; set; } = "";

    //}

    public class CreateTransactionDto
    {
        public Guid LocationId { get; set; }
        public Guid? BusinessPartnerId { get; set; }
        public TransactionResultsType TransactionResultsType { get; set; }
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public List<CreateTransactionItemDto> Items { get; set; }
        public string? TransactionCode { get; set; } = string.Empty;
        public string? CouponCode { get; set; } = string.Empty;
        public string? CurrencyCode { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; } = 0;
        //NEW 
        public TransactionType TransactionType { get; set; }
        public string? Remarks { get; set; } = "";

    }

    public class GetTransactionDto
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string? LocationName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionCode { get; set; }
        public List<GetTransactionItemDto>? Items { get; set; }
        public List<GetTransactionPaymentsDto>? Payments { get; set; }


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


    public class GetTransactionPaymentsDto
    {
        public Guid TransactionId { get; set; } = Guid.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethods PaymentMethod { get; set; } //PaymentMethods converted to string
        public string? PaymentMethodName { get; set; } = "";
        public string? Remarks { get; set; }
        public GetPaymentCouponDto? Coupon { get; set; }

    }

    public class TransactionPaymentsDto
    {
        public Guid TransationId { get; set; } = Guid.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethods PaymentMethod { get; set; } //PaymentMethods converted to string
        public string? PaymentMethodName { get; set; } = "";
        public string? Remarks { get; set; }
        public string? CouponCode { get; set; }
        public string? CurrencyCode { get; set; }

    }

    public class GetPaymentCouponDto
    {
        public string Code { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransactionItemsReceivedDto
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

    }

    public class SpecificTransactionCreationReturnDto
    {
        public string QrCode { get; set; }
        public int? Count { get; set; }
        public EmailReceiver EmailReceiver { get; set; }
    }

}
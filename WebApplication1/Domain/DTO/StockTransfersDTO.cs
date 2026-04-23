using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class StockTransfersDTO
    {
        public Guid Id { get; set; }
        public string TransactionCode { get;  set; }
        public string SupplierName { get; set; }
        public Guid? SupplierId { get; set; }
        public string CustomerName { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime TransactionDate { get;  set; }
        public StockTakeStatus StatusNumber { get;  set; }
        public string Status { get;  set; }
        public string? InitiatedBy { get;  set; }
        public string? ApprovedBy { get;  set; }
        public decimal? TotalAmount { get; set; }
        public List<StockTransfersDTOItems> Items { get;  set; }
    }


    public class StockTransfersDTOItems { 
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string? Code { get; set; }
        public string? ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int RequestedQuantity { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public int DeliveredQuantity { get; set; }
        public decimal CostPrice { get; set; }

    }

}

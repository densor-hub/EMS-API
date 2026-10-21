using WebApplication1.Domain.Enums;

namespace WebApplication1.Controllers
{
    

    public class StockTakeCreateDto
    {
        public Guid LocationId { get; set; }
        public string ConductedBy { get; set; }
        public DateTime StockTakingDate { get; set; }
        public DateTime? NextStockTakingDate { get; set; }
        public string? Comment { get; set; }
        public Guid CreatedBy { get; set; }
        public List<StockTakeItemDto>? Items { get; set; }
    }

    public class StockTakeItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class StockTakeVerifyDto
    {
        public Guid StockTakingId { get; set; }
        public Guid VerifiedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public List<StockTakeItemDto> Items { get; set; }
        public StockTakeStatus Stage { get; set; }
        public string? Comment { get; set; }
    }


    public class StockTransferCreateDto
    {
        public DateTime TransferDate { get; set; }
        public string? Notes { get; set; }
        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }
        public string? Comment { get; set; } = string.Empty;
        public bool ForceIt { get; set; }
      //  public string? InitiatedById { get; set; }
       // public Guid CreatedBy { get; set; }
        public List<StockTakeItemDto>? Items { get; set; }
    }


    public class StockTransferApproveDto
    {
        public Guid StockTransferId { get; set; }
        public string Comment { get;  set; }
        public StockTransferEnum Stage { get; set; }
    }

 

    public class StockTransferCancelDto
    {
        public Guid CancelledBy { get; set; }
    }

    public class StockTransferReceiveDto
    {
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public Guid LocationId { get; set; }
        public string? Comment { get; set; }
        public List<ReceivedItemDto> items { get; set; } = new();
    }

    public class ReceivedItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        
    }

    public record CreateStockAdjustmentRequest(
        Guid ItemId,
        Guid LocationId,
        int Quantity,
        Guid? SupplierId,
        DateTime TransactionDate);

    // Verify request — approve or decline
    public record VerifyStockAdjustmentRequest(
        bool Approve,
        string Remarks, 
        GeneralStatus Status);           // true = approve, false = decline

    // ---- Response ----

    public record StockAdjustmentDto(
        Guid Id,
        Guid ItemId,
        string ItemName,
        Guid LocationId,
        string LocationName,
        int Quantity,
        Guid? SupplierId,
        string? SupplierName,
        DateTime CreatedAt,
        DateTime TransactionDate,
        string CreatedById,
        GeneralStatus GeneralStatus,
        string? ApprovedById,
        DateTime? ApprovedAt,
        string StatusName
       );
}
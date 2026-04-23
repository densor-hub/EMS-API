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
        public StockTakeStatus Stage { get; set; }
        //public List<StockTakeItemDto>? ReceivedItems { get; set; }
        //public string ConfirmationCode { get; set; }
        //public bool ForceIt { get; set; }
    }

 

    public class StockTransferCancelDto
    {
        public Guid CancelledBy { get; set; }
    }
}
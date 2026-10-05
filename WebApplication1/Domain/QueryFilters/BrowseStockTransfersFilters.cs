using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.QueryFilters
{
    public class BrowseStockTransfersFilters
    {
        public Guid? LocationId { get; set; }
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public StockTransferEnum? Stage { get; set; } 
        public bool Approval { get; set; } = false;
        public int? Type { get; set; } = 1;
    }
}

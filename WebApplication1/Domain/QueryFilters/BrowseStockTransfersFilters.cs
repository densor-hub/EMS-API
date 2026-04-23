using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.QueryFilters
{
    public class BrowseStockTransfersFilters
    {
        public Guid LocationId { get; set; }
        public GeneralStatus GeneralStatus { get; set; } = GeneralStatus.Active;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
        public bool From { get; set; }
        public int? Stage { get; set; } = 1;
    }
}

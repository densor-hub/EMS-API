using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.QueryFilters
{
    public class BrowseSalesFilters : BaseFilter
    {
        public Guid LocationId { get; set; }
        public GeneralStatus GeneralStatus { get; set; } = GeneralStatus.Active;
        public Guid? CustomerId { get; set; } = null;
        public Guid? SalesPersonId { get; set; } = null;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;
    }
}

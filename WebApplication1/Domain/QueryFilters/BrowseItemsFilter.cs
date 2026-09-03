using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.QueryFilters
{
    public class BrowseItemsFilter : BaseFilter
    {
        public Guid LocationId { get; set; }
        public ItemsCategory? Category { get; set; } = null;

        //public TransactionType? TransactionType { get; set; } = Enums.TransactionType.SALE;
    }
}

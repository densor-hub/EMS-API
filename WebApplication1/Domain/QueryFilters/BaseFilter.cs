namespace WebApplication1.Domain.QueryFilters
{
    public class BaseFilter
    {
        public Guid? LocationId = Guid.Empty;
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? TextFilter { get; set; } = null;
        public bool OnlyActive { get; set; } = true;
        
    }
}

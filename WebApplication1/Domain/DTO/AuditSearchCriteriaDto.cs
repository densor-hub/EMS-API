namespace WebApplication1.Domain.DTO
{
    public class AuditSearchCriteriaDto
    {
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Action { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchTerm { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
}

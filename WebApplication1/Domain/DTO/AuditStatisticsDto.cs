namespace WebApplication1.Domain.DTO
{
    public class AuditStatisticsDto
    {
        public int TotalChanges { get; set; }
        public Dictionary<string, int> ByEntityType { get; set; }
        public Dictionary<string, int> ByAction { get; set; }
        public Dictionary<string, int> ByUser { get; set; }
        public string Period { get; set; }

    }
}

namespace WebApplication1.Domain.Entities
{
    public class AuditSnapshot
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Snapshot { get; set; }            // Full entity state as JSON
        public DateTime SnapshotAt { get; set; }
        public string TakenBy { get; set; }
        public string Reason { get; set; }
    }
}

namespace WebApplication1.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; }          // "StockTaking", "Transaction", "Item", "User", etc.
        public string EntityId { get; set; }            // ID as string to handle different types
        public string Action { get; set; }              // "Create", "Update", "Delete", "Lock", "Unlock", etc.
        public string OldValue { get; set; }            // JSON serialized old state
        public string NewValue { get; set; }            // JSON serialized new state
        public string ChangedBy { get; set; }           // User ID
        public string ChangedByName { get; set; }
        public DateTime ChangedAt { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string RequestPath { get; set; }
        public string RequestMethod { get; set; }
        public string Reason { get; set; }              // Why the change was made
        public Dictionary<string, object> Metadata { get; set; } // Additional context
        public List<AuditDetail> Details { get; set; }  // Detailed changes
    }
}

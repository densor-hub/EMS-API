namespace WebApplication1.Domain.Entities
{
    public class AuditDetail
    {
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string DataType { get; set; }
        public bool IsSensitive { get; set; }
    }
}

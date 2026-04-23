using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public GeneralStatus GeneralStatus { get; set; } = GeneralStatus.Active;
        public string? CancellationReason { get; set; } = string.Empty;
    }
}

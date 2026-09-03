using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Domain.Entities
{
    public abstract class BaseEntity : IAuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public GeneralStatus GeneralStatus { get; set; } = GeneralStatus.Active;
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        //public string? CancellationReason { get; set; } = string.Empty;

        //[NotMapped]
        //public Dictionary<string, object> OriginalValues { get; set; }

        public void BaseSoftDelete(Guid deletedBy)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
            IsDeleted = true;
        }
    }
}

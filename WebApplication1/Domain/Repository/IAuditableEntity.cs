using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Repository
{
    public interface IAuditableEntity
{
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedAt { get;  set; }
    }
}

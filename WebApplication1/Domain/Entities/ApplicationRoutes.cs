using WebApplication1.Domain.DTO;

namespace WebApplication1.Domain.Entities
{
    public class ApplicationRoutes
    {
        public Guid Id { get; set; }
        public string Title { get; set; }    
        public bool Status { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; } = null;
        public virtual  ApplicationRoutes? Parent { get; set; }
        public string? Path { get; set; }
        public virtual ICollection<ApplicationRoutes>? Children { get; set; }
        public virtual ICollection<PositionRoutes>? PositionRoutes { get; set; }
    }
}

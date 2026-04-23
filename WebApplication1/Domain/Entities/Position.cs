using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.Entities
{
    public class Position : BaseEntity
    {
       [Required]
       public  string Title { get; private set; }
        public Guid? CompanyId { get; private set; } = null;
       public virtual Company Company { get; private set; }
       public bool Status { get; private set; }
        public string Description { get; private set; }
       public ICollection<Employee> Employees { get; private  set;}
       public virtual ICollection<PositionRoutes> PositionRoutes { get; private set; }

        public Position()
        {
            
        }

        private Position(Guid id, string title, Guid? companyId, bool status, DateTime createdAt, Guid createdBy, string description)
        {
            Id = id;
            Title = title;
            CompanyId = companyId;
            Status = status;
            CreatedAt = createdAt;  
            CreatedBy = createdBy;
            Description = description;
        }

        public static Position Create(Guid id, string title, Guid? companyId, bool status, DateTime createdAt, Guid createdBy, string description) 
        => new Position(id, title, companyId, status, createdAt, createdBy, description);

        public void Update(string tiltle, bool status, DateTime updatedAt, Guid updatedBy, string description)
        {
            Title= tiltle;
            Status = status;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
            Description = description;
        }
    }
}

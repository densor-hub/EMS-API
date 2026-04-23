using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.DTO
{
    public class CreatePositionDTO
    {
        public string Title { get; set; }
        public bool Status { get; set; }
        public string? Description { get; set; }
        public List <Guid>? Routes { get; set; } = new List<Guid>();
    }


    public class UpdatePositionDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        public string? Description { get; set; }
        public List<Guid>? Routes { get; set; }
    }

    
}

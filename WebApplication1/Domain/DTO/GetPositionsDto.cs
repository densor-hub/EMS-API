namespace WebApplication1.Domain.DTO
{
    public class GetPositionsDto
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public List<Guid>? Permissions { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}

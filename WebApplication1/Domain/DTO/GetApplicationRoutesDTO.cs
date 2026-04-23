namespace WebApplication1.Domain.DTO
{
    public class GetApplicationRoutesDTO
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public int? Level { get; set; } = null;
        public Guid ParentId { get; set; }
        public string? Path { get; set; }
        public IEnumerable<GetApplicationRoutesDTO> Children { get; set; } = null;
    }
}

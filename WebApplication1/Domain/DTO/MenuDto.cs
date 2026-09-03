// Domain/DTO/MenuDto.cs
namespace WebApplication1.Domain.DTO
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        public int Level { get; set; }
        public string Path { get; set; }
        public List<MenuDto> Children { get; set; } = new();
    }

    public class MenuHierarchyDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public bool Status { get; set; }
        public int Level { get; set; }
        public string? ParentId { get; set; }
        public List<MenuHierarchyDto> Children { get; set; } = new();
    }
}
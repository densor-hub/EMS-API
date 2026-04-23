namespace WebApplication1.Domain.DTO
{
    public class GeneralDeleteDTO
    {
        public Guid Id { get; set; }
        public string? Comment { get; set; } = string.Empty;
    }
}

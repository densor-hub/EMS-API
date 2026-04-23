namespace WebApplication1.Domain.DTO
{
    public class AuthResponseDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public IdAndNameDTO Company { get; set; }
        public List<DropDownDTO>? Locations { get; set; }
        public List<DropDownDTO>? Routes { get; set; }
        public bool IsCreator { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class UpdateShopDTO
    {
        public Guid Id { get; set; }
        public  string? Code { get; set; } = string.Empty;
        [Required]
        public  string Name { get; set; }
        public string? Address { get; set; } = string.Empty;
        [Required]
        public  string Phone { get; set; }
        public string? Email { get; set; } = string.Empty;
        public bool Status { get; set; }
        public LocationType? LocationType { get; set; } = null;
        public List<LocationMangersDTO?>? Managers { get; set; }
    }
}

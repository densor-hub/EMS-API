using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateShopDTO
    {
        public  string? Code { get; set; } = string.Empty;
        [Required]
        public  string Name { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public  string Phone { get; set; }
        [Required]
        public string? Email { get; set; }
        public bool Status { get; set; }
        public LocationType? LocationType { get; set; } = null;
        public List<LocationMangersDTO?>? Managers { get; set; }
    }

    public class LocationMangersDTO
    {
        public Guid Id { get; set;}
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;
        public bool IsMainManager { get; set; }
    }
}

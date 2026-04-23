using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateSupplierDto
    {
        [Required]
        public  string FirstName { get; set; }
        [Required]
        public  string LastName { get; set; }
        [Required]
        public  string Phone { get; set; }
        public string? Email { get; set; }
        [Required]
        public  string Address { get; set; }
        [Required]
        public string  SupplierComanyName { get; set; }
        [Required]
        public bool Status { get; set; }
        public string? Tin { get; set; }
        public List<Guid> Locations { get; set; }

    }
}

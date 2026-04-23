using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateItemDTO
    {

        [StringLength(50)]
        public string? Code { get; set; } = null;

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public ItemsCategory Category { get; set; }

        [Required]
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int QuantityInUnit { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal CostPrice { get; set; }
        public int? ReorderLevel { get; set; }

        [Required]
        public List<Guid> Locations { get; set; }
        public bool Status { get; set; }
    }
}

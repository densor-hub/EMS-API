using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class UpdateItemDto
    {
        [Required]
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ItemsCategory Category { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int? QuantityInUnit { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? SellingPrice { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal? CostPrice { get; set; }
        public int? ReorderLevel { get; set; }
        public List<Guid> Locations { get; set; }
        public bool Status { get; set; }
    }
}

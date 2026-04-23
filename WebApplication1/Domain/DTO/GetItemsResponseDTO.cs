using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class GetItemsResponseDTO
    {
        public Guid Id { get; set; }
        public string Code { get;  set; }
        public string Name { get;  set; }
        public string? Description { get;  set; }
        public ItemsCategory Category { get;  set; }
        public string? CategoryName { get; set; }
        public UnitOfMeasure UnitOfMeasure { get;  set; }
        public string? UnitOfMeasureName { get; set; }
        public int QuanityInUnit { get;  set; }
        public decimal SellingPrice { get;  set; }
        public decimal CostPrice { get;  set; }
        public int ReorderLevel { get;  set; }
        public bool Status { get;  set; }
        public List<DropDownDTO>? Locations { get; set; }
    }
}

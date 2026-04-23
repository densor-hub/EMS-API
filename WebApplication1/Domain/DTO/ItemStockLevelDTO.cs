namespace WebApplication1.Domain.DTO
{
    public class ItemStockLevelDTO
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int ReorderLevel { get; set; }
        public int AvailableQuantity { get; set; }
        public int ActualQuantity { get; set; }
    }
}

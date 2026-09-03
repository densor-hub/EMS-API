namespace WebApplication1.Domain.Entities
{
    public class SupplierItemCostPrice
    {
        public Guid Id { get; private set; }
        public Guid SupplierId { get; private set; }
        public Supplier Supplier { get; private set; }
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }
        public decimal Price { get; private set; }
        public string CreatedById { get; private set; } 
        public ApplicationUser ApplicationUser { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private SupplierItemCostPrice()
        {
            
        }
        private SupplierItemCostPrice(Guid id, Guid supplierId, Guid itemId, decimal price, string createdById, DateTime createdAt)
        {
            Id = id;
            SupplierId = supplierId;
            ItemId = itemId;
            Price = price;
            CreatedById = createdById;
            CreatedAt = createdAt;

        }

        public static SupplierItemCostPrice Create(Guid id, Guid supplierId, Guid itemId, decimal price, string createdById, DateTime createdAt)
        => new SupplierItemCostPrice(id, supplierId, itemId, price, createdById, createdAt);

    }
}

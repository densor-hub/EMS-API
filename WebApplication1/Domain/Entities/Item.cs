using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Item : BaseEntity
    {
        public string Code { get;  private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ItemsCategory Category { get; private set; }
        public UnitOfMeasure UnitOfMeasure { get; private set; }
        public int QuanityInUnit { get; private set; }
        public decimal SellingPrice { get; private set; }
        public decimal CostPrice { get; private set; }
        public int ReorderLevel { get; private set; }
        public bool Status { get; private set; }
        public Guid CompanyId { get; private set; }
        public   Company Company { get; private set; }
        // Foreign keys
        public virtual StockLevel StockLevel { get; set; }
        public ICollection<ItemLocation> ItemLocations { get; private set; }
        public ICollection<TransactionItemDelivered> TransactionItemsDelivered { get; private set; }
        public virtual ICollection<TransactionItem> TransactionItems { get; private set; }
        public virtual ICollection<StockTakingItem> StockTakingItems { get; private set; }
        public virtual ICollection<StockTransferItem> StockTransferItems { get; private set; }
        


        public Item()
        {
            
        }

        private  Item(Guid id, string code, string name, string description, ItemsCategory category, UnitOfMeasure unit, int quantityInUnit,
            decimal sellingPrice, decimal costPrice, int reorderLevel, bool status,  DateTime createdAt,Guid createdBy, Guid companyId )
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            Category = category;
            UnitOfMeasure = unit;
            QuanityInUnit = quantityInUnit;
            SellingPrice = sellingPrice;
            CostPrice = costPrice;
            ReorderLevel = reorderLevel;
            Status = status;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            CompanyId = companyId;
        }

        public static Item Create(Guid id, string code, string name, string description, ItemsCategory category, UnitOfMeasure unit, int quantityInUnit,
            decimal sellingPrice, decimal costPrice, int reorderLevel, bool status,DateTime createdAt, Guid createdBy, Guid companyId)
        => new Item(id, code, name, description, category, unit, quantityInUnit, sellingPrice, costPrice, reorderLevel, status, createdAt, createdBy, companyId);

        public void Update(string code, string name, string description, ItemsCategory category, UnitOfMeasure unit, int quantityInUnit,
            decimal sellingPrice, decimal costPrice, int reorderLevel, bool status, DateTime updatedAt, Guid uodatedBy)
        {
            Code = code;
            Name = name;
            Description = description;
            Category = category;
            UnitOfMeasure = unit;
            QuanityInUnit = quantityInUnit;
            SellingPrice = sellingPrice;
            CostPrice = costPrice;
            ReorderLevel = reorderLevel;
            Status = status;
            UpdatedAt = updatedAt;
            UpdatedBy = uodatedBy;
        }

        public void SoftDelete(Guid deletedBy)
        {
            Status = false;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = deletedBy;
        }
    }

}

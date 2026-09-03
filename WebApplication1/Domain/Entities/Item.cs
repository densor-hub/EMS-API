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
        public int QuantityInUnit { get; private set; }
        public decimal SellingPrice { get; private set; }
        public decimal? CostPrice { get; private set; } = 0;
        public int ReorderLevel { get; private set; }
        public bool Status { get; private set; }
        public Guid CompanyId { get; private set; }
        public   Company Company { get; private set; }
        // Foreign keys
        public Guid? CreatedAtLocationId { get; private set; }
        public int IncrementalId { get; private set; }
        public virtual Location CreatedAtLocation { get; private set; }
        public virtual ICollection<StockLockDownItem> StockLockDownItems { get; private set; }
        public ICollection < StockLevel> StockLevel { get; private set; }

        public ICollection<ItemLocation> ItemLocations { get; private set; }
        //public ICollection<TransactionItemDelivered> TransactionItemsDelivered { get; private set; }
        public virtual ICollection<TransactionItem> TransactionItems { get; private set; }
        public virtual ICollection<SupplierItemCostPrice> SupplierItemCostPrices { get; private set; }




        private Item()
        {
            
        }

        private  Item(Guid id, string code, string name, string description, ItemsCategory category, UnitOfMeasure unit, int quantityInUnit,
            decimal sellingPrice, int reorderLevel, bool status,  DateTime createdAt,Guid createdBy, Guid companyId, Guid createAtLocation, decimal? costPrice)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
            Category = category;
            UnitOfMeasure = unit;
            QuantityInUnit = quantityInUnit;
            SellingPrice = sellingPrice;
            CostPrice = costPrice;
            ReorderLevel = reorderLevel;
            Status = status;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            CompanyId = companyId;
            CreatedAtLocationId = createAtLocation;
        }

        public static Item Create(Guid id, string code, string name, string description, ItemsCategory category, UnitOfMeasure unit, int quantityInUnit,
            decimal sellingPrice,  int reorderLevel, bool status,DateTime createdAt, Guid createdBy, Guid companyId, Guid createAtLocation, decimal? costPrice)
        => new Item(id, code, name, description, category, unit, quantityInUnit, sellingPrice, reorderLevel, status, createdAt, createdBy, companyId, createAtLocation, costPrice);

        public void Update(string code, string name, string description, ItemsCategory category, UnitOfMeasure unit, int quantityInUnit,
            decimal sellingPrice, int reorderLevel, bool status, DateTime updatedAt, Guid uodatedBy)
        {
            Code = code;
            Name = name;
            Description = description;
            Category = category;
            UnitOfMeasure = unit;
            QuantityInUnit = quantityInUnit;
            SellingPrice = sellingPrice;
            ReorderLevel = reorderLevel;
            Status = status;
            UpdatedAt = updatedAt;
            UpdatedBy = uodatedBy;
        }

        public void SoftDelete(Guid deletedBy)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = deletedBy;
        }
    }

}

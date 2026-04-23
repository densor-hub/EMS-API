namespace WebApplication1.Domain.Entities
{
    public class StockTakingItem : BaseEntity
    {
        public Guid StockTakingId { get; private set; }
        public virtual StockTaking StockTaking { get; private set; }
        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }
        public int ExpectedQuantity { get; private set; }
        public int ActualQuantity { get; private set; }
        public int Variance { get; private set; }
        public int VerifiedQuantity { get; private set; }
      
        private StockTakingItem()
        {
            
        }

        private StockTakingItem(Guid id, int expectedQuantity, int actualQuantity, int variance, Guid stockTakeId, Guid itemId, DateTime createdAt, Guid createdBy )
        {
            Id = id;
            ExpectedQuantity = expectedQuantity;
            ActualQuantity = actualQuantity;
            Variance = variance;
            StockTakingId = stockTakeId;
            ItemId = itemId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static StockTakingItem Create(Guid id, int expectedQuantity, int actualQuantity, int variance, Guid stockTakeId, Guid itemId, DateTime createdAt, Guid createdBy)
        => new StockTakingItem(id, expectedQuantity, actualQuantity, variance, stockTakeId, itemId, createdAt, createdBy);

        public void Verify(int verifiedQuantity, DateTime verifiedAt, Guid verifiedBy)
        {
            VerifiedQuantity = verifiedQuantity;
            UpdatedAt = verifiedAt;
            UpdatedBy = verifiedBy;
        }

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = Enums.GeneralStatus.SoftDeleted;
            UpdatedAt = updatedAt;
            UpdatedBy = updatedBy;
        }
    }
}

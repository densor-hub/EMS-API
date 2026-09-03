namespace WebApplication1.Domain.Entities
{
    public class StockLockDownItem : BaseEntity
    {
        public new Guid Id { get; private  set; }
        public Guid ItemId { get; private  set; }
        public Item Item { get; private set; }
        public Guid StockLockDownRequestId { get; private set; }
        public bool? IsEscalated { get; private set; } = false;
        public StockLockDownRequest StockLockDownRequest { get; private set; }
        public ICollection<StockTakeItemSubmission> Submissions { get; private set; }

        private StockLockDownItem()
        {
            
        }
        private StockLockDownItem(Guid id, Guid itemId, Guid stockLockDownRequestId, Guid createdBy, DateTime createdAt )
        {
            Id = id;
            ItemId = itemId;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
            StockLockDownRequestId = stockLockDownRequestId;
        }

        public static StockLockDownItem Create(Guid id, Guid itemId, Guid stockLockDownRequestId, Guid createdBy, DateTime createdAt)
        => new StockLockDownItem(id, itemId, stockLockDownRequestId,createdBy, createdAt);

        public void Escalate()
        {
            IsEscalated = true;
        }
    }
}

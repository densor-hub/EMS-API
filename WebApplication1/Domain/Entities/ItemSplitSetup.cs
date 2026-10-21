using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class ItemSplitSetup
    {
        public Guid Id { get; private set; }
        public Guid SourceItemId { get; private set; }
        public Item SourceItem { get; private set; } = null!;
        public Guid TargetItemId { get; private set; }
        public Item TargetItem { get; private set; } = null!;
        public int SourceQuantity { get; private set; } = 1;
        public int TargetQuantity { get; private set; }
        public string CreatedById { get; private set; } = null!;
        public ApplicationUser CreatedBy { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public string? UpdatedById { get; private set; }
        public ApplicationUser? UpdatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public GeneralStatus GeneralStatus { get; private set; }
        public virtual ICollection<ItemSplitTransaction> ItemSplitTransactions { get; private set; }
            = new List<ItemSplitTransaction>();

        private ItemSplitSetup() { }

        private ItemSplitSetup(
            Guid id,
            Guid sourceItemId,
            Guid targetItemId,
            int sourceQty,
            int targetQty,
            string createdById,
            DateTime createdAt)
        {
            Id = id;
            SourceItemId = sourceItemId;
            TargetItemId = targetItemId;
            SourceQuantity = sourceQty;
            TargetQuantity = targetQty;
            CreatedById = createdById;
            CreatedAt = createdAt;
            GeneralStatus = GeneralStatus.Active;
        }

        public static ItemSplitSetup Create(
            Guid id,
            Guid sourceItemId,
            Guid targetItemId,
            int sourceQty,
            int targetQty,
            string createdById,
            DateTime createdAt)
            => new(id, sourceItemId, targetItemId, sourceQty, targetQty, createdById, createdAt);

        public void Update(
            int targetQty,
            string updatedByUserId,
            DateTime updatedAt,
            GeneralStatus generalStatus)
        {
            TargetQuantity = targetQty;
            UpdatedById = updatedByUserId;
            UpdatedAt = updatedAt;
            GeneralStatus = generalStatus;
        }
    }
}
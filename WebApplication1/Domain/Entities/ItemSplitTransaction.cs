namespace WebApplication1.Domain.Entities
{
    public class ItemSplitTransaction
    {
        public Guid Id { get; private set; }
        public Guid ItemSplitSetUpId { get; private set; }
        public ItemSplitSetup ItemSplitSetup { get; private set; } = null!;

        public Guid LocationId { get; private set; }
        public Location Location { get; private set; } = null!;

        public string CreatedById { get; private set; } = null!;
        public ApplicationUser CreatedBy { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }

        // Snapshots — these capture state at transaction time so history
        // is not affected by later edits to ItemSplitSetup or Item.
        public string SourceItemNameSnapShot { get; private set; } = null!;
        public string TragetItemNameSnapShot { get; private set; } = null!;   // (keep typo for DB compat if you want; else rename)
        public int TargetItemQuantitySnapShot { get; private set; }

        private ItemSplitTransaction() { }

        private ItemSplitTransaction(
            Guid id,
            Guid itemSplitSetUpId,
            Guid locationId,
            string createdById,
            DateTime createdAt,
            string sourceItemNameSnapShot,
            string targetItemNameSnapShot,
            int targetItemQtySnapShot)
        {
            Id = id;
            ItemSplitSetUpId = itemSplitSetUpId;
            LocationId = locationId;
            CreatedById = createdById;
            CreatedAt = createdAt;
            SourceItemNameSnapShot = sourceItemNameSnapShot;
            TragetItemNameSnapShot = targetItemNameSnapShot;
            TargetItemQuantitySnapShot = targetItemQtySnapShot;
        }

        public static ItemSplitTransaction Create(
            Guid id,
            Guid itemSplitSetUpId,
            Guid locationId,
            string createdById,
            DateTime createdAt,
            string sourceItemNameSnapShot,
            string targetItemNameSnapShot,
            int targetItemQtySnapShot)
            => new(
                id,
                itemSplitSetUpId,
                locationId,
                createdById,
                createdAt,
                sourceItemNameSnapShot,
                targetItemNameSnapShot,
                targetItemQtySnapShot);
    }
}
namespace WebApplication1.Domain.Entities
{
    public class ItemLocation : BaseEntity
    {
        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }
        public Guid LocationId { get; private set; }
        public virtual Location Location { get; private set; }
        public bool Status { get; private set; }

        public ItemLocation()
        {
            
        }
        public ItemLocation(Guid id, Guid locationId, Guid itemId, DateTime createdAt, Guid createdBy, bool status )
        {
            Id = id;
            LocationId = locationId;
            ItemId = itemId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Status = status;
        }

        public static ItemLocation Create(Guid id, Guid locationId, Guid itemId, DateTime createdAt, Guid createdBy, bool status)
            => new ItemLocation(id, locationId, itemId, createdAt, createdBy, status);


        public void RemoveAccess()
        {
            Status = false;
        }
        public void ActivateAccess()
        {
            Status = true;
        }
    }
}

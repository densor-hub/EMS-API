namespace WebApplication1.Domain.Entities
{
    public class LocationSaleSequence : BaseEntity
    {
        public Guid LocationId { get; private set; }
        public int LastSaleIncremental { get; private set; }

        private LocationSaleSequence() { }

        private LocationSaleSequence(Guid id, Guid locationId, int last, DateTime createdAt, Guid createdBy)
        {
            Id = id;
            LocationId = locationId;
            LastSaleIncremental = last;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static LocationSaleSequence Create(Guid id, Guid locationId, DateTime createdAt, Guid createdBy)
            => new(id, locationId, 0, createdAt, createdBy);

        public int NextSaleIncremental()
        {
            LastSaleIncremental++;
            return LastSaleIncremental;
        }
    }
}
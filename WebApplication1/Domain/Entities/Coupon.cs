namespace WebApplication1.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; private set; }
        public decimal Amount { get; private set; }
        public bool Used { get; private set; } = false;
        public DateTime? ExpiryDate { get; private set; }
        public bool Status { get; private set; }
        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        
       

        private Coupon(Guid id, string code, decimal amount, bool status, DateTime? expiryDate, Guid createdBy, Guid locationId )
        {
            Id = id;
            Code = code;
            Status = status;
            ExpiryDate = expiryDate;
            CreatedAt = DateTime.UtcNow;
            Used = false;
            Amount = amount;
            CreatedBy = createdBy;
            LocationId = locationId;
        }

        public static Coupon Create(Guid id, string code, decimal amount, bool expires, DateTime? expiryDate, Guid createdBy, Guid locationId)
        => new Coupon(id, code, amount, expires, expiryDate, createdBy, locationId);

        public void Use(Guid updatedBy)
        {
            Used = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
            
        }

        public void Update (DateTime? expiryDate, decimal amount, bool status)
        {
            Amount = amount;
            ExpiryDate = expiryDate;
            Status = status;
        }


    }
}

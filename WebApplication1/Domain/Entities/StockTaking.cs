using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockTaking : BaseEntity
    {
      
    
        public StockTakeStatus Stage { get; private set; } // InProgress, Completed, Verified
        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        public string ConductedBy { get; private set; }
        public virtual ApplicationUser Conductor { get; private set; }
        public string? VerifiedBy { get; private set; }
        public virtual ApplicationUser Verifier { get; private set; }
        public DateTime StockTakingDate { get; private set; }
        public DateTime? NextStockTakingDate { get; private set; } = null;
        public string Notes { get; private set; }
        public virtual ICollection<StockTakingItem> StockTakingItems { get; private set; }

        private StockTaking()
        {
            
        }

        private StockTaking(Guid id, Guid locationId, string conductedBy, DateTime stockTakeDate, 
            DateTime? nextStockTakeDate, string note, DateTime createAt, Guid createdBy, StockTakeStatus stage)
        {
            Id = id;
            LocationId = locationId;
            ConductedBy = conductedBy;
            StockTakingDate = stockTakeDate;
            NextStockTakingDate = nextStockTakeDate;
            Notes = note;
            CreatedAt = createAt;
            CreatedBy = createdBy;
            Stage = stage;
        }

        public static StockTaking Create(Guid id, Guid locationId, string conductedBy, DateTime stockTakeDate,
            DateTime? nextStockTakeDate, string note, DateTime createAt, Guid createdBy, StockTakeStatus status)
        => new StockTaking(id, locationId, conductedBy, stockTakeDate, nextStockTakeDate, note, createAt, createdBy, status);

        public void VerifyStock (StockTakeStatus stage, Guid verifiedBy, DateTime updatedAt, Guid updatedBy)
        {
            Stage =stage;
            VerifiedBy = verifiedBy.ToString();
            UpdatedAt= updatedAt;
            UpdatedBy = updatedBy;  
        }

        public void Restart()
        {
            Stage = StockTakeStatus.Initiated;
        }

        public void SoftDelete(Guid updatedBy, DateTime updatedAt)
        {
            GeneralStatus = GeneralStatus.SoftDeleted;
            UpdatedAt = updatedAt;
            UpdatedBy= updatedBy;
        }
    }

}

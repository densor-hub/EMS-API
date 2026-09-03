using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class StockTakeItemSubmission
    {
        public Guid Id { get; private  set; }
        public Guid StockLockDownItemId { get; private  set; }
        public StockLockDownItem StockLockDownItem { get; private set; } // Required
        public StocktakeSubmissionStatusEnum Status { get; private  set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedById { get; private  set; } // Guid converted to string
        public ApplicationUser CreatedByUser { get; private set; }
        public string? VerifiedById { get; private set; } // Guid converted to string
        public virtual ApplicationUser VerifiedByByUser { get; private set; }
        public int SystemAvailableQuantity { get; private  set; }
        public int PhysicalUnitOfMeasureQuantity { get; private  set; }
        public int QuanityPerUnitOfMeasure { get; private set; }    
        public int PhysicalAdditionalPiecesQuantity { get; private set; }
        public int Variance { get; private set; }
        public DateTime? ApprovedAt { get; private set; }
        


        public StockTakeItemSubmission()
        {
            
        }

        private StockTakeItemSubmission(Guid id, Guid stockLockdownItemId, StocktakeSubmissionStatusEnum status, DateTime createdAt, string createdBy, int systemAvailableQuantity, int physicalUinitOfMeasureQty, int quantityPerUnitOfMeasure, int physicalPiecesQty, int variance)
        {
            Id = id;
            StockLockDownItemId = stockLockdownItemId;
            Status = status;
            CreatedAt = createdAt;
            CreatedById = createdBy;
            SystemAvailableQuantity = systemAvailableQuantity;
            PhysicalUnitOfMeasureQuantity = physicalUinitOfMeasureQty;
            QuanityPerUnitOfMeasure = quantityPerUnitOfMeasure;
            PhysicalAdditionalPiecesQuantity = physicalPiecesQty;
            Variance = variance;
        }


        public static StockTakeItemSubmission Create(Guid id, Guid stockLockdownItemId, StocktakeSubmissionStatusEnum status, DateTime createdAt, string createdBy, int systemAvailableQuantity, int physicalUinitOfMeasureQty, int quantityPerUnitOfMeasure, int physicalPiecesQty, int variance)
        => new StockTakeItemSubmission(id, stockLockdownItemId, status, createdAt, createdBy, systemAvailableQuantity, physicalUinitOfMeasureQty,  quantityPerUnitOfMeasure, physicalPiecesQty, variance);

       
        public void Approve( string userId)
        {
            VerifiedById = userId;
            ApprovedAt = DateTime.UtcNow;
            Status = StocktakeSubmissionStatusEnum.Approved;

        }
        public void Decline(string userId)
        {
            VerifiedById = userId;
            ApprovedAt = DateTime.UtcNow;
            Status = StocktakeSubmissionStatusEnum.Declined;

        }

    }
}

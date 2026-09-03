using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    // DTO for grouped response

   
    public class CreateStockLockDownRequestDto
    {
        public Guid LocationId { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<Guid> ItemIds { get; set; }
        public DateTime? TurnAroundTime { get; set; } = null;
    }

    public class GetStockLockDownQuery
    {
        public Guid LocationId { get; set; }
        public DateTime? StartDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;

    }

    public class GetAllStockLockDownDto
    {
        
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime? TurnAroundTime { get; set; } = null;
        public DateTime SystemCreationDate { get; set; }
    }

    public class GetStockLockDownByIdDto
    {

        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime? TurnAroundTime { get; set; } = null;
        public DateTime SystemCreationDate { get; set; }
         public List<StockLockDownItemDto> Items { get; set; }
    }





    public class StockLockDownItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string UnitOfMeasure { get; set; }
        public int QuantityPerUnitOfMeasure { get; set; }
        public string ItemCode { get; set; }
        public bool IsEscalated { get; set; }
        public List<GetStockLockDownSubmittedItemDto> Submissions { get; set; }
    }

    public class GetStockLockDownSubmittedItemDto
    {
        public Guid Id { get; set ; }
        public int? SystemAvailableQuantity { get; set; }
        public int? TotalInPieces { get; set; }
        public int? PhysicalUnitOfMeasureQuantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public int? QuantityPerUnitOfMeasure { get; set; }
        public int? PhysicalAdditionalPiecesQuantity { get; set; }
        public int? TotalVariance { get; set; }
        public string Status { get; set; } //StocktakeSubmissionStatusEnum converted to string
        public string SubmittedBy { get; set; }
    }

    // DTO for creating and updating submission
    public class CreateStockSubmissionDto
    {

        public Guid StockLockDownItemId { get; set; }
        public int PhysicalUnitOfMeasureQuantity { get; set; }
        public int PhysicalAdditionalPiecesQuantity { get; set; }
        public string Remarks { get; set; }
    }
    public class UpdateStockTakeSubmissionDto
    {
        public Guid SubmissionId { get; set; }
        public StocktakeSubmissionStatusEnum status { get; set; }
        public string Remarks { get; set; }
    }
}

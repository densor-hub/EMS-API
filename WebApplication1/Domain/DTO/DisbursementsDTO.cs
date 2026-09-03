// DTOs/DisbursementDtos.cs
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateDisbursementDto
    {
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool RequiresExternalApproval { get; set; }
        public Guid LocationId { get; set; }
        public string Remark { get; set; }  
        public string CurrencyCode { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Discount { get; set; }
    }

    public class UpdateDisbursementDto
    {
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DisbursementType? Type { get; set; }
    }

    public class DisbursementResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TypeName { get; set; }
        public GeneralStatus Status { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class DisbursementSummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<DisbursementType, decimal> AmountByType { get; set; }
        public Dictionary<DisbursementType, int> CountByType { get; set; }
    }

    public class SubmitExternalResponseDto
    {
        public Guid TransactionId { get; set; }
        public bool Status { get; set; }
        public string Comments { get; set; }
        public string Token { get; set; }
        public TransactionType transactionType {get; set; }


    }
}
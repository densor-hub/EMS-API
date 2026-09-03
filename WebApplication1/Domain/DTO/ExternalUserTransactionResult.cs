namespace WebApplication1.Domain.DTO
{
    public class ExternalUserTransactionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Guid? TransactionId { get; set; }
        public string NewStatus { get; set; }
    }
}

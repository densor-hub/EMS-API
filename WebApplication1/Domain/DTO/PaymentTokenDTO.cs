using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.DTO
{
    public class TokenConfiguration
    {
        public int? TokenExpirationMinutes { get; set; }
        public string BaseUrl { get; set; }
    }
    public class ApprovalTokenResult
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string ApprovalUrl { get; set; }
    }

    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public Guid? PaymentId { get; set; }
       // public Disbursement Disbursement { get; set; }
    }
}

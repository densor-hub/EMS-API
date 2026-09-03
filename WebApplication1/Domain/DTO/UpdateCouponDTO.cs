namespace WebApplication1.Domain.DTO
{
    public class UpdateCouponDTO
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal Amount { get; set; }
    }
}

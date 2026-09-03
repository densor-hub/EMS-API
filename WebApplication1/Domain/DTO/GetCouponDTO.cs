namespace WebApplication1.Domain.DTO
{
    public class GetCouponDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public bool Used { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}

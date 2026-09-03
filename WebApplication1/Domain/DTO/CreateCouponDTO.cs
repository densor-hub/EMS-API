namespace WebApplication1.Domain.DTO
{
    public class CreateCouponDTO
    {
        public List<CreateCouponDTOContent>? CouponAmounts { get; set; }
        public Guid LocationId { get; set; }
    }

    public class CreateCouponDTOContent
    {
        public decimal Amount { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}

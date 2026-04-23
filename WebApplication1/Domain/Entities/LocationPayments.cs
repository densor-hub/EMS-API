using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class LocationPayments : BaseEntity
    {
        public ShopPaymentType Type { get; set; } //salary, stipent
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public Guid ReceiverId { get; set; }
        public  Employee Receiver { get; set; }
        public string Note { get; set; }
       
    }
}

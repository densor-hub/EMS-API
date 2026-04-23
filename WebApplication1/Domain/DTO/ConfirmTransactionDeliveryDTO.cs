namespace WebApplication1.Domain.DTO
{
    public class ConfirmTransactionDeliveryDTO
    {
        public Guid LocationId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Transportation { get; set; }
        public List<ConfirmTransactionDeliveryDTOItems>? Items {get; set;}
    }

    public class ConfirmTransactionDeliveryDTOItems
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
     }
}

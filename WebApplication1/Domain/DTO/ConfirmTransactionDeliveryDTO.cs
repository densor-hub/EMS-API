using System.ComponentModel.DataAnnotations;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class ConfirmTransactionDeliveryDTO
    {
        public Guid LocationId { get; set; }
        public Guid TransactionId { get; set; }
       // public Guid BatchId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public decimal? Transportation { get; set; }
        public List<ConfirmTransactionDeliveryDTOItems>? Items {get; set;}
    
    }

    public class ConfirmTransactionDeliveryDTOItems
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal? UnitPrice { get; set; }
     }
}

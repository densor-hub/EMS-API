// DTOs/Sale/SaleCancellationDto.cs
namespace WebApplication1.DTOs
{
    public class SaleCancellationDto
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public string TransactionNumber { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }

    public class CreateSaleCancellationDto
    {
        public Guid SaleId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public Guid CancelledBy { get; set; }
    }
}
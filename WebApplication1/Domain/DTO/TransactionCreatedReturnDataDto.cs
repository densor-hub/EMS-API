namespace WebApplication1.Domain.DTO
{
    public class TransactionCreatedReturnDataDto
    {
        public string QrCode { get; set; }
        public string TransactionNumber { get; set; }
        public int Count { get; set; }
    }
}

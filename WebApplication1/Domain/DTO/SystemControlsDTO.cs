namespace WebApplication1.Domain.DTO
{
    public class UpdateSettingsDTO
    {
        public Guid LocationId {  get; set;}
        public bool? SaleToDeliveryMode { get;  set; }
        public bool? RecordingOldTransactions { get;  set; }
        public int? MinimumSalePaymentPercentage { get;  set; }
        public int? PosPrinterSize { get;  set; } 
        public int? DefaultPageSize { get;  set; }
        public string? Currency { get;  set; }
    }
}

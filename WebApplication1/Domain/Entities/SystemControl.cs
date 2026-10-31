namespace WebApplication1.Domain.Entities
{
    public class SystemControl
    {
        public Guid Id { get; private set; }
        public Guid? LocationId { get; private set; }
        public virtual Location? Location { get; private set; }
        public string? UpdatedById { get; private set; }
        public ApplicationUser? UpdatedBy { get; private set; }
        public bool? SaleToDeliveryMode { get; private set; } = true;
        public bool? RecordingOldTransactions { get; private set; } = false;
        public int? MinimumSalePaymentPercentage { get; private set; } = 50;
        public int? PosPrinterSize { get; private set; } = 57;
        public int? DefaultPageSize { get; private set; } = 10;
        public string? Currency { get; private set; } = "GHS";

        private SystemControl()
        {
        }

        private SystemControl(
            Guid id,
            Guid? locationId,
            bool? saleToDeliveryMode,
            bool? recordingOldTransactions,
            int? minimumSalePaymentPercentage,
            int? posPrinterSize,
            int? defaultPageSize,
            string currency,
            string createdBy)
        {
            Id = id;
            LocationId = locationId;
            SaleToDeliveryMode = saleToDeliveryMode;
            RecordingOldTransactions = recordingOldTransactions;
            MinimumSalePaymentPercentage = minimumSalePaymentPercentage;
            PosPrinterSize = posPrinterSize;
            DefaultPageSize = defaultPageSize;
            Currency = currency;
            UpdatedById = createdBy;
        }

        public static SystemControl Create(
            Guid id,
            Guid? locationId,
            bool? saleToDeliveryMode,
            bool? recordingOldTransactions,
            int? minimumSalePaymentPercentage,
            int? posPrinterSize,
            int? defaultPageSize,
            string currency,
            string createdBy
            )
            => new SystemControl(
                id,
                locationId,
                saleToDeliveryMode,
                recordingOldTransactions,
                minimumSalePaymentPercentage,
                posPrinterSize,
                defaultPageSize,
                currency, 
                createdBy);

        public void Update(
            bool? saleToDeliveryMode,
            bool? recordingOldTransactions,
            int? minimumSalePaymentPercentage,
            int? posPrinterSize,
            int? defaultPageSize,
            string currency,
            string updatedByUser)
        {
            SaleToDeliveryMode = saleToDeliveryMode;
            RecordingOldTransactions = recordingOldTransactions;
            MinimumSalePaymentPercentage = minimumSalePaymentPercentage;
            PosPrinterSize = posPrinterSize;
            DefaultPageSize = defaultPageSize;
            Currency = currency;
            UpdatedById = updatedByUser;
        }
    }
}
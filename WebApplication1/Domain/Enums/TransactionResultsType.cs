namespace WebApplication1.Domain.Enums
{
    public enum TransactionResultsType
    {
        Deposit = 1,
        Debit = 2,

        //These are just to specify items for emails
        SaleDelivery=-1,
        StockTransDelivery=-2,
        PurchaseReceival=-3,
        Reversal=-4,
        Creation = -3,
        TransferReceival =-4,
        TransferReversal = -5,
        FinancialServiceProviderDisbursement=-6,
        EmployeeDisbursement = -7
    }

    
}

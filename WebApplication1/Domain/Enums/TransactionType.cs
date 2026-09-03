namespace WebApplication1.Domain.Enums
{
    public enum TransactionType
    {
        SALE=1, //sale
        PURC =2, //purchase
        STOC=3, //stock lock
        TRAN=4, // stock transfer
        DEPO=5, // financial deposit
        SLP=6, //salary payment
        MISC=7, //miscellaneous
        SREV=8, //sale reversal
        PREV=9, //purchase reversal
        TREV = 10,
        GIFT =11, // giftings
    }
}

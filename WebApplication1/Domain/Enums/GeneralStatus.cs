namespace WebApplication1.Domain.Enums
{
    public enum GeneralStatus
    {
        Inactive = 0, Cancelled = 0, Initiated =0,

        Active = 1,
        SoftDeleted = -1,
       
        PendingCompletionOrDelivery = 2,
        Declined = -2,

        Approved = 3,
        Committed = 4
    }
}

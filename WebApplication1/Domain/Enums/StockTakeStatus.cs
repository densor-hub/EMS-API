namespace WebApplication1.Domain.Enums
{
    public enum StockTakeStatus
    {
        Initiated,
        Approved,
        Declined, 
        Completed,
        Revoked //after it has been approved but receiver revokes it because of difference in quantity
    }
}

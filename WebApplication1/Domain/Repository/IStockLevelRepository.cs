using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IStockLevelRepository
    {
        Task<StockLevel?> GetByItemAndLocationAsync(
            Guid itemId,
            Guid locationId,
            CancellationToken ct = default);
    }
}

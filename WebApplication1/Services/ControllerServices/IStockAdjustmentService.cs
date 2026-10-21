using WebApplication1.Domain.DTO;

using WebApplication1.Controllers;

namespace WebApplication1.Services.ControllerServices
{
    public interface IStockAdjustmentService
    {
        Task<Guid> CreateAsync(
            CreateStockAdjustmentRequest request,
            CancellationToken ct = default);

        Task<IReadOnlyList<StockAdjustmentDto>> GetAllAsync(
            CancellationToken ct = default);

        Task<StockAdjustmentDto?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        /// <summary>
        /// Approves or declines a pending adjustment.
        /// Approving adds <see cref="StockAdjustment.Quantity"/> to the
        /// item's available and actual balances at the adjustment's location.
        /// Declining marks the adjustment inactive without touching stock.
        /// </summary>
        Task<bool> VerifyAsync(
            Guid id,
            VerifyStockAdjustmentRequest request,
            CancellationToken ct = default);
    }
}

using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.DAL.Repositories;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IStockAdjustmentRepository _repo;
        private readonly IStockLevelRepository _stockRepo;
        private readonly IUserRepository _userRepo;

        public StockAdjustmentService(
            IStockAdjustmentRepository repo,
            IStockLevelRepository stockRepo,
            IUserRepository userRepo)
        {
            _repo = repo;
            _stockRepo = stockRepo;
            _userRepo = userRepo;
        }

        // ------------------------------------------------------------
        // 1. CREATE
        // ------------------------------------------------------------
        public async Task<Guid> CreateAsync(
            CreateStockAdjustmentRequest request,
            CancellationToken ct = default)
        {
            var user = await _userRepo.GetUserByRefreshTokenAsync()
                ?? throw new UnauthorizedAccessException("Unauthorized.");

            if (request.ItemId == Guid.Empty)
                throw new ArgumentException("ItemId is required.");
            if (request.LocationId == Guid.Empty)
                throw new ArgumentException("LocationId is required.");
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.");
            if (request.TransactionDate > DateTime.UtcNow.AddDays(1))
                throw new ArgumentException(
                    "Transaction date cannot be more than one day in the future.");

            var entity = StockAdjustment.Create(
                id: Guid.NewGuid(),
                itemId: request.ItemId,
                locationId: request.LocationId,
                quantity: request.Quantity,
                supplierId: request.SupplierId,
                createdAt: DateTime.UtcNow,
                transactionDate: request.TransactionDate,
                createdById: user.Id);

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return entity.Id;
        }

        // ------------------------------------------------------------
        // 2. GET ALL
        // ------------------------------------------------------------
        public async Task<IReadOnlyList<StockAdjustmentDto>> GetAllAsync(
            CancellationToken ct = default)
        {
            return await _repo.GetAll()
                .Where(x => x.GeneralStatus != GeneralStatus.SoftDeleted)
                .OrderByDescending(x => x.TransactionDate)
                .Select(x => new StockAdjustmentDto(
                    x.Id,
                    x.ItemId,
                    x.Item.Name,
                    x.LocationId,
                    x.Location.Name,
                    x.Quantity,
                    x.SupplierId,
                   x.Supplier != null ? $"{x.Supplier.FirstName} {x.Supplier.LastName}" : null,
                    x.CreatedAt,
                    x.TransactionDate,
                    x.CreatedById,
                    x.GeneralStatus,
                    x.ApprovedById,
                    x.ApprovedAt,
                    x.GeneralStatus.ToString()))
                .ToListAsync(ct);
        }

        // ------------------------------------------------------------
        // 3. GET BY ID
        // ------------------------------------------------------------
        public async Task<StockAdjustmentDto?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return await _repo.GetAll()
                .Where(x => x.Id == id && x.GeneralStatus != GeneralStatus.SoftDeleted)
                .Select(x => new StockAdjustmentDto(
                    x.Id,
                    x.ItemId,
                    x.Item.Name,
                    x.LocationId,
                    x.Location.Name,
                    x.Quantity,
                    x.SupplierId,
                    x.Supplier != null ? $"{x.Supplier.FirstName} {x.Supplier.LastName}" : null,
                    x.CreatedAt,
                    x.TransactionDate,
                    x.CreatedById,
                    x.GeneralStatus,
                    x.ApprovedById,
                    x.ApprovedAt,
                    x.GeneralStatus.ToString()))
                .FirstOrDefaultAsync(ct);
        }

        // ------------------------------------------------------------
        // 4. VERIFY — approve (apply stock) or decline (no stock change)
        // ------------------------------------------------------------
        public async Task<bool> VerifyAsync(
            Guid id,
            VerifyStockAdjustmentRequest request,
            CancellationToken ct = default)
        {
            var user = await _userRepo.GetUserByRefreshTokenAsync()
                ?? throw new UnauthorizedAccessException("Unauthorized.");

            // Load adjustment tracked (we're going to mutate status)
            var adjustment = await _repo.GetByIdForUpdateAsync(id, ct);
            if (adjustment is null) return false;

            if (adjustment.GeneralStatus == GeneralStatus.SoftDeleted)
                throw new InvalidOperationException(
                    "Cannot verify a soft-deleted adjustment.");

            if (adjustment.ApprovedById is not null)
                throw new InvalidOperationException(
                    "Adjustment has already been verified.");

            // ---- Approve path ----
            if (request.Approve)
            {
                // Load the stock level for the item at the adjustment's location
                var stockLevel = await _stockRepo.GetByItemAndLocationAsync(
                    adjustment.ItemId,
                    adjustment.LocationId,
                    ct);

                if (stockLevel is null)
                    throw new InvalidOperationException(
                        "No stock level exists for this item at this location. " +
                        "Create a stock level before approving the adjustment.");

                // Apply the adjustment — both available and actual grow by Quantity
                stockLevel.AddAvailable(adjustment.Quantity);
                stockLevel.AddActual(adjustment.Quantity);

                adjustment.Verify(user.Id, DateTime.UtcNow, request.Status);
            }
            // ---- Decline path ----
            else
            {
                 adjustment.Verify(user.Id, DateTime.UtcNow, request.Status);
            }

            // Single SaveChanges — stock changes + status change are atomic.
            // The repo and stockRepo share the same AppDbContext (scoped per request).
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
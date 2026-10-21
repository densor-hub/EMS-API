using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.DAL.Repositories;
using WebApplication1.DAL.Repository;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class ItemSplitService : IItemSplitService
    {
        private readonly IItemSplitSetupRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IItemSplitTransactionRepository _txRepo;
        private readonly IStockLevelRepository _stockRepo;
        private readonly ITransactionCodeRepository _txCodeRepo;
        private readonly AppDbContext _context;   // ✅ needed for Database.BeginTransactionAsync

        public ItemSplitService(
            IItemSplitSetupRepository repo,
            IUserRepository userRepo,
            IItemSplitTransactionRepository txRepo,
            IStockLevelRepository stockRepo,
            ITransactionCodeRepository txCodeRepo,
            AppDbContext context)
        {
            _repo = repo;
            _userRepo = userRepo;
            _txRepo = txRepo;
            _stockRepo = stockRepo;
            _txCodeRepo = txCodeRepo;
            _context = context;
        }

        // ------------------------------------------------------------
        // 1. CREATE (setup)
        // ------------------------------------------------------------
        public async Task<Guid> CreateAsync(
            CreateItemSplitSetupRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var user = await _userRepo.GetUserByRefreshTokenAsync()
                    ?? throw new UnauthorizedAccessException("Unauthorized.");

                if (request.SourceItemId == Guid.Empty)
                    throw new ArgumentException("SourceItemId is required.");
                if (request.TargetItemId == Guid.Empty)
                    throw new ArgumentException("TargetItemId is required.");
                if (request.SourceItemId == request.TargetItemId)
                    throw new InvalidOperationException("An item cannot split into itself.");
                if (request.SourceQuantity <= 0)
                    throw new ArgumentException("SourceQuantity must be greater than 0.");
                if (request.TargetQuantity <= 0)
                    throw new ArgumentException("TargetQuantity must be greater than 0.");

                var alreadyExists = await _repo.GetAll(request.LocationId)
                    .AnyAsync(x =>
                        x.SourceItemId == request.SourceItemId &&
                        x.TargetItemId == request.TargetItemId &&
                        x.GeneralStatus == GeneralStatus.Active, ct);

                if (alreadyExists)
                    throw new InvalidOperationException(
                        "An active split rule already exists for this source/target pair.");

                var entity = ItemSplitSetup.Create(
                    id: Guid.NewGuid(),
                    sourceItemId: request.SourceItemId,
                    targetItemId: request.TargetItemId,
                    sourceQty: request.SourceQuantity,
                    targetQty: request.TargetQuantity,
                    createdById: user.Id,
                    createdAt: DateTime.UtcNow);

                await _repo.AddAsync(entity, ct);
                await _repo.SaveChangesAsync(ct);

                return entity.Id;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    "Could not create split setup due to a database constraint. " +
                    "Verify the items exist and are not already split.",
                    ex);
            }
        }

        // ------------------------------------------------------------
        // 2. GET ALL (setups)
        // ------------------------------------------------------------
        public async Task<IReadOnlyList<ItemSplitSetupDto>> GetAllAsync(Guid locationId,
            CancellationToken ct = default)
        {
            try
            {
                return await _repo.GetAll(locationId)
                    .Where(x => x.GeneralStatus != GeneralStatus.SoftDeleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new ItemSplitSetupDto(
                        x.Id,
                        x.SourceItemId,
                        x.SourceItem.Name,
                        x.TargetItemId,
                        x.TargetItem.Name,
                        x.SourceQuantity,
                        x.TargetQuantity,
                        x.GeneralStatus))
                    .ToListAsync(ct);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    "Could not load split setups.", ex);
            }
        }

        // ------------------------------------------------------------
        // 3. GET BY ID
        // ------------------------------------------------------------
        public async Task<ItemSplitSetupDto?> GetByIdAsync(
            Guid id,
            Guid locationId,
            CancellationToken ct = default)
        {
            try
            {
                return await _repo.GetAll(locationId)
                    .Where(x => x.Id == id && x.GeneralStatus != GeneralStatus.SoftDeleted)
                    .Select(x => new ItemSplitSetupDto(
                        x.Id,
                        x.SourceItemId,
                        x.SourceItem.Name,
                        x.TargetItemId,
                        x.TargetItem.Name,
                        x.SourceQuantity,
                        x.TargetQuantity,
                        x.GeneralStatus))
                    .FirstOrDefaultAsync(ct);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"Could not load split setup '{id}'.", ex);
            }
        }

        // ------------------------------------------------------------
        // 4. UPDATE (setup)
        // ------------------------------------------------------------
        public async Task<bool> UpdateAsync(
            Guid id,
            UpdateItemSplitSetupRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var user = await _userRepo.GetUserByRefreshTokenAsync()
                    ?? throw new UnauthorizedAccessException("Unauthorized.");

                if (request.TargetQuantity <= 0)
                    throw new ArgumentException("TargetQuantity must be greater than 0.");
                if (request.GeneralStatus == GeneralStatus.SoftDeleted)
                    throw new InvalidOperationException(
                        "Use DeleteAsync to soft-delete a setup.");

                var entity = await _repo.GetByIdForUpdateAsync(id, ct);
                if (entity is null) return false;

                if (entity.GeneralStatus == GeneralStatus.SoftDeleted)
                    throw new InvalidOperationException(
                        "Cannot update a soft-deleted setup.");

                entity.Update(
                    targetQty: request.TargetQuantity,
                    updatedByUserId: user.Id,
                    updatedAt: DateTime.UtcNow,
                    generalStatus: request.GeneralStatus);

                await _repo.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(
                    "Split setup was modified by another request. Reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    "Could not update split setup due to a database constraint.", ex);
            }
        }

        // ------------------------------------------------------------
        // 5. DELETE (soft)
        // ------------------------------------------------------------
        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken ct = default)
        {
            try
            {
                var user = await _userRepo.GetUserByRefreshTokenAsync()
                    ?? throw new UnauthorizedAccessException("Unauthorized.");

                var entity = await _repo.GetByIdForUpdateAsync(id, ct);
                if (entity is null) return false;

                if (entity.GeneralStatus == GeneralStatus.SoftDeleted)
                    return true;

                entity.Update(
                    targetQty: entity.TargetQuantity,
                    updatedByUserId: user.Id,
                    updatedAt: DateTime.UtcNow,
                    generalStatus: GeneralStatus.SoftDeleted);

                await _repo.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(
                    "Split setup was modified by another request. Reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    "Could not delete split setup due to a database constraint.", ex);
            }
        }

        // ------------------------------------------------------------
        // 6. CREATE TRANSACTION — moves stock + records tx atomically
        // ------------------------------------------------------------
        public async Task<Guid> CreateTransactionAsync(
            CreateItemSplitTransactionRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var user = await _userRepo.GetUserByRefreshTokenAsync()
                    ?? throw new UnauthorizedAccessException("Unauthorized.");

                // ---- Validate input ----
                if (request.SourceItemId == Guid.Empty)
                    throw new ArgumentException("SourceItemId is required.");
                if (request.TargetItemId == Guid.Empty)
                    throw new ArgumentException("TargetItemId is required.");
                if (request.SourceItemId == request.TargetItemId)
                    throw new InvalidOperationException("An item cannot split into itself.");
                if (request.LocationId == Guid.Empty)
                    throw new ArgumentException("LocationId is required.");

                // ✅ Explicit transaction — guarantees atomicity even if
                //    some future change causes multiple SaveChanges calls.
                await using var dbTx = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    // ---- 1. Find the active setup for this pair ----
                    var setup = await _repo.GetBySourceAndTargetAsync(
                        request.SourceItemId,
                        request.TargetItemId,
                        ct);

                    if (setup is null)
                        throw new InvalidOperationException(
                            "No active split setup exists for this source/target pair.");

                    // ---- 2. Source stock (tracked — we mutate) ----
                    var sourceStock = await _stockRepo.GetByItemAndLocationAsync(
                        request.SourceItemId,
                        request.LocationId,
                        ct);

                    if (sourceStock is null)
                        throw new InvalidOperationException(
                            "Source item has no stock level at this location.");

                    // ---- 3. Target stock (tracked — we mutate) ----
                    var targetStock = await _stockRepo.GetByItemAndLocationAsync(
                        request.TargetItemId,
                        request.LocationId,
                        ct);

                    if (targetStock is null)
                        throw new InvalidOperationException(
                            "Target item has no stock level at this location. " +
                            "Create the stock level first or choose a different location.");

                    // ---- 4. Apply the movement ----
                    sourceStock.SubstractAvailable(setup.SourceQuantity);
                    sourceStock.SubstractActual(setup.SourceQuantity);

                    targetStock.AddAvailable(setup.TargetQuantity);
                    targetStock.AddActual(setup.TargetQuantity);

                    // ---- 5. Record the transaction ----
                    var tx = ItemSplitTransaction.Create(
                        Guid.NewGuid(),
                        setup.Id,
                        request.LocationId,
                        user.Id,
                        DateTime.UtcNow,
                        setup.SourceItem.Name,
                        setup.TargetItem.Name,
                        setup.TargetQuantity);

                    await _txRepo.AddAsync(tx, ct);

                    // ---- 6. Single SaveChanges (all 3 entities) ----
                    await _txRepo.SaveChangesAsync(ct);

                    // ---- 7. Commit ----
                    await dbTx.CommitAsync(ct);

                    return tx.Id;
                }
                catch
                {
                    // Roll back on any failure — stock and tx both revert.
                    await dbTx.RollbackAsync(ct);
                    throw;
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(
                    "Stock level was modified by another request during the split. " +
                    "Reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    "Could not complete the split transaction due to a database constraint.", ex);
            }
        }

        // ------------------------------------------------------------
        // 7. GET ALL (transactions)
        // ------------------------------------------------------------
        public async Task<IReadOnlyList<ItemSplitTransactionDto>> GetAllTransactionsAsync(
            CancellationToken ct = default)
        {
            try
            {
                return await _txRepo.GetAll()
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new ItemSplitTransactionDto(
                        x.Id,
                        x.ItemSplitSetUpId,
                        x.ItemSplitSetup.SourceItemId,
                        x.ItemSplitSetup.SourceItem.Name,
                        x.ItemSplitSetup.TargetItemId,
                        x.ItemSplitSetup.TargetItem.Name,
                        x.ItemSplitSetup.SourceQuantity,
                        x.ItemSplitSetup.TargetQuantity,
                        x.TargetItemQuantitySnapShot,
                        x.CreatedById,
                        x.CreatedAt))
                    .ToListAsync(ct);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    "Could not load split transactions.", ex);
            }
        }

        // ------------------------------------------------------------
        // 8. GET BY ID (transaction)
        // ------------------------------------------------------------
        public async Task<ItemSplitTransactionDto?> GetTransactionByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            try
            {
                return await _txRepo.GetAll()
                    .Where(x => x.Id == id)
                    .Select(x => new ItemSplitTransactionDto(
                        x.Id,
                        x.ItemSplitSetUpId,
                        x.ItemSplitSetup.SourceItemId,
                        x.ItemSplitSetup.SourceItem.Name,
                        x.ItemSplitSetup.TargetItemId,
                        x.ItemSplitSetup.TargetItem.Name,
                        x.ItemSplitSetup.SourceQuantity,
                        x.ItemSplitSetup.TargetQuantity,
                        x.TargetItemQuantitySnapShot,
                        x.CreatedById,
                        x.CreatedAt))
                    .FirstOrDefaultAsync(ct);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"Could not load split transaction '{id}'.", ex);
            }
        }
    }
}
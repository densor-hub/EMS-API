using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DAL;
using WebApplication1.Domain.Repository;
using Microsoft.Extensions.Logging;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.Domain.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApplication1.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StockTransferController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<StockTransferController> _logger;

        public StockTransferController(
            AppDbContext context,
            ITransactionCodeRepository transactionCodeRepository,
            IItemRepository itemRepository,
            IUserRepository userRepository,
            ILogger<StockTransferController> logger)
        {
            _context = context;
            _transactionCodeRepository = transactionCodeRepository;
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Step 1: ToLocation initiates a transfer request
        /// </summary>
        [HttpPost("Request")]
        public async Task<ActionResult<StockTransfer>> CreateStockTransfer(StockTransferCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    return Unauthorized("User not found");

                // Verify items exist at both locations (they should be created in the system)
                var itemIds = dto.Items.Select(x => x.ItemId).Distinct().ToList();

                var allItemsValidAtFromLoc = await _itemRepository.AllItemsAreValid(itemIds, (Guid)dto.FromLocationId);
                if (!allItemsValidAtFromLoc)
                    return BadRequest("Some items are not valid at the source location");

                var allItemsValidAtToLoc = await _itemRepository.AllItemsAreValid(itemIds, (Guid)dto.ToLocationId);
                if (!allItemsValidAtToLoc)
                    return BadRequest("Some items are not valid at the destination location. Please ensure items are created there first.");

                // Generate transaction code
                var transactionCode = await _transactionCodeRepository.GenerateUniquePinAsync();

                var stockTransfer = StockTransfer.Create(
                    Guid.NewGuid(),
                    transactionCode,
                    DateTime.SpecifyKind((DateTime)dto.TransferDate, DateTimeKind.Utc),
                    StockTakeStatus.Initiated, // Request initiated by ToLocation
                    dto.FromLocationId,
                    dto.ToLocationId,
                    user.Id,
                    Guid.Parse(user.Id),
                    DateTime.UtcNow
                );

                // Add comment if provided
                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    var comment = Comments.Create(
                        Guid.NewGuid(),
                        stockTransfer.Id,
                        (int)StockTakeStatus.Initiated,
                        dto.Comment,
                        DateTime.UtcNow,
                        Guid.Parse(user.Id));
                    await _context.Comments.AddAsync(comment);
                }

                // Create transfer items (requested quantities)
                var transferItems = dto.Items.Select(item => StockTransferItem.Create(
                        Guid.NewGuid(),
                        item.ItemId,
                        stockTransfer.Id,
                        item.Quantity, // Requested quantity
                        DateTime.UtcNow,
                        Guid.Parse(user.Id)
                    ));

                await _context.StockTransferItems.AddRangeAsync(transferItems);

                _context.StockTransfers.Add(stockTransfer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetStockTransfer), new { id = stockTransfer.Id },
                    new { id = stockTransfer.Id, status = true, message = "Transfer request created successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating stock transfer");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Step 2: FromLocation approves and transfers the items
        /// </summary>
        [HttpPut("approve")]
        public async Task<IActionResult> ApproveStockTransfer(StockTransferApproveDto dto)
        {
            if (!Enum.IsDefined(typeof(StockTakeStatus), dto.Stage) ||
                (dto.Stage != StockTakeStatus.Approved && dto.Stage != StockTakeStatus.Declined))
                return BadRequest("Invalid status submitted. Must be Approved or Declined.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    return Unauthorized("User not found");

                var stockTransfer = await _context.StockTransfers
                    .Include(st => st.StockTransferItems)
                        .ThenInclude(x=> x.Item)
                    .FirstOrDefaultAsync(st => st.Id == dto.StockTransferId);

                if (stockTransfer == null)
                    return NotFound("Transfer request not found");

                // Verify the user is from the FromLocation (source location)
                //if (stockTransfer.FromLocationId.ToString() != user.LocationId)
                //    return BadRequest("Only users from the source location can approve transfers");

                if (stockTransfer.Status != StockTakeStatus.Initiated)
                    return BadRequest("Transfer is not in pending state");

                if (dto.Stage == StockTakeStatus.Declined)
                {
                    // Handle decline - no stock movement, just update status
                    stockTransfer.UpdateApproval(StockTakeStatus.Declined, DateTime.UtcNow, Guid.Parse(user.Id));

                    if (!string.IsNullOrWhiteSpace(dto.Comment))
                    {
                        var comment = Comments.Create(
                            Guid.NewGuid(),
                            stockTransfer.Id,
                            (int)StockTakeStatus.Declined,
                            dto.Comment,
                            DateTime.UtcNow,
                            Guid.Parse(user.Id));
                        await _context.Comments.AddAsync(comment);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(new { message = "Transfer request declined" });
                }

                // Handle approval - check stock availability and deduct
                foreach (var item in stockTransfer.StockTransferItems)
                {
                    // Check if source has enough stock
                    var sourceStock = await _context.StockLevels
                        .FirstOrDefaultAsync(sl => sl.ItemId == item.ItemId &&
                                                   sl.LocationId == stockTransfer.FromLocationId);

                    if (sourceStock == null || sourceStock.ActualQuantity < item.RequestedQuantity)
                    {
                        return BadRequest($"Insufficient stock for item {item?.Item?.Name}. Requested Qty: {item.RequestedQuantity}, Available Qty: {sourceStock?.ActualQuantity ?? 0}");
                    }

                    // Record the transferred quantity (same as requested for now - can be adjusted if needed)
                    item.UpdateAproval(item.RequestedQuantity, DateTime.UtcNow, Guid.Parse(user.Id));

                    // Deduct from source location stock
                    sourceStock.SubstractAvailable(item.RequestedQuantity);
                    sourceStock.SubstractActual(item.RequestedQuantity);

                    _context.StockLevels.Update(sourceStock);
                }

                // Update transfer status
                stockTransfer.UpdateApproval(StockTakeStatus.Approved, DateTime.UtcNow, Guid.Parse(user.Id));

                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    var comment = Comments.Create(
                        Guid.NewGuid(),
                        stockTransfer.Id,
                        (int)StockTakeStatus.Approved,
                        dto.Comment,
                        DateTime.UtcNow,
                        Guid.Parse(user.Id));
                    await _context.Comments.AddAsync(comment);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Transfer approved and items deducted from source location" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error approving stock transfer");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Step 3: ToLocation receives the transferred items
        /// </summary>
        [HttpPut("receive")]
        public async Task<IActionResult> ReceiveStockTransfer(StockTransferReceiveDto dto)
        {
            if (!Enum.IsDefined(typeof(StockTakeStatus), dto.Stage) ||
                (dto.Stage != StockTakeStatus.Completed && dto.Stage != StockTakeStatus.Revoked))
                return BadRequest("Invalid status submitted. Must be Completed or Declined.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    return Unauthorized("User not found");

                var stockTransfer = await _context.StockTransfers
                    .Include(st => st.StockTransferItems)
                    .FirstOrDefaultAsync(st => st.Id == dto.StockTransferId);

                if (stockTransfer == null)
                    return NotFound("Transfer not found");

                //// Verify the user is from the ToLocation (destination location)
                //if (stockTransfer.ToLocationId.ToString() != user.LocationId)
                //    return BadRequest("Only users from the destination location can receive transfers");

                if (stockTransfer.Status != StockTakeStatus.Approved)
                    return BadRequest("Transfer must be approved before receiving");

                // Handle revoke at receipt stage
                if (dto.Stage == StockTakeStatus.Revoked)
                {
                    // Validate confirmation code for decline
                    if (string.IsNullOrWhiteSpace(dto.ConfirmationCode))
                        return BadRequest("Confirmation code is required to decline a transfer");

                    var isValidCode = await ValidateConfirmationCode(dto.ConfirmationCode, Guid.Parse(user.Id));
                    if (!isValidCode)
                        return BadRequest("Invalid or expired confirmation code");

                    // Return items to source location stock
                    foreach (var item in stockTransfer.StockTransferItems)
                    {
                        var sourceStock = await _context.StockLevels
                            .FirstOrDefaultAsync(sl => sl.ItemId == item.ItemId &&
                                                       sl.LocationId == stockTransfer.FromLocationId);

                        if (sourceStock == null)
                        {
                            // Create stock record if it doesn't exist
                            sourceStock = StockLevel.Create(
                                Guid.NewGuid(),
                                0,
                                0,
                                item.ItemId,
                                stockTransfer.FromLocationId.Value
                            );
                            _context.StockLevels.Add(sourceStock);
                        }

                        // Add the items back to source
                        sourceStock.AddAvailable(item.TransferedQuantity);
                        sourceStock.AddActual(item.TransferedQuantity);
                    }

                    stockTransfer.UpdateApproval(StockTakeStatus.Revoked, DateTime.UtcNow, Guid.Parse(user.Id));

                    if (!string.IsNullOrWhiteSpace(dto.Comment))
                    {
                        var comment = Comments.Create(
                            Guid.NewGuid(),
                            stockTransfer.Id,
                            (int)StockTakeStatus.Declined,
                            dto.Comment,
                            DateTime.UtcNow,
                            Guid.Parse(user.Id));
                        await _context.Comments.AddAsync(comment);
                    }

                    // Mark confirmation code as used
                    await MarkConfirmationCodeAsUsed(dto.ConfirmationCode);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { message = "Transfer revoked and items returned to source" });
                }

                // Handle completion (receipt)
                bool hasVariance = false;
                bool requiresConfirmationCode = false;

                foreach (var item in stockTransfer.StockTransferItems)
                {
                    var receivedItem = dto.ReceivedItems?.FirstOrDefault(ri => ri.ItemId == item.ItemId);

                    if (receivedItem == null)
                        return BadRequest($"Received quantity not specified for item {item.ItemId}");

                    // Check for variance
                    if (receivedItem.Quantity != item.TransferedQuantity)
                    {
                        hasVariance = true;

                        // Validate confirmation code for variance
                        if (string.IsNullOrWhiteSpace(dto.ConfirmationCode))
                        {
                            requiresConfirmationCode = true;
                            break;
                        }
                    }

                    // Record received quantity and variance
                    item.UpdateReceival(receivedItem.Quantity, DateTime.UtcNow, Guid.Parse(user.Id));

                    var variance = receivedItem.Quantity - item.TransferedQuantity;
                    if (variance != 0)
                    {
                        item.UpdateVariance(variance, DateTime.UtcNow, Guid.Parse(user.Id));
                    }

                    // Add to destination stock
                    var destStock = await _context.StockLevels
                        .FirstOrDefaultAsync(sl => sl.ItemId == item.ItemId &&
                                                   sl.LocationId == stockTransfer.ToLocationId);

                    if (destStock == null)
                    {
                        destStock = StockLevel.Create(
                            Guid.NewGuid(),
                            0,
                            0,
                            item.ItemId,
                            stockTransfer.ToLocationId.Value
                        );
                        _context.StockLevels.Add(destStock);
                    }

                    destStock.AddAvailable(receivedItem.Quantity);
                    destStock.AddActual(receivedItem.Quantity);
                }

                // If variance exists and no confirmation code provided, return error
                if (requiresConfirmationCode || (hasVariance && string.IsNullOrWhiteSpace(dto.ConfirmationCode)))
                {
                    return BadRequest("Confirmation code is required when received quantities differ from transferred quantities");
                }

                // Validate confirmation code if provided (for variance)
                if (hasVariance && !string.IsNullOrWhiteSpace(dto.ConfirmationCode))
                {
                    var isValidCode = await ValidateConfirmationCode(dto.ConfirmationCode, Guid.Parse(user.Id));
                    if (!isValidCode)
                        return BadRequest("Invalid or expired confirmation code for variance approval");

                    await MarkConfirmationCodeAsUsed(dto.ConfirmationCode);
                }

                // Update transfer status
                stockTransfer.UpdateApproval(StockTakeStatus.Completed, DateTime.UtcNow, Guid.Parse(user.Id));

                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    var comment = Comments.Create(
                        Guid.NewGuid(),
                        stockTransfer.Id,
                        (int)StockTakeStatus.Completed,
                        dto.Comment,
                        DateTime.UtcNow,
                        Guid.Parse(user.Id));
                    await _context.Comments.AddAsync(comment);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var responseMessage = hasVariance
                    ? "Transfer completed with variances. Items added to destination stock."
                    : "Transfer completed successfully. Items added to destination stock.";

                return Ok(new { message = responseMessage, hasVariance });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error receiving stock transfer");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockTransfer(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    return Unauthorized("User not found");

                var stockTransfer = await _context.StockTransfers
                    .Include(st => st.StockTransferItems)
                    .FirstOrDefaultAsync(st => st.Id == id);

                if (stockTransfer == null)
                    return NotFound();

                // Can only delete if status is Initiated
                if (stockTransfer.Status != StockTakeStatus.Initiated)
                    return BadRequest("Cannot delete a transfer that is not in pending state");

                _context.StockTransferItems.RemoveRange(stockTransfer.StockTransferItems);
                _context.StockTransfers.Remove(stockTransfer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Transfer request deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock transfer");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockTransfer>> GetStockTransfer(Guid id)
        {
            try
            {
                var stockTransfer = await _context.StockTransfers
                    .Include(st => st.StockTransferItems)
                        .ThenInclude(sti => sti.Item)
                    .FirstOrDefaultAsync(st => st.Id == id);

                if (stockTransfer == null)
                    return NotFound();

                return Ok(stockTransfer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock transfer");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockTransfersDTO>>> GetStockTransferRecords([FromQuery] BrowseStockTransfersFilters filters)
        {
            try
            {
                var stockTransferQuery = _context.StockTransfers
                    .Include(st => st.StockTransferItems)
                        .ThenInclude(sti => sti.Item)
                            .ThenInclude(i => i.StockLevel)
                    .Include(st => st.StockTransferItems)
                        .ThenInclude(x => x.stockTransferItemsDelivered)
                    .Include(st => st.InitiatedBy)  // Add this
                    .Include(st => st.ApprovedBy)   // Add this
                    .Where(x => filters.From ? x.FromLocationId == filters.LocationId : x.ToLocationId == filters.LocationId);

                //if (filters.Stage == 1)
                //{
                //    stockTransferQuery = stockTransferQuery.Where(x => x.Status == StockTakeStatus.Initiated);
                //}
                //else if (filters.Stage == 2)
                //{
                //    stockTransferQuery = stockTransferQuery.Where(x => x.Status != StockTakeStatus.Initiated);
                //} else if (filters.Stage != 55) {
                //    return BadRequest("Invalid request");
                //}

                var returnData = await stockTransferQuery
                    .OrderByDescending(x=> x.CreatedAt)
                    .Select(x => new StockTransfersDTO
                    {
                        Id = x.Id,
                        SupplierId = x.FromLocationId,
                        SupplierName = x.FromLocation != null ? x.FromLocation.Name : string.Empty,
                        Status = x.Status.ToString(),
                        StatusNumber = x.Status,
                        InitiatedBy = x.InitiatedBy != null ? x.InitiatedBy.FullName : null,
                        ApprovedBy = x.ApprovedBy != null ? x.ApprovedBy.FullName : null,
                        CustomerId = x.ToLocationId,
                        CustomerName = x.ToLocation != null ? x.ToLocation.Name : string.Empty,
                        TransactionCode = x.TransactionCode,
                        TransactionDate = x.TransferDate,
                        TotalAmount = x.StockTransferItems.Sum(sti => sti.Item.CostPrice * sti.RequestedQuantity),
                        Items = x.StockTransferItems.Select(sti => new StockTransfersDTOItems
                        {
                            ActualQuantity = sti.Item.StockLevel != null ? sti.Item.StockLevel.ActualQuantity : 0,
                            AvailableQuantity = sti.Item.StockLevel != null ? sti.Item.StockLevel.AvailableQuanity : 0,
                            ItemId = sti.Item.Id,
                            ItemName = sti.Item.Name,
                            Id = sti.Id,
                            DeliveredQuantity = sti.stockTransferItemsDelivered != null ? sti.stockTransferItemsDelivered.Sum(stid => stid.Quanity) : 0,
                            Quantity = sti.RequestedQuantity,
                            UnitPrice = sti.Item.CostPrice,
                            CostPrice = sti.Item.CostPrice,
                        }).ToList()
                    })
                    .ToListAsync();

                //if (!returnData.Any())
                    //return NotFound();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock transfer");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
        #region Private Helper Methods

        private async Task<(bool IsValid, string ErrorMessage)> ValidateItemsForTransfer(StockTransferCreateDto dto)
        {
            var itemIds = dto.Items.Select(x => x.ItemId).Distinct().ToList();

            var allItemsValidAtFromLoc = await _itemRepository.AllItemsAreValid(itemIds, (Guid)dto.FromLocationId);
            if (!allItemsValidAtFromLoc)
                return (false, "Some items are not valid at the source location");

            var allItemsValidAtToLoc = await _itemRepository.AllItemsAreValid(itemIds, (Guid)dto.ToLocationId);
            if (!allItemsValidAtToLoc)
                return (false, "Some items are not valid at the destination location");

            return (true, null);
        }

        private async Task<(bool IsValid, string ErrorMessage)> CheckAvailableStock(StockTransferCreateDto dto, Guid transferId, string userId)
        {
            foreach (var item in dto.Items)
            {
                // Check current stock at source location
                var sourceStock = await _context.StockLevels
                    .FirstOrDefaultAsync(sl => sl.ItemId == item.ItemId &&
                                               sl.LocationId == dto.FromLocationId);

                if (sourceStock == null)
                {
                    return (false, $"Item {item.ItemId} has no stock record at source location");
                }

                // Don't block creation based on insufficient stock - just warn
                if (sourceStock.ActualQuantity < item.Quantity)
                {
                    _logger.LogWarning("Low stock warning for item {ItemId} at location {LocationId}. Requested: {Requested}, Available: {Available}",
                        item.ItemId, dto.FromLocationId, item.Quantity, sourceStock.ActualQuantity);
                }

                // Create transfer item
                var transferItem = StockTransferItem.Create(
                    Guid.NewGuid(),
                    item.ItemId,
                    transferId,
                    item.Quantity,
                    DateTime.UtcNow,
                    Guid.Parse(userId)
                );

                _context.StockTransferItems.Add(transferItem);
            }

            return (true, null);
        }

        private async Task<bool> ValidateConfirmationCode(string code, Guid userId)
        {
            var confirmationCode = await _context.ConfirmationCodes
                .FirstOrDefaultAsync(x => x.Code.ToLower().Trim() == code.ToLower().Trim() &&
                                         x.AllowedUser == userId &&
                                         !x.Used);

            return confirmationCode != null;
        }

        private async Task MarkConfirmationCodeAsUsed(string code)
        {
            var confirmationCode = await _context.ConfirmationCodes
                .FirstOrDefaultAsync(x => x.Code.ToLower().Trim() == code.ToLower().Trim());

            if (confirmationCode != null)
            {
                confirmationCode.Used = true;
                confirmationCode.UpdatedAt = DateTime.UtcNow;
                _context.ConfirmationCodes.Update(confirmationCode);
            }
        }

        private async Task AddComment(Guid transferId, int status, string comment, string userId)
        {
            var commentEntity = Comments.Create(
                Guid.NewGuid(),
                transferId,
                status,
                comment,
                DateTime.UtcNow,
                Guid.Parse(userId)
            );
            await _context.Comments.AddAsync(commentEntity);
        }

        #endregion
    }

    // DTO for receiving items
    public class StockTransferReceiveDto
    {
        public Guid StockTransferId { get; set; }
        public StockTakeStatus Stage { get; set; }
        public string? Comment { get; set; }
        public string? ConfirmationCode { get; set; }
        public List<ReceivedItemDto> ReceivedItems { get; set; } = new();
    }

    public class ReceivedItemDto
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
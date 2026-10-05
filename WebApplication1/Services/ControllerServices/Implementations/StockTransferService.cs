// Services/StockTransferService.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.Controllers;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.DTOs;
using NLog.Filters;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class StockTransferService : IStockTransferService
    {
        private readonly AppDbContext _context;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<StockTransferService> _logger;
        private readonly ITransactionService _transactionService;

        public StockTransferService(
            AppDbContext context,
            ITransactionCodeRepository transactionCodeRepository,
            IItemRepository itemRepository,
            IUserRepository userRepository,
            ILogger<StockTransferService> logger,
            ITransactionService transactionService)
        {
            _context = context;
            _transactionCodeRepository = transactionCodeRepository;
            _itemRepository = itemRepository;
            _userRepository = userRepository;
            _logger = logger;
            _transactionService = transactionService;
        }

        public async Task CreateStockTransferAsync(CreateTransactionDto dto)
        {
          //  using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    throw new Exception("User not found");

                if (dto.BusinessPartnerId.HasValue == false) throw new Exception("A destination or target shop is required for this transaction to compltete");

                var itemIds = dto.Items.Select(x => x.ItemId).Distinct().ToList();

                //var allItemsValidAtFromLoc = await _itemRepository.AllItemsAreValid(itemIds, (Guid)dto.LocationId);
                //if (allItemsValidAtFromLoc != "ALL-VALID")
                //    throw new Exception(allItemsValidAtFromLoc);

                var allItemsValidAtToLoc = await _itemRepository.AllItemsAreValid(itemIds, (Guid)dto.BusinessPartnerId);
                if (allItemsValidAtToLoc != "ALL-VALID")
                    throw new Exception(allItemsValidAtToLoc);

                var transactionCode = await _transactionCodeRepository.GenerateTransactionCodeAsync("TRA", dto.LocationId);

                dto.TransactionType = TransactionType.TRAN;

                await _transactionService.CompleteTransationProcess(dto, TransactionResultsType.Creation);

              //  await _context.SaveChangesAsync();
              //  await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
              //  await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating stock transfer");
                throw;
            }
        }

        public async Task ApproveStockTransferAsync(StockTransferApproveDto dto)
        {
            if (!Enum.IsDefined(typeof(StockTransferEnum), dto.Stage) ||
                (dto.Stage != StockTransferEnum.Approved && dto.Stage != StockTransferEnum.Declined))
                throw new Exception("Invalid status submitted. Must be Approved or Declined.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    throw new Exception("Unauthorized");

                var stockTransfer = await  _context.StockTransfers.Where(x => x.Id == dto.StockTransferId)
                    .Include(x => x.Transaction)
                        .ThenInclude(x => x.TransactionItems)
                            .ThenInclude(x=> x.Item)
                                .ThenInclude(x=> x.StockLevel)
                        .FirstOrDefaultAsync();

                if (stockTransfer == null) throw new Exception("Transaction not found");
               
                if (stockTransfer.Status != StockTransferEnum.Pending)
                    throw new Exception("Transfer is not in pending state");

                if (dto.Stage == StockTransferEnum.Approved)
                {
                    stockTransfer.UpdateApproval(StockTransferEnum.Approved, DateTime.UtcNow, Guid.Parse(user.Id));

                    if (!string.IsNullOrWhiteSpace(dto.Comment))
                    {
                        var comment = TransactionComment.Create(
                            Guid.NewGuid(),
                            stockTransfer.TransactionId,
                            TransactionType.TRAN.ToString(),
                            StockTakeStatus.Declined.ToString(),
                            dto.Comment,
                            DateTime.UtcNow,
                            user.Id,
                            user.FullName);
                        await _context.Comments.AddAsync(comment);
                    }

                    //reduce stock level actual balance  of producer/Responser
                    var stockLevelsToUpdate = new List<StockLevel>();

                    foreach (var transItem in stockTransfer.Transaction.TransactionItems)
                    {
                        var stockLevel = transItem.Item.StockLevel.FirstOrDefault(x=> x.LocationId  == stockTransfer.ResponderId);

                        if ( stockLevel == null)
                        {
                            stockLevel = StockLevel.Create(Guid.NewGuid(), 0, 0, transItem.ItemId, stockTransfer.ResponderId);
                            _context.StockLevels.Add(stockLevel);
                        }
                        stockLevel.SubstractActual(transItem.Quantity);
                        stockLevelsToUpdate.Add(stockLevel);
                    }


                    _context.StockLevels.UpdateRange(stockLevelsToUpdate);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                   // return new { message = "Transfer request declined" };
                }

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error approving stock transfer");
                throw;
            }
        }

        public async Task ReceiveStockTransferAsync(StockTransferReceiveDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    throw new Exception("User not found");

                var stockTransfer = await _context.StockTransfers
                    .Include(st => st.Transaction)
                        .ThenInclude(t => t.TransactionItems)
                            .ThenInclude(ti => ti.TransactionItemsDelivered)
                    .Include(st => st.Transaction)
                        .ThenInclude(t => t.TransactionItems)
                            .ThenInclude(ti => ti.TransactionItemReceived)
                    .Include(st => st.Transaction)
                        .ThenInclude(t => t.TransactionItems)
                            .ThenInclude(ti => ti.Item)
                                .ThenInclude(i => i.StockLevel)
                    .Include(st => st.Transaction)
                        .ThenInclude(t => t.TransactionItems)
                            .ThenInclude(ti => ti.Item)
                                .ThenInclude(i => i.ItemLocations)
                    .FirstOrDefaultAsync(st => st.TransactionId == dto.TransactionId);

                if (stockTransfer == null)
                    throw new Exception("Transfer not found.");

                if (stockTransfer.Status != StockTransferEnum.Approved)
                    throw new Exception("Transfer must be approved before receiving.");

                var receivedItemsDict = dto.items.ToDictionary(x => x.ItemId, x => x.Quantity);

                // Track whether all items in this transfer are completely fulfilled
               // bool allItemsFullyReceived = true;

                Guid batchId = new Guid();
                foreach (var transItem in stockTransfer.Transaction.TransactionItems)
                {
                    // If item isn't in DTO, treat incoming quantity as 0 (partial receiving scenario)
                    var incomingQty = receivedItemsDict.GetValueOrDefault(transItem.ItemId, 0);

                    var qtyAlreadyReceived = transItem.TransactionItemReceived.Sum(x => x.Quantity);
                    var qtyAlreadyDelivered = transItem.TransactionItemsDelivered.Sum(x => x.Quantity);

                    var totalReceivedAfterThis = qtyAlreadyReceived + incomingQty;

                    if (totalReceivedAfterThis > qtyAlreadyDelivered)
                    {
                        throw new Exception(
                            $"Total received quantity ({totalReceivedAfterThis}) for Item {transItem.Item.Name} " +
                            $"exceeds delivered quantity ({qtyAlreadyDelivered}).");
                    }

                    if (totalReceivedAfterThis > transItem.Quantity)
                    {
                        throw new Exception(
                            $"Total received quantity ({totalReceivedAfterThis}) for Item {transItem.ItemId} " +
                            $"exceeds ordered quantity ({transItem.Quantity}).");
                    }

                    // Only process inventory & delivery records if there is actual stock coming in
                    if (incomingQty > 0)
                    {
                        var receiverLocation = transItem.Item.ItemLocations
                            .FirstOrDefault(x => x.LocationId == stockTransfer.RequesterId);

                        if (receiverLocation == null)
                            throw new Exception($"Receiving location ID {stockTransfer.RequesterId} not associated with Item ID {transItem.ItemId}.");

                        var receiverStockLevel = transItem.Item.StockLevel
                            .FirstOrDefault(x => x.LocationId == stockTransfer.RequesterId);


                        if (receiverStockLevel == null)
                        {
                            receiverStockLevel = StockLevel.Create(Guid.NewGuid(), incomingQty, incomingQty, transItem.ItemId, stockTransfer.RequesterId);
                           await _context.StockLevels.AddAsync(receiverStockLevel);
                        } else
                        {
                            receiverStockLevel.AddActual(incomingQty);
                            receiverStockLevel.AddAvailable(incomingQty);
                            _context.StockLevels.Update(receiverStockLevel);

                        }


                        var deliveryRecord = TransactionItemReceived.Create(
                            Guid.NewGuid(),
                            transItem.Id,
                            batchId,
                            incomingQty,
                            DateTime.UtcNow
                        );
                        await _context.TransactionItemReceived.AddAsync(deliveryRecord);
                    }

                }

                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    var comment = TransactionComment.Create(
                        Guid.NewGuid(),
                        stockTransfer.TransactionId,
                        TransactionType.TRAN.ToString(),
                        StockTransferEnum.Receival.ToString(),
                        dto.Comment,
                        DateTime.UtcNow,
                        user.Id,
                        user.FullName);
                    await _context.Comments.AddAsync(comment);
                }

                // Auto-complete transfer if every item's delivered total equals its ordered quantity
                //check for all delivered
                var checkForAlldeliverd = await _context.TransactionItems
                    .AnyAsync(x => (x.TransactionItemsDelivered != null ? x.TransactionItemsDelivered.Sum(d => d.Quantity) : 0)
                    != (x.TransactionItemReceived != null ? x.TransactionItemReceived.Sum(r => r.Quantity) : 0));

                if (!checkForAlldeliverd)
                {
                    stockTransfer.Completed();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error receiving stock transfer with ID: {StockTransferId}", dto.TransactionId);
                throw;
            }
        }

        public async Task<StockTransfer> GetStockTransferAsync(Guid id)
        {
            try
            {
                var stockTransfer = await _context.StockTransfers
                    .Include(st => st.Transaction)
                        .ThenInclude(x=> x.TransactionItems)
                            .ThenInclude(sti => sti.Item)
                    .FirstOrDefaultAsync(st => st.Id == id);

                if (stockTransfer == null)
                    throw new Exception("Transfer not found");

                return stockTransfer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock transfer");
                throw;
            }
        }

        public async Task<IEnumerable<StockTransfersDTO>> GetStockTransferRecordsAsync(BrowseStockTransfersFilters filters)
        {
            try
            {
                // Guard clause: Return empty list if both primary filters are missing
                if (!filters.LocationId.HasValue)
                {
                    return Enumerable.Empty<StockTransfersDTO>();
                }

                var query = _context.StockTransfers.AsNoTracking();
               
                query = query.Where(x => filters.Type == 1 ? x.RequesterId == filters.LocationId.Value : filters.Type == 2 ? x.ResponderId == filters.LocationId.Value : x.RequesterId == Guid.Empty);

                if (filters.Stage.HasValue)
                {
                    query = query.Where(x => x.Status == filters.Stage.Value);
                }
                
                if (filters.Approval && filters.Stage == StockTransferEnum.Pending)
                {
                    query = query.Where(x => x.ResponderId == filters.LocationId && x.Status == StockTransferEnum.Pending);
                }


                var returnData = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new StockTransfersDTO
                    {
                        Id = x.Id,
                        TransactionId = x.TransactionId,
                        SupplierId = x.ResponderId,
                        SupplierName = x.Responder != null ? x.Responder.Name : string.Empty,
                        Status = x.Status.ToString(),
                        CustomerId = x.RequesterId,
                        CustomerName = x.Requester != null ? x.Requester.Name : string.Empty,
                        TransactionCode = x.Transaction != null ? x.Transaction.TransactionNumber : string.Empty,
                        TransactionDate = x.TransferDate,
                        TotalAmount = x.Transaction != null ? x.Transaction.TotalAmount : 0
                    })
                    .ToListAsync();

                return returnData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock transfer records");
                throw;
            }
        }
    }
}
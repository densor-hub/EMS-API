using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DAL;
using WebApplication1.Domain.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using WebApplication1.Domain.DTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTakeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;

        public StockTakeController(
            AppDbContext context,
            IUserRepository userRepository,
            IItemRepository itemRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
        }

        // POST: api/StockTake
        [HttpPost]
        public async Task<ActionResult<bool>> CreateStockTake(StockTakeCreateDto dto)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();

            // Validate that all items exist in the system
            var itemIds = dto.Items is not null ? dto.Items.Select(si => si.ItemId).Distinct().ToList() : new List<Guid>();
            var allVItemsValid = await _itemRepository.AllItemsAreValid(itemIds, dto.LocationId);

            if (allVItemsValid == false) return BadRequest("Some items submitted to not exist in the sytem");

            var stockTakingId = Guid.Empty;

            var stockTakingAlreadyExist = await _context.StockTakings.Where(x =>
                                                                    x.LocationId == dto.LocationId
                                                                && x.StockTakingDate.Date == dto.StockTakingDate.Date
                                                                && x.GeneralStatus != GeneralStatus.SoftDeleted)
                                                            .FirstOrDefaultAsync();

            if (stockTakingAlreadyExist != null && stockTakingAlreadyExist.Stage != StockTakeStatus.Declined) return BadRequest("Invalid stock status");


            if (stockTakingAlreadyExist == null)
            {
                var stockTaking = StockTaking.Create(
                    Guid.NewGuid(),
                    dto.LocationId,
                    dto.ConductedBy,
                    dto.StockTakingDate,
                    dto.NextStockTakingDate,
                    dto.Comment ?? "",
                    DateTime.UtcNow,
                    dto.CreatedBy,
                    StockTakeStatus.Initiated
                );
                stockTakingId = stockTaking.Id;
                await _context.StockTakings.AddAsync(stockTaking);
            }
            else
            {
                stockTakingId = stockTakingAlreadyExist.Id;
                stockTakingAlreadyExist.Restart();

            }

            if (!string.IsNullOrEmpty(dto.Comment))
            {
                var comment = Comments.Create(Guid.NewGuid(), stockTakingId, (int)StockTakeStatus.Initiated, dto.Comment, DateTime.UtcNow, Guid.Parse(user.Id));
                await _context.Comments.AddAsync(comment);
            }


            if (stockTakingAlreadyExist != null)
            {
                var stockItems = _context.StockTakingItems.Where(x => x.StockTakingId == stockTakingAlreadyExist.Id);
                if (stockItems.Any())
                {
                    _context.StockTakingItems.RemoveRange(stockItems);
                }
            }

            // Add stock take items
            if (dto.Items != null)
            {
                foreach (var item in dto.Items)
                {
                    var stockLevel = await _context.StockLevels
                        .FirstOrDefaultAsync(sl => sl.ItemId == item.ItemId && sl.LocationId == dto.LocationId);

                    int expectedQuantity;

                    if (stockLevel == null)
                    {
                        // OPTION 1: Create stock level entry with zero quantity
                        // This ensures future stock operations have a record
                        stockLevel = StockLevel.Create(
                            Guid.NewGuid(),
                            0, // Actual quantity
                            0, // Available quantity
                            item.ItemId,
                            dto.LocationId
                        );

                        await _context.StockLevels.AddAsync(stockLevel);
                        expectedQuantity = 0;

                        // You might want to log this for auditing
                        // _context.StockLevelCreationLogs.Add(new StockLevelCreationLog...)
                    }
                    else
                    {
                        expectedQuantity = stockLevel.ActualQuantity;
                    }

                    var stockTakeItem = StockTakingItem.Create(
                        Guid.NewGuid(),
                        expectedQuantity,
                        item.Quantity,
                        expectedQuantity - item.Quantity,
                        stockTakingId,
                        item.ItemId,
                        DateTime.UtcNow,
                        dto.CreatedBy
                    );

                    _context.StockTakingItems.Add(stockTakeItem);
                }
            }

            var saved = await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateStockTake), new { id = stockTakingId }, saved > 0);
        }

        // PUT: api/StockTake/Verify
        [HttpPut("Verify")]
        public async Task<IActionResult> VerifyStockTake(StockTakeVerifyDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                // Validate status
                if (dto.Stage == StockTakeStatus.Initiated)
                    return BadRequest("Invalid Stage. Cannot verify with Initiated status.");

                if (dto.Stage != StockTakeStatus.Approved && dto.Stage != StockTakeStatus.Declined)
                    return BadRequest($"Invalid Stage. Expected Approved or Declined, got {dto.Stage}");

                // Fix typo in property name - assuming your DTO has StockTakingId
                var stockTaking = await _context.StockTakings
                    .Include(st => st.StockTakingItems)
                    .FirstOrDefaultAsync(st => st.Id == dto.StockTakingId && st.GeneralStatus != GeneralStatus.SoftDeleted && st.Stage != StockTakeStatus.Approved); // Fixed typo

                if (stockTaking == null)
                    return NotFound($"Stock take record not found");

                // Check if stock take can be verified
                if (stockTaking.Stage != StockTakeStatus.Initiated)
                    return BadRequest($"Invalid stage detected, Cannot verify stock take with status");

                // Validate that verified items are provided for BOTH Approved and Declined
                // (since you want to capture quantities even when declined)
                //if (dto.VerifiedItems == null || !dto.VerifiedItems.Any())
                //    return BadRequest("Verified items are required for both approval and decline");

                // Update stock take status
                stockTaking.VerifyStock(dto.Stage, dto.VerifiedBy, DateTime.UtcNow, dto.UpdatedBy);

                // Add comment if provided
                if (!string.IsNullOrEmpty(dto.Comment))
                {
                    var comment = Comments.Create(
                        Guid.NewGuid(),
                        stockTaking.Id,
                        (int)dto.Stage,
                        dto.Comment,
                        DateTime.UtcNow,
                        Guid.Parse(user.Id)
                    );
                    await _context.Comments.AddAsync(comment);
                }

                // Track if all items were found in the DTO
                var processedItems = new List<Guid>();

                // Update stock take items with verified quantities (for both Approved and Declined)
                foreach (var item in stockTaking.StockTakingItems)
                {
                    var verifiedItem = dto.Items.FirstOrDefault(vi => vi.ItemId == item.ItemId);
                    if (verifiedItem != null)
                    {
                        // Always update the item with verified quantity (for both Approved and Declined)
                        item.Verify(verifiedItem.Quantity, DateTime.UtcNow, dto.VerifiedBy);
                        processedItems.Add(item.ItemId);

                        // Only update stock levels if Approved
                        if (dto.Stage == StockTakeStatus.Approved)
                        {
                            var stockLevel = await _context.StockLevels
                                .FirstOrDefaultAsync(sl => sl.ItemId == item.ItemId && sl.LocationId == stockTaking.LocationId);

                            if (stockLevel != null)
                            {
                                // Adjust stock levels based on variance
                                var variance = verifiedItem.Quantity - stockLevel.ActualQuantity;
                                if (variance != 0)
                                {
                                    if (variance > 0)
                                    {
                                        stockLevel.AddActual(variance);
                                        stockLevel.AddAvailable(variance);
                                    }
                                    else
                                    {
                                        stockLevel.SubstractActual(Math.Abs(variance));
                                        stockLevel.SubstractAvailable(Math.Abs(variance));
                                    }
                                }
                            }
                            else
                            {
                                // Create stock level if it doesn't exist (shouldn't happen with your POST logic)
                                stockLevel = StockLevel.Create(
                                    Guid.NewGuid(),
                                    verifiedItem.Quantity,
                                    verifiedItem.Quantity,
                                    item.ItemId,
                                    stockTaking.LocationId
                                );
                                await _context.StockLevels.AddAsync(stockLevel);
                            }
                        }
                        // For Declined, we don't update stock levels, but the verified quantity is saved in StockTakingItem
                    }
                }

                // Check if all stock taking items were processed
                var missingItems = stockTaking.StockTakingItems
                    .Select(x => x.ItemId)
                    .Except(processedItems)
                    .ToList();

                if (missingItems.Any())
                {
                    return BadRequest($"Missing verification for items: {string.Join(", ", missingItems)}");
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                // Return appropriate response based on status
                if (dto.Stage == StockTakeStatus.Approved)
                {
                    return Ok(new
                    {
                        Message = "Stock take approved and stock levels updated successfully",
                        StockTakeId = stockTaking.Id,
                        Stage = dto.Stage,
                        VerifiedItemsCount = processedItems.Count
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Message = "Stock take declined. Verified quantities have been recorded but stock levels were not updated.",
                        StockTakeId = stockTaking.Id,
                        Stage = dto.Stage,
                        VerifiedItemsCount = processedItems.Count
                    });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //_logger.LogError(ex, "Registration failed for {Email}", request.AdminInfo.Email);
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        // DELETE: api/StockTake/{id} (Soft Delete)
        [HttpDelete]
        public async Task<IActionResult> DeleteStockTake([FromBody] GeneralDeleteDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var stockTaking = await _context.StockTakings
                    .Include(x => x.StockTakingItems)
                    .FirstOrDefaultAsync(st => st.Id == dto.Id);

                if (stockTaking == null)
                    return NotFound($"Stock take record not found");

                // Define which statuses allow deletion
                var allowedStatusesForDeletion = new[]
                {
                StockTakeStatus.Initiated,
                StockTakeStatus.Declined
            };

                if (!allowedStatusesForDeletion.Contains(stockTaking.Stage))
                {
                    return BadRequest(new
                    {
                        Message = $"Cannot delete stock take with status: {stockTaking.Stage}",
                        AllowedStatuses = allowedStatusesForDeletion.Select(s => s.ToString()),
                        CurrentStatus = stockTaking.Stage.ToString()
                    });
                }

                // Soft delete - mark as deleted instead of removing
                stockTaking.SoftDelete(Guid.Parse(user.Id), DateTime.UtcNow);

                if (stockTaking.StockTakingItems.Any())
                {
                    foreach (var item in stockTaking.StockTakingItems)
                    {
                        item.SoftDelete(Guid.Parse(user.Id), DateTime.UtcNow);
                    }
                }

                if (!string.IsNullOrEmpty(dto.Comment))
                {
                    var newComment = Comments.Create(Guid.NewGuid(), stockTaking.Id, (int)stockTaking.Stage, dto.Comment, DateTime.UtcNow, Guid.Parse(user.Id));
                    _context.Comments.Add(newComment);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Stock take marked as deleted",
                  //  StockTakeId = id,
                    Stage = stockTaking.Stage
                });
            } catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //_logger.LogError(ex, "Registration failed for {Email}", request.AdminInfo.Email);
                return StatusCode(500, new { message = "An error occurred" });
            }



        }

        // GET: api/StockTake/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StockTakingDto>> GetStockTake(Guid id)
        {
            var stockTaking = await _context.StockTakings
                .Include(st => st.Location)
                .Include(st => st.StockTakingItems)
                    .ThenInclude(sti => sti.Item)
                .Include(st => st.Conductor)
                .Include(st => st.Verifier)
                .FirstOrDefaultAsync(st => st.Id == id);

            if (stockTaking == null)
                return NotFound($"Stock take record not found");

            var stockTakeDto = MapToDto(stockTaking);
            return Ok(stockTakeDto);
        }

        // GET: api/StockTake
        [HttpGet]
        public async Task<ActionResult<PagedResult<StockTakingDto>>> GetAllStockTakes(
            [FromQuery] StockTakeFilterDto filter)
        {
            var query = _context.StockTakings
                .Include(st => st.Location)
                .Include(st => st.Conductor)
                .Include(st => st.Verifier)
                .Where(st => st.GeneralStatus == filter.GenealStatus )
                .AsQueryable();

            // Apply filters
            if (filter.LocationId.HasValue)
                query = query.Where(st => st.LocationId == filter.LocationId);

            if (filter.Stage.HasValue)
                query = query.Where(st => st.Stage == filter.Stage);

            if (filter.FromDate.HasValue)
                query = query.Where(st => st.StockTakingDate >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(st => st.StockTakingDate <= filter.ToDate);

            if (!string.IsNullOrEmpty(filter.ConductedBy))
                query = query.Where(st => st.ConductedBy.Contains(filter.ConductedBy));

            if (filter.HasVariance.HasValue && filter.HasVariance.Value)
            {
                query = query.Where(st => st.StockTakingItems.Any(item => item.Variance != 0));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "date" => filter.SortDescending
                    ? query.OrderByDescending(st => st.StockTakingDate)
                    : query.OrderBy(st => st.StockTakingDate),
                "status" => filter.SortDescending
                    ? query.OrderByDescending(st => st.Stage)
                    : query.OrderBy(st => st.Stage),
                "location" => filter.SortDescending
                    ? query.OrderByDescending(st => st.Location.Name)
                    : query.OrderBy(st => st.Location.Name),
                _ => filter.SortDescending
                    ? query.OrderByDescending(st => st.CreatedAt)
                    : query.OrderBy(st => st.CreatedAt)
            };

            // Apply pagination
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(st => new StockTakingListDto
                {
                    Id = st.Id,
                    LocationName = st.Location.Name,
                    Stage = st.Stage.ToString(),
                    StockTakingDate = st.StockTakingDate,
                    ConductedBy = st.ConductedBy,
                    VerifiedBy = st.VerifiedBy,
                    ItemCount = st.StockTakingItems.Count,
                    TotalVariance = st.StockTakingItems.Sum(x => Math.Abs(x.Variance)),
                    CreatedAt = st.CreatedAt
                })
                .ToListAsync();

            var result = new PagedResult<StockTakingListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return Ok(result);
        }

        // GET: api/StockTake/location/{locationId}/summary
        [HttpGet("location/{locationId}/summary")]
        public async Task<ActionResult<LocationStockTakeSummaryDto>> GetLocationSummary(Guid locationId)
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == locationId);

            if (location == null)
                return NotFound($"Location with ID {locationId} not found");

            var stockTakes = await _context.StockTakings
                .Include(st => st.StockTakingItems)
                .Where(st => st.LocationId == locationId && st.GeneralStatus != GeneralStatus.SoftDeleted)
                .OrderByDescending(st => st.StockTakingDate)
                .ToListAsync();

            var summary = new LocationStockTakeSummaryDto
            {
                LocationId = locationId,
                LocationName = location.Name,
                TotalStockTakes = stockTakes.Count,
                CompletedStockTakes = stockTakes.Count(st => st.Stage == StockTakeStatus.Completed),
                ApprovedStockTakes = stockTakes.Count(st => st.Stage == StockTakeStatus.Approved),
                DeclinedStockTakes = stockTakes.Count(st => st.Stage == StockTakeStatus.Declined),
              //  InProgressStockTakes = stockTakes.Count(st => st.Stage == StockTakeStatus.InProgress),
                LastStockTakeDate = stockTakes.FirstOrDefault()?.StockTakingDate,
                NextScheduledDate = stockTakes.FirstOrDefault()?.NextStockTakingDate
            };

            return Ok(summary);
        }

        // GET: api/StockTake/{id}/items
        [HttpGet("{id}/items")]
        public async Task<ActionResult<IEnumerable<StockTakingItemDto>>> GetStockTakeItems(Guid id)
        {
            var stockTaking = await _context.StockTakings
                .FirstOrDefaultAsync(st => st.Id == id );

            if (stockTaking == null)
                return NotFound($"Stock take record not found");

            var items = await _context.StockTakingItems
                .Include(sti => sti.Item)
                .Where(sti => sti.StockTakingId == id )
                .Select(sti => new StockTakingItemDto
                {
                    Id = sti.Id,
                    ItemId = sti.ItemId,
                    ItemName = sti.Item.Name,
                    ItemCode = sti.Item.Code,
                    ExpectedQuantity = sti.ExpectedQuantity,
                    ActualQuantity = sti.ActualQuantity,
                    VerifiedQuantity = sti.VerifiedQuantity,
                    Variance = sti.Variance,
                    Stage = GetItemStatus(sti)
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/StockTake/{id}/variance-report
        [HttpGet("{id}/variance-report")]
        public async Task<ActionResult<VarianceReportDto>> GetVarianceReport(Guid id)
        {
            var stockTaking = await _context.StockTakings
                .Include(st => st.Location)
                .Include(st => st.StockTakingItems)
                    .ThenInclude(sti => sti.Item)
                .FirstOrDefaultAsync(st => st.Id == id && st.GeneralStatus != GeneralStatus.SoftDeleted);

            if (stockTaking == null)
                return NotFound($"Stock take record not found");

            var itemsWithVariance = stockTaking.StockTakingItems
                .Where(sti => sti.Variance != 0)
                .Select(sti => new VarianceItemDto
                {
                    ItemId = sti.ItemId,
                    ItemName = sti.Item.Name,
                    ItemCode = sti.Item.Code,
                    ExpectedQuantity = sti.ExpectedQuantity,
                    ActualQuantity = sti.ActualQuantity,
                    VerifiedQuantity = sti.VerifiedQuantity,
                    Variance = sti.Variance,
                    VarianceType = sti.Variance > 0 ? "Surplus" : "Shortage",
                    VarianceValue = Math.Abs(sti.Variance)
                })
                .ToList();

            var report = new VarianceReportDto
            {
                StockTakeId = stockTaking.Id,
                LocationName = stockTaking.Location.Name,
                StockTakeDate = stockTaking.StockTakingDate,
                Stage = stockTaking.Stage.ToString(),
                TotalItemsCounted = stockTaking.StockTakingItems.Count,
                ItemsWithVariance = itemsWithVariance.Count,
                TotalVarianceValue = itemsWithVariance.Sum(x => x.VarianceValue),
                SurplusTotal = itemsWithVariance.Where(x => x.VarianceType == "Surplus").Sum(x => x.VarianceValue),
                ShortageTotal = itemsWithVariance.Where(x => x.VarianceType == "Shortage").Sum(x => x.VarianceValue),
                VarianceItems = itemsWithVariance
            };

            return Ok(report);
        }

       
        private StockTakingDto MapToDto(StockTaking stockTaking)
        {
            return new StockTakingDto
            {
                Id = stockTaking.Id,
                LocationId = stockTaking.LocationId,
                LocationName = stockTaking.Location?.Name,
                Stage = stockTaking.Stage.ToString(),
                StockTakingDate = stockTaking.StockTakingDate,
                NextStockTakingDate = stockTaking.NextStockTakingDate,
                ConductedBy = stockTaking.ConductedBy,
                VerifiedBy = stockTaking.VerifiedBy,
                Notes = stockTaking.Notes,
                CreatedAt = stockTaking.CreatedAt,
                UpdatedAt = stockTaking.UpdatedAt,
                Items = stockTaking.StockTakingItems?.Select(item => new StockTakingItemDto
                {
                    Id = item.Id,
                    ItemId = item.ItemId,
                    ItemName = item.Item?.Name,
                    ExpectedQuantity = item.ExpectedQuantity,
                    ActualQuantity = item.ActualQuantity,
                    VerifiedQuantity = item.VerifiedQuantity,
                    Variance = item.Variance
                }).ToList()
               
            };
        }

        private string GetItemStatus(StockTakingItem item)
        {
            if (item.Variance == 0)
                return "Matched";
            return item.Variance > 0 ? "Surplus" : "Shortage";
        }
    }
}
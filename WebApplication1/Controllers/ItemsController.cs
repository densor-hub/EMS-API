using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<ItemsController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IItemLocationRepository _itemLocationRepository;
        private readonly AppDbContext _appDbContext;
        private readonly ITransactionCodeRepository _transactionCodeRepository;

        private readonly   List<int> _lockedItemStatuses = new List<int> { (int)StocktakeSubmissionStatusEnum.Pending, (int)StocktakeSubmissionStatusEnum.Declined };

        public ItemsController(
            IItemRepository itemRepository,
            ILocationRepository locationRepository,
            ILogger<ItemsController> logger,
            IUserRepository userRepository,
            IItemLocationRepository itemLocationRepository,
            AppDbContext appDbContext,
            ITransactionCodeRepository transactionCodeRepository)
        {
            _itemRepository = itemRepository;
            _locationRepository = locationRepository;
            _logger = logger;
            _userRepository = userRepository;
            _itemLocationRepository = itemLocationRepository;
            _appDbContext = appDbContext;
            _transactionCodeRepository = transactionCodeRepository;
        }

        // GET: api/item
        [HttpGet("Sale")]
        public async Task<ActionResult<List<GetItemsResponseDTO>>> GetAllForSaleTransactions([FromQuery] BrowseItemsFilter filter)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if(user == null) { throw new Exception("Unauthorized"); }

                var items = _itemRepository.GetAllAsync(true);
                items = items.Where(x =>  x.CompanyId == user.CompanyId
                        && x.GeneralStatus != GeneralStatus.SoftDeleted
                        && x.StockLevel.Any(x=> x.LocationId == filter.LocationId && x.AvailableQuanity > 0)
                        && !(x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => _lockedItemStatuses.Contains((int)x.Status)))))
                        && x.ItemLocations.Any(x => x.LocationId == filter.LocationId && x.Status)
                     );


                if (filter.Category != null)
                {
                    items = items.Where(x => x.Category == filter.Category);
                }

                if (!string.IsNullOrEmpty(filter.TextFilter))
                {
                    var searchTerm = $"%{filter.TextFilter.Trim()}%";

                    items = items.Where(x =>
                        EF.Functions.Like(x.Name, searchTerm) ||
                        EF.Functions.Like(x.Code, searchTerm)
                    );
                }

                var returnData = await items.Select(x => new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = string.IsNullOrEmpty(x.Code) ? _transactionCodeRepository.GenerateEntityCodeAsync("T", x.IncrementalId, x.Company.IncrementalId) : x.Code,
                    Category = x.Category,
                    CategoryName = x.Category.ToString(),
                    //CostPrice = filter.TransactionType == TransactionType.PURC ? x.CostPrice : null,
                    Description = x.Description,
                    Name = x.Name,
                    SellingPrice = x.SellingPrice,
                    // SellingPrice = filter.TransactionType == TransactionType.SALE ? x.SellingPrice : null,
                    Status = x.Status,
                    ReorderLevel = x.ReorderLevel,
                    QuanityInUnit = x.QuantityInUnit,
                    UnitOfMeasure = x.UnitOfMeasure,
                    UnitOfMeasureName = x.UnitOfMeasure.ToString(),
                    Locations = x.ItemLocations.Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),
                    Locked= x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => _lockedItemStatuses.Contains((int)x.Status))))
                }).ToListAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items");
                return StatusCode(500, new { Message = "An error occurred while retrieving items" });
            }
        }

        // GET: api/item
        [HttpGet("Admin-View")]
        public async Task<ActionResult<List<GetItemsResponseDTO>>> GetAllForAdminWork([FromQuery] BrowseItemsFilter filter)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var items = _itemRepository.GetAllAsync(false);
                items = items.Where(x =>
                     x.CompanyId == user.CompanyId
                     && x.GeneralStatus != GeneralStatus.SoftDeleted
                     && x.ItemLocations.Any(x => x.LocationId == filter.LocationId) // && x.Status
                     //  && !(x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => lockedItemStatuses.Contains((int)x.Status)))))
                     );

                if (filter.Category != null)
                {
                    items = items.Where(x => x.Category == filter.Category);
                }

                if (!string.IsNullOrEmpty(filter.TextFilter))
                {
                    var searchTerm = $"%{filter.TextFilter.Trim()}%";

                    items = items.Where(x =>
                        EF.Functions.Like(x.Name, searchTerm) ||
                        EF.Functions.Like(x.Code, searchTerm)
                    );
                }

                var returnData = await items.Select(x => new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = string.IsNullOrEmpty(x.Code) ? _transactionCodeRepository.GenerateEntityCodeAsync("T", x.IncrementalId, x.Company.IncrementalId) : x.Code,
                    Category = x.Category,
                    CategoryName = x.Category.ToString(),
                    //CostPrice =  x.CostPrice ,
                    Description = x.Description,
                    Name = x.Name,
                    SellingPrice = x.SellingPrice,
                    Status = x.Status,
                    ReorderLevel = x.ReorderLevel,
                    QuanityInUnit = x.QuantityInUnit,
                    UnitOfMeasure = x.UnitOfMeasure,
                    Locked = x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => _lockedItemStatuses.Contains((int)x.Status)))),
                    UnitOfMeasureName = x.UnitOfMeasure.ToString(),
                    Locations = x.ItemLocations.Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),
                }).OrderBy(x=> x.Name.Trim()).ToListAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items");
                return StatusCode(500, new { Message = "An error occurred while retrieving items" });
            }
        }

        [HttpGet("Stocking")]
        public async Task<ActionResult<List<GetItemsResponseDTO>>> GetAllForPurchaseTransactions([FromQuery] BrowseItemsFilter filter)
        {
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var items = _itemRepository.GetAllAsync(true);
                items = items.Where(x =>
                     x.CompanyId == user.CompanyId
                     &&  x.GeneralStatus != GeneralStatus.SoftDeleted
                     && !(x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => _lockedItemStatuses.Contains((int)x.Status)))))
                        && x.ItemLocations.Any(x => x.LocationId == filter.LocationId && x.Status)
                     );

                items = items.Where(x => x.ItemLocations.Any(il => il.LocationId == filter.LocationId));

                if (filter.Category != null)
                {
                    items = items.Where(x => x.Category == filter.Category);
                }

                if (!string.IsNullOrEmpty(filter.TextFilter))
                {
                    var searchTerm = $"%{filter.TextFilter.Trim()}%";

                    items = items.Where(x =>
                        EF.Functions.Like(x.Name, searchTerm) ||
                        EF.Functions.Like(x.Code, searchTerm)
                    );
                }

                var returnData = await items.Select(x => new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Category = x.Category,
                    CategoryName = x.Category.ToString(),
                    //CostPrice = x.CostPrice,
                   // CostPrice = filter.TransactionType == TransactionType.PURC ? x.CostPrice : null,
                    Description = x.Description,
                    Name = x.Name,
                    SellingPrice =  x.SellingPrice,
                    Status = x.Status,
                    ReorderLevel = x.ReorderLevel,
                    QuanityInUnit = x.QuantityInUnit,
                    UnitOfMeasure = x.UnitOfMeasure,
                    UnitOfMeasureName = x.UnitOfMeasure.ToString(),
                    Locations = x.ItemLocations.Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),
                    Locked = x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => _lockedItemStatuses.Contains((int)x.Status))))
                }).ToListAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items");
                return StatusCode(500, new { Message = "An error occurred while retrieving items" });
            }
        }



        [HttpGet("Locked")]
        public async Task<ActionResult<List<GetItemsResponseDTO>>> GetAllAllLocked([FromQuery] BrowseItemsFilter filter)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var lockedItemStatuses = new List<int> { (int)StocktakeSubmissionStatusEnum.Pending, (int)StocktakeSubmissionStatusEnum.Declined };
                var items = _itemRepository.GetAllAsync(false);
                items = items.Where(x =>
                    x.CompanyId == user.CompanyId
                    && x.GeneralStatus != GeneralStatus.SoftDeleted
                    && (x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => lockedItemStatuses.Contains((int)x.Status)))))
                     && x.ItemLocations.Any(x => x.LocationId == filter.LocationId && x.Status)
                    );

                items = items.Where(x => x.ItemLocations.Any(il => il.LocationId == filter.LocationId));

                if (filter.Category != null)
                {
                    items = items.Where(x => x.Category == filter.Category);
                }

                if (!string.IsNullOrEmpty(filter.TextFilter))
                {
                    var searchTerm = $"%{filter.TextFilter.Trim()}%";

                    items = items.Where(x =>
                        EF.Functions.Like(x.Name, searchTerm) ||
                        EF.Functions.Like(x.Code, searchTerm)
                    );
                }

                var returnData = await items.Select(x => new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = string.IsNullOrEmpty(x.Code) ? _transactionCodeRepository.GenerateEntityCodeAsync("T", x.IncrementalId, x.Company.IncrementalId) : x.Code,
                    Category = x.Category,
                    CategoryName = x.Category.ToString(),
                    //CostPrice = x.CostPrice,
                    Description = x.Description,
                    Name = x.Name,
                    //SellingPrice = x.SellingPrice,
                    Status = x.Status,
                    ReorderLevel = x.ReorderLevel,
                    QuanityInUnit = x.QuantityInUnit,
                    UnitOfMeasure = x.UnitOfMeasure,
                    UnitOfMeasureName = x.UnitOfMeasure.ToString(),
                    Locked = x.StockLockDownItems.Any(sld => (!sld.Submissions.Any() || sld.Submissions.Any(x => _lockedItemStatuses.Contains((int)x.Status)))),
                    Locations = x.ItemLocations.Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),
                }).ToListAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items");
                return StatusCode(500, new { Message = "An error occurred while retrieving items" });
            }
        }


        // GET: api/item/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetItemsResponseDTO>> GetById(Guid id)
        {
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var items = _itemRepository.GetAllAsync(false);
                items = items.Where(x => x.CompanyId == user.CompanyId && x.Id == id);


                var returnData = await items.Select(x => new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = string.IsNullOrEmpty(x.Code) ? _transactionCodeRepository.GenerateEntityCodeAsync("T", x.IncrementalId, x.Company.IncrementalId) : x.Code,
                    Category = x.Category,
                    CategoryName = x.Category.ToString(),
                    //   CostPrice = x.CostPrice,
                    Description = x.Description,
                    Name = x.Name,
                    //    SellingPrice = x.SellingPrice,
                    Status = x.Status,
                    ReorderLevel = x.ReorderLevel,
                    QuanityInUnit = x.QuantityInUnit,
                    UnitOfMeasure = x.UnitOfMeasure,
                    UnitOfMeasureName = x.UnitOfMeasure.ToString()
                }).FirstOrDefaultAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item {Id}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving the item" });
            }
        }


        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ItemStockLevelDTO>>> GetLowStockItems([FromQuery] Guid locationId)
        {
            try
            {
                // Guard against empty location GUIDs
                if (locationId == Guid.Empty)
                    return BadRequest(new { error = "A valid locationId is required." });

                var itemsQuery = _itemRepository.GetLowStockItems(locationId);

                var returnData = await itemsQuery.Select(x => new ItemStockLevelDTO
                {
                    ActualQuantity = x.StockLevel
                        .Where(s => s.LocationId == locationId)
                        .Select(s => (int?)s.ActualQuantity)
                        .FirstOrDefault() ?? 0,

                    AvailableQuantity = x.StockLevel
                        .Where(s => s.LocationId == locationId)
                        .Select(s => (int?)s.AvailableQuanity)
                        .FirstOrDefault() ?? 0,

                    ReorderLevel = x.ReorderLevel,
                    Name = x.Name,
                    Code = x.Code
                }).ToListAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items for location {LocationId}", locationId);
                return StatusCode(500, new { error = "An error occurred while retrieving low stock items" });
            }
        }

        [HttpGet("Stock-Level/{itemId:guid}")]
        public async Task<ActionResult<ItemStockLevelDTO>> GetCurrentStockLevel([FromRoute] Guid itemId, [FromQuery] Guid locationId)
        {
            try
            {
                if (itemId == Guid.Empty)
                    return BadRequest(new { Message = "A valid item ID is required." });

                var item = await _itemRepository.GetByIdAsync(itemId);

                if (item == null)
                    return NotFound(new { message = $"Item was not found." });
                // Retrieve single item entity
                var stockLevel = await _itemRepository.GetItemStockLevel(itemId, locationId);

               

                // Safely extract stock level depending on whether StockLevel is a collection or single object
                // If StockLevel is ICollection<StockLevel>: use item.StockLevel?.FirstOrDefault()
                // If StockLevel is a single StockLevel entity: use item.StockLevel directly
                //var stockLevel = StockLevel?.FirstOrDefault();

                var results = new ItemStockLevelDTO
                {
                    ActualQuantity = stockLevel?.ActualQuantity ?? 0,
                    AvailableQuantity = stockLevel?.AvailableQuanity ?? 0, // Fixed typo: AvailableQuanity
                    ReorderLevel = stockLevel?.Item?.ReorderLevel ?? 0,
                    Name = stockLevel?.Item?.Name ?? "",
                    Code = stockLevel?.Item?.Code ?? ""
                };

                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock level for item ID {ItemId}", itemId);
                return StatusCode(500, new { Message = "An error occurred while retrieving stock level" });
            }
        }

        // POST: api/item
        [HttpPost("{LocationId:Guid}")]
        public async Task<IActionResult> Create([FromBody] CreateItemDTO dto, [FromRoute]  Guid locationId)
        {

           
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if location exists
                if (!Enum.IsDefined(typeof(ItemsCategory), dto.Category)) return BadRequest(new { Message = $"Category not found" });

                if (!Enum.IsDefined(typeof(UnitOfMeasure), dto.UnitOfMeasure)) return BadRequest(new { Message = $"Unit of measure not found" });

                if (dto.SellingPrice < dto.CostPrice) return BadRequest("Selling Price cannot be less than cost price");

                if (dto.ReorderLevel <= 0) return BadRequest("Re-order level must be greater than 0");

                if (dto.QuantityInUnit <= 0) return BadRequest("Quanity in unit must be greater than 0");

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) { return Unauthorized(); }


                if (await _itemRepository.NameExist(dto.Name, Guid.Empty)) throw new Exception("Another item exists with the submitted name");

                var locationsForValidation = dto.Locations.Contains(locationId) ? dto.Locations : dto.Locations.Prepend(locationId).ToList();

                var queriableLocations =   _locationRepository.ExistingLocations(locationsForValidation);

                if (!queriableLocations.Any()) { return NotFound($"No Shop found"); }

                var locationsIds = queriableLocations.Select(x => x.Id).ToList();
                if (!locationsIds.Contains(locationId)) return BadRequest("Access to shop not found");

                // Check if code already exists
                if (!string.IsNullOrEmpty(dto.Code) && await _itemRepository.CodeExistsAsync(dto.Code))
                    return Conflict($"Item with code '{dto.Code}' already exists");

                

                var item = dto;

               // var Code = await _transactionCodeRepository.GenerateEntityCodeAsync("ITM", locationId);
                var newItem = Item.Create(Guid.NewGuid(), item.Code ?? "", item.Name, item?.Description ?? "", item.Category, item.UnitOfMeasure, item.QuantityInUnit,
                    item.SellingPrice,  item.ReorderLevel ?? 0, item.Status, DateTime.UtcNow, Guid.Parse(user.Id), (Guid)user.CompanyId, locationId, dto.CostPrice);

                var createdItem = await _itemRepository.CreateAsync(newItem);

                

                var itemLocations = locationsIds.Select(x => ItemLocation.Create(Guid.NewGuid(), x, newItem.Id, DateTime.UtcNow, Guid.Parse(user.Id), true)).ToList();

                await _itemLocationRepository.AddRangeAsync(itemLocations);

                await _appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return StatusCode(201, new { Id = createdItem.Id});
            }
            catch (Exception ex)
            {
                 await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating item");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // PUT: api/item/{id}
        [HttpPut("UpdateItem")]
        public async Task<IActionResult> Update([FromBody] UpdateItemDto dto)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) return Unauthorized();

                if (dto.SellingPrice < dto.CostPrice) return BadRequest("Selling Price cannot be less than cost price");

                if (!Enum.IsDefined(typeof(ItemsCategory), dto.Category)) return BadRequest(new { error = $"Category not found" });

                if (!Enum.IsDefined(typeof(UnitOfMeasure), dto.UnitOfMeasure)) return BadRequest(new { error = $"Unit of measure not found" });

                if (dto.ReorderLevel <= 0) return BadRequest(new { Message = "Re-order level must be greater than 0" });

                if (dto.QuantityInUnit <= 0) return BadRequest(new { Message = "Quanity in unit must be greater than 0" });

                var existingItem = await _itemRepository.GetByIdAsync(dto.Id);

                if (existingItem == null) return NotFound(new { Message = $"Item  not found" });


                if (await _itemRepository.NameExist(dto?.Name??"", existingItem.Id)) return Conflict(new { error = $"Item with the same name exists" });
                // Check if location exists
                var queriableLocations = _locationRepository.GetAll();

                queriableLocations  = queriableLocations.Where(x => dto.Locations.Contains(x.Id));

                if (!queriableLocations.Any()) { return BadRequest(new { Message = $"No Shop found" }); }



                var item = dto;
                existingItem.Update(existingItem.Code, item?.Name??existingItem.Name, item?.Description??existingItem.Description, item?.Category??existingItem.Category, item?.UnitOfMeasure??existingItem.UnitOfMeasure,
                    item?.QuantityInUnit??existingItem.QuantityInUnit, item?.SellingPrice??existingItem.SellingPrice,  item?.ReorderLevel??existingItem.ReorderLevel, item.Status,  DateTime.UtcNow, Guid.Parse(user.Id));


                var updatedItem = await _itemRepository.UpdateAsync(existingItem);

                var locationsId = queriableLocations.Select(x => x.Id).ToList();

                var newItemLocations = locationsId.Select(x => ItemLocation.Create(Guid.NewGuid(), x, updatedItem.Id, DateTime.UtcNow, Guid.Parse(user.Id), true)).ToList();

                //delete existing item locations
                var itemLocationsToBeDeleted = _itemLocationRepository.GetAllByItemId(dto.Id);
                 _appDbContext.ItemLocations.RemoveRange(itemLocationsToBeDeleted);

                //add new item locations
                await _itemLocationRepository.AddRangeAsync(newItemLocations);

                 await _appDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating item {Id}", dto.Id);
                return StatusCode(500, new { Message = "An error occurred while updating the item" });
            }
        }



        // DELETE: api/item/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(id);

                if (item.StockLockDownItems.Any() || item.TransactionItems.Any()) return BadRequest("Item has been used for transaction, hence delete is not allowed");

                if (!await _itemRepository.ExistsAsync(id))
                    return NotFound(new { Message = $"Item  not found" });

                await _itemRepository.DeleteAsync(id);
                await _appDbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item {Id}", id);
                return StatusCode(500, new { Message = "An error occurred while deleting the item" });
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public ItemsController(
            IItemRepository itemRepository,
            ILocationRepository locationRepository,
            ILogger<ItemsController> logger,
            IUserRepository userRepository,
            IItemLocationRepository itemLocationRepository,
            AppDbContext appDbContext)
        {
            _itemRepository = itemRepository;
            _locationRepository = locationRepository;
            _logger = logger;
            _userRepository = userRepository;
            _itemLocationRepository = itemLocationRepository;
            _appDbContext = appDbContext;
        }

        // GET: api/item
        [HttpGet]
        public async Task<ActionResult<List<GetItemsResponseDTO>>> GetAll([FromQuery] BrowseItemsFilter filter)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var items  =  _itemRepository.GetAllAsync();
                items = items.Include(x=> x.ItemLocations).Where(x=> x.CompanyId == user.CompanyId);

                if (filter.LocationId != null)
                {
                    items = items.Where(x => x.ItemLocations.Any(il => il.LocationId == filter.LocationId));
                }

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

                var returnData = await items.Select(x=> new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                     Category = x.Category,
                     CategoryName = x.Category.ToString(),
                     CostPrice = x.CostPrice,
                     Description = x.Description,
                     Name = x.Name,
                     SellingPrice = x.SellingPrice,
                     Status = x.Status,
                     ReorderLevel = x.ReorderLevel,
                     QuanityInUnit = x.QuanityInUnit,
                     UnitOfMeasure = x.UnitOfMeasure,
                     UnitOfMeasureName = x.UnitOfMeasure.ToString(),
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
                return StatusCode(500, new { error = "An error occurred while retrieving items" });
            }
        }

        // GET: api/item/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetItemsResponseDTO>> GetById(Guid id)
        {
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var items = _itemRepository.GetAllAsync();
                items = items.Where(x => x.CompanyId == user.CompanyId && x.Id == id);


                var returnData = await items.Select(x => new GetItemsResponseDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Category = x.Category,
                    CategoryName = x.Category.ToString(),
                    CostPrice = x.CostPrice,
                    Description = x.Description,
                    Name = x.Name,
                    SellingPrice = x.SellingPrice,
                    Status = x.Status,
                    ReorderLevel = x.ReorderLevel,
                    QuanityInUnit = x.QuanityInUnit,
                    UnitOfMeasure = x.UnitOfMeasure,
                    UnitOfMeasureName = x.UnitOfMeasure.ToString()
                }).FirstOrDefaultAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the item" });
            }
        }


        [HttpGet("Low-stock")]
        public async Task<ActionResult<ItemStockLevelDTO>> GetLowStockItems([FromQuery] Guid locationId)
        {
            try
            {
                var items =  _itemRepository.GetLowStockItems(locationId);

                var returnData =  await items.Select(x => new ItemStockLevelDTO
                {
                    ActualQuantity = x.StockLevel.ActualQuantity,
                    AvailableQuantity = x.StockLevel.AvailableQuanity,
                    ReorderLevel = x.ReorderLevel,
                    Name = x.Name,
                    Code = x.Code
                }).ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items");
                return StatusCode(500, new { error = "An error occurred while retrieving low stock items" });
            }
        }

        // POST: api/item
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemDTO dto)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!Enum.IsDefined(typeof(ItemsCategory), dto.Category)) return BadRequest(new { error = $"Category not found" });

                if (!Enum.IsDefined(typeof(UnitOfMeasure), dto.UnitOfMeasure)) return BadRequest(new { error = $"Unit of measure not found" });

                if (dto.SellingPrice < dto.CostPrice) return BadRequest("Selling Price cannot be less than cost price");

                if (dto.ReorderLevel <= 0) return BadRequest("Re-order level must be greater than 0");

                if (dto.QuantityInUnit <= 0) return BadRequest("Quanity in unit must be greater than 0");

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) { return Unauthorized(); }

                // Check if location exists
                var queriableLocations =   _locationRepository.ValidateLocations(dto.Locations);

                if (!queriableLocations.Any()) { return NotFound($"No Shop found"); }

                // Check if code already exists
                if (!string.IsNullOrEmpty(dto.Code) && await _itemRepository.CodeExistsAsync(dto.Code))
                    return Conflict($"Item with code '{dto.Code}' already exists");


                var item = dto;

                var Code = await _itemRepository.GenerateCodeAsync(dto.Locations.FirstOrDefault());
                var newItem = Item.Create(Guid.NewGuid(), item.Code ?? Code, item.Name, item?.Description ?? "", item.Category, item.UnitOfMeasure, item.QuantityInUnit,
                    item.SellingPrice, item.CostPrice, item.ReorderLevel ?? 0, item.Status, DateTime.UtcNow, Guid.Parse(user.Id), (Guid)user.CompanyId );

                var createdItem = await _itemRepository.CreateAsync(newItem);

                var locationsId = queriableLocations.Select(x => x.Id).ToList();

                var itemLocations = locationsId.Select(x => ItemLocation.Create(Guid.NewGuid(), x, newItem.Id, DateTime.UtcNow, Guid.Parse(user.Id), true)).ToList();

                await _itemLocationRepository.AddRangeAsync(itemLocations);

                await transaction.CommitAsync();

                return StatusCode(201, new { Id = createdItem.Id});
            }
            catch (Exception ex)
            {
                 await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating item");
                return StatusCode(500, new { error = "An error occurred while creating the item" });
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

                if (dto.ReorderLevel <= 0) return BadRequest(new { error = "Re-order level must be greater than 0" });

                if (dto.QuantityInUnit <= 0) return BadRequest(new { error = "Quanity in unit must be greater than 0" });

                var existingItem = await _itemRepository.GetByIdAsync(dto.Id);

                if (existingItem == null) return NotFound(new { error = $"Item  not found" });


                // Check if location exists
                var queriableLocations = _locationRepository.GetAll();

                queriableLocations  = queriableLocations.Where(x => dto.Locations.Contains(x.Id));

                if (!queriableLocations.Any()) { return BadRequest(new { error = $"No Shop found" }); }


                
                var code = string.IsNullOrEmpty(existingItem.Code) ? await _itemRepository.GenerateCodeAsync(dto.Locations.FirstOrDefault()) : existingItem.Code;
                //if (await _itemRepository.CodeExistsAsync(dto.Code, dto.Id))
                //    return Conflict(new { error = $"Item with code '{dto.Code}' already exists" });
                //if (existingItem == null)  return null;



                var item = dto;
                existingItem.Update(code, item?.Name??existingItem.Name, item?.Description??existingItem.Description, item?.Category??existingItem.Category, item?.UnitOfMeasure??existingItem.UnitOfMeasure,
                    item?.QuantityInUnit??existingItem.QuanityInUnit, item?.SellingPrice??existingItem.SellingPrice, item?.CostPrice??existingItem.CostPrice, item?.ReorderLevel??existingItem.ReorderLevel, item.Status,  DateTime.UtcNow, Guid.Parse(user.Id));


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
                return StatusCode(500, new { error = "An error occurred while updating the item" });
            }
        }



        // DELETE: api/item/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (!await _itemRepository.ExistsAsync(id))
                    return NotFound(new { error = $"Item with ID {id} not found" });

                await _itemRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item {Id}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the item" });
            }
        }
    }
}
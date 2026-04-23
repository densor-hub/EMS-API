using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
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
    public class LocationsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<LocationsController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IPositionRepository _positonRepository;
        private readonly IEmployeeRepository _employeeRepository;   
        private readonly ILocationManagementsRepository _locationManagementsRepository;
        private readonly AppDbContext _appDbContext;

        public LocationsController(
            ILocationRepository locationRepository,
            ILogger<LocationsController> logger,
             IUserRepository userRepository,
             IPositionRepository positonRepository,
             IEmployeeRepository employeeRepository,
             ILocationManagementsRepository locationManagementsRepository,
             AppDbContext appDbContext
            )
        {
            _locationRepository = locationRepository;
            _logger = logger;
            _userRepository = userRepository;
            _positonRepository = positonRepository;
            _employeeRepository = employeeRepository;
            _locationManagementsRepository  = locationManagementsRepository;
            _appDbContext   = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetLocationDTO>>> GetAll([FromQuery] Guid? LocationId, [FromQuery] bool OnlyActive, [FromQuery] string? TextFilter)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var locations =  _locationRepository.GetAll();

                 locations = locations.Where(x => x.CompanyId == user.CompanyId);

                if (LocationId != Guid.Empty) locations = locations.Where(x => x.Id != LocationId);

                if (OnlyActive) locations = locations.Where(x => x.Status == OnlyActive);
                if (!string.IsNullOrEmpty(TextFilter))
                {
                    ///locations = locations.Where(x => x.Name.ToLower().Trim().Contains(filter.TextFilter.ToLower().Trim()));
                     var searchTerm = $"%{TextFilter.Trim()}%";

                    locations = locations.Where(x =>
                        EF.Functions.Like(x.Name, searchTerm) ||
                        EF.Functions.Like(x.Code, searchTerm)
                    );
                }

                var returnData = await locations.Select(x => new GetLocationDTO
                {
                    Id = x.Id,
                    Address = x.Address,
                    Code = x.Code,
                    Email = x.Email,
                    Status = x.Status,
                    Name = x.Name,
                    Phone = x.Phone,
                    TypeOfLocationName = x.TypeOfLocation.ToString(),
                    //TypeOfLocation = x.TypeOfLocation,
                    Managers = x.LocationManangements.Where(x=> x.Status == true).Select(sm => new IdAndNameDTO
                    {
                        Id = sm != null ? sm.ManagerId: Guid.Empty,
                        Name =$"{sm.Manager.FirstName} {sm.Manager.LastName}",
                    }).ToList()
                }).ToListAsync();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all locations");
                return StatusCode(500, new { error = "An error occurred while retrieving locations" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetLocationDTO>> GetById([FromRoute]Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                var locations = await _locationRepository.GetAllAsync();

                locations = locations.Where(x => x.CompanyId == user.CompanyId);

                var returnData = locations.Select(x => new GetLocationDTO
                {
                    Id = x.Id,
                    Address = x.Address,
                    Code = x.Code,
                    Email = x.Email,
                    Status = x.Status,
                    Name = x.Name,
                    Phone = x.Phone,
                    TypeOfLocationName = x.TypeOfLocation.ToString(),
                  //  TypeOfLocation = x.TypeOfLocation,
                    Managers = x.LocationManangements.Where(x => x.Status == true).Select(sm => new IdAndNameDTO
                    {
                       // Id = sm != null ? sm.ManagerId: Guid.Empty,
                        Name = $"{sm.Manager.FirstName} {sm.Manager.LastName}",
                    }).ToList()
                }).FirstOrDefault();

                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving the location" });
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateShopDTO dto)
        {
            var transaction =  await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) return StatusCode(401, new { error = "Unautorized" });
               
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

               if (dto.LocationType != null)
                {
                    if (!Enum.IsDefined(typeof(LocationType), dto.LocationType)) { return StatusCode(500, new { error = "Invalid  type" }); }
                }

                if (await _locationRepository.CodeExistsAsync(dto.Code))  return Conflict(new { error = $"Location with code '{dto.Code}' already exists" });

                if (dto?.Managers?.Count > 0)
                {
                    var validManager = _employeeRepository.GetAll();
                    validManager = validManager.Where(x => dto.Managers.Select(m => m.Id).ToList().Contains(x.Id) && x.CompanyId == user.CompanyId);
                    if (!validManager.Any()) return NotFound("Submitted manager(s) not found");
                }

                var Code =  await _locationRepository.GenerateCodeAsync(user.CompanyId);
                
                var location = Location.Create(Guid.NewGuid(), dto.Code ?? Code, dto.Name, dto.Status, dto.Address, dto.Phone, dto.Email, (Guid)user.CompanyId, dto.LocationType, DateTime.UtcNow, Guid.Parse(user.Id));

                var createdLocation = await _locationRepository.CreateAsync(location);

                if (dto?.Managers?.Count > 0)
                {
                    var managers = dto.Managers.Select(x => LocationManangement.Create(Guid.NewGuid(), x.Id, location.Id, true, x.StartDate, x.EndDate, DateTime.UtcNow, Guid.Parse(user.Id), x.IsMainManager)).ToList();
                    await _locationManagementsRepository.AddRangeAsync(managers);
                }

                await transaction.CommitAsync();

                return StatusCode(201, new { id = createdLocation.Id });
               // return CreatedAtAction(nameof(GetById), new { id = createdLocation.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating location");
                return StatusCode(500, new { error = "An error occurred while creating the location" });
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update( [FromBody] UpdateShopDTO dto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) return StatusCode(401, new { error = "Unautorized" });

                if (dto.LocationType is not null)
                {
                    if (!Enum.IsDefined(typeof(LocationType), dto.LocationType)) { return StatusCode(500, new { error = "Invalid location type" }); }
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var existingLocation = await _locationRepository.GetByIdAsync(dto.Id);
                if (existingLocation == null || existingLocation.CompanyId != user.CompanyId) { return NotFound(new { error = $"Location not found" }); }

                if (!string.IsNullOrEmpty(dto.Code))
                {
                    if (await _locationRepository.CodeExistsAsync(dto.Code, dto.Id))
                        return Conflict(new { error = $"Location with code '{dto.Code}' already exists" });
                }


                if (dto?.Managers?.Count > 0)
                {
                    var validManager = _employeeRepository.GetAll();
                    validManager = validManager.Where(x => dto.Managers.Select(m => m.Id).ToList().Contains(x.Id) && x.CompanyId == user.CompanyId);
                    if (!validManager.Any()) return NotFound("Submitted manager(s) not found");
                }

                existingLocation.Update(
                    dto?.Code??existingLocation.Code,
                    dto?.Name ?? existingLocation.Name,
                     dto.Status,
                      dto?.Address ?? existingLocation.Address,
                       dto.Phone ?? existingLocation.Phone,
                        dto?.Email ?? existingLocation.Email,
                      dto?.LocationType != null ? Enum.IsDefined(typeof(LocationType), dto.LocationType) ? dto.LocationType : existingLocation.TypeOfLocation : null,
                    DateTime.UtcNow,
                         Guid.Parse(user.Id)
                    );
                
                await _locationRepository.UpdateAsync(existingLocation);

                if (dto?.Managers?.Count > 0)
                {
                    //delete previous
                    var oldManagers =   _locationManagementsRepository.GetAllByLocationId(existingLocation.Id);
                    await _locationManagementsRepository.DeleteRangeAsync(oldManagers.ToList());

                    //new managers
                    var managers = dto.Managers.Select(x => LocationManangement.Create(Guid.NewGuid(), x.Id, existingLocation.Id, true, x.StartDate, x.EndDate, DateTime.UtcNow, Guid.Parse(user.Id), x.IsMainManager)).ToList();
                    await _locationManagementsRepository.AddRangeAsync(managers);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location {Id}", dto.Id);
                return StatusCode(500, new { error = "An error occurred while updating the location" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                if (!await _locationRepository.ExistsAsync(id))
                    return NotFound(new { error = $"Location not found" });

                await _locationRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location {Id}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the location" });
            }
        }
    }
}
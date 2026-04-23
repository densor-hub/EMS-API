using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.DAL.Repository;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierLocationRepository _supplierLocationRepository;
        private readonly ILogger<SuppliersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly AppDbContext _appDbContext;

        public SuppliersController(
            ISupplierRepository supplierRepository,
            ISupplierLocationRepository supplierLocationRepository,
            ILogger<SuppliersController> logger,
            IUserRepository userRepository,
            ILocationRepository locationRepository,
             AppDbContext appDbContext)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _appDbContext = appDbContext;
            _supplierLocationRepository = supplierLocationRepository;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetSupplierDto>>> GetAllEmployees([FromQuery] Guid? locationId = null)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();

                if (user is null) return Unauthorized();
                var suppliers =  _supplierRepository.GetAllAsync((Guid)user.CompanyId);

                if (locationId != null)
                {
                    suppliers = suppliers.Where(x => x.SupplierLocations.Any(x => x.LocationId == locationId));
                }

                var returnData = suppliers.Select(supplier => new GetSupplierDto
                {
                    Id = supplier.Id,
                    FirstName = supplier.FirstName,
                    LastName = supplier.LastName,
                    Phone = supplier.Phone,
                    Email = supplier.Email,
                    Address = supplier.Address,
                    Code = supplier.Code,
                    TIN = supplier.TIN,
                    SupplierCompanyName = supplier.SupplierCompanyName,
                    Status = supplier.Status,
                    StatusName = supplier.Status.ToString(),
                    Locations = supplier.SupplierLocations.Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),

                });
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                return StatusCode(500, "An error occurred while retrieving employees");
            }
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetSupplierDto>> GetEmployeeById(Guid id)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);

                if (supplier == null)
                    return NotFound($"Employee not found");

                var returnData = new GetSupplierDto
                {
                    Id = supplier.Id,
                    FirstName = supplier.FirstName,
                    LastName = supplier.LastName,
                    Phone = supplier.Phone,
                    Email = supplier.Email,
                    Address = supplier.Address,
                    Code = supplier.Code,
                    TIN = supplier.TIN,
                    SupplierCompanyName = supplier.SupplierCompanyName,
                    Status = supplier.Status,
                    StatusName = supplier.Status.ToString(),
                    Locations = supplier.SupplierLocations.Select(x => new DropDownDTO
                    {
                        Name = x.Location.Name,
                        Code = x.Location.Code,
                        Id = x.LocationId
                    }).ToList(),
                };
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supplier with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the supplier");
            }
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult> CreateEmployee([FromBody] CreateSupplierDto createDto)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) { return Unauthorized(); }
                // Check if location exists
                var queriableLocations = _locationRepository.GetAll();

                queriableLocations = queriableLocations.Where(x => createDto.Locations.Contains(x.Id));

                if (!queriableLocations.Any()) { return BadRequest(new { error = $"No Shop found" }); }

                var Code = await  _supplierRepository.GenerateCodeAsync((Guid)user.CompanyId);

                var currentUser = await _userRepository.GetUserByRefreshTokenAsync();
                if (currentUser is null)  return Unauthorized();

                var newSupplier = Supplier.Create(Guid.NewGuid(), createDto.SupplierComanyName,(Guid)(currentUser is not null ? currentUser.CompanyId : Guid.Empty),  createDto.FirstName, createDto.LastName,
                    createDto.Email ?? "", createDto.Phone, createDto.Tin??"", Code, createDto.Status,  createDto.Address, DateTime.UtcNow, Guid.Parse(currentUser.Id));

                var supplier = await _supplierRepository.CreateAsync(newSupplier);

                var locationsId = queriableLocations.Select(x => x.Id).ToList();

                var SupplierLocations = locationsId.Select(x => SupplierLocation.Create(Guid.NewGuid(), x, newSupplier.Id, DateTime.UtcNow, Guid.Parse(currentUser.Id), true)).ToList();

                await _supplierLocationRepository.AddRangeAsync(SupplierLocations);

                await transaction.CommitAsync();

                return StatusCode(201, new { id = supplier.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating supplier");
                return StatusCode(500, "An error occurred while creating the supplier");
            }
        }

        // PUT: api/employees/{id}
        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateSupplierDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var supplier = await _supplierRepository.GetByIdAsync(updateDto.Id) ;
                if (supplier == null) return NotFound($"Supplier not found");

                // Check if location exists
                var queriableLocations = _locationRepository.GetAll();

                queriableLocations = queriableLocations.Where(x => updateDto.Locations.Contains(x.Id));

                if (!queriableLocations.Any()) { return BadRequest(new { error = $"No Shop found" }); }

                var currentUserId = await _userRepository.GetCurrentUserId();

                supplier.Update(updateDto.SupplierComanyName, updateDto.FirstName, updateDto.LastName, updateDto.Email ?? "", updateDto.Phone,
                updateDto?.Tin??"", updateDto.Status,  updateDto.Address,DateTime.UtcNow, currentUserId);

                await _supplierRepository.UpdateAsync(supplier);

                var locationsId = queriableLocations.Select(x => x.Id).ToList();

                var newSupLocations = locationsId.Select(x => SupplierLocation.Create(Guid.NewGuid(), x, supplier.Id, DateTime.UtcNow, currentUserId, true)).ToList();

                await _supplierLocationRepository.ManageLocationAccess(updateDto.Locations, supplier.Id, currentUserId);

                await _supplierRepository.UpdateAsync(supplier);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier");
                return StatusCode(500, "An error occurred while updating the supplier");
            }
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var deleted = await _supplierRepository.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Employee not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier ");
                return StatusCode(500, "An error occurred while deleting the supplier");
            }
        }

    }
}

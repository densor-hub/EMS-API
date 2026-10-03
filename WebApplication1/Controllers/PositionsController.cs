using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ILogger<PositionsController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public PositionsController(
            IPositionRepository positionRepository,
            ILogger<PositionsController> logger,
            IUserRepository userRepository,
             AppDbContext context)
        {
            _positionRepository = positionRepository;
            _logger = logger;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetPositionsDto>>> GetAllPositionsOrRoles([FromQuery] bool OnlyActive = true)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) { return Unauthorized(); }

                var positions =  _positionRepository.GetAll();

                if (OnlyActive) positions = positions.Where(x => x.Status);

                var returnData = positions
                    .Include(x=> x.PositionRoutes)
                    .Where(x=>x.CompanyId == user.CompanyId && x.GeneralStatus == GeneralStatus.Active).Select(p => new GetPositionsDto
                {
                    Id = p.Id,
                    Name = p.Title,
                    Permissions = p.PositionRoutes.Select(x=> x.AppRouteId).ToList(),
                    Status  = p.Status,
                    Description = p.Description
                });
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customer");
                return StatusCode(500, "An error occurred while retrieving customer");
            }
        }

        [HttpGet("ForTransactions")]
        public async Task<ActionResult<IEnumerable<GetPositionsDto>>> GetAllPositionsOrRolesForTransactions()
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var positions = _positionRepository.GetAll();

                var returnData = positions
                    .Include(x => x.PositionRoutes)
                    .Where(x => x.CompanyId == user.CompanyId && x.GeneralStatus == GeneralStatus.Active && x.Status).Select(p => new GetPositionsDto
                    {
                        Id = p.Id,
                        Name = p.Title,
                        Permissions = p.PositionRoutes.Select(x => x.AppRouteId).ToList(),
                        Status = p.Status,
                        Description = p.Description
                    });
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customer");
                return StatusCode(500, "An error occurred while retrieving customer");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DropDownDTO>> GetEmployeeById(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var position = await _positionRepository.GetByIdAsync(id);

                var returnData =  new DropDownDTO
                {
                    Id = position.Id,
                    Name = position.Title
                    // Code = p.C
                };
                return Ok(returnData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer");
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateEmployee([FromBody] CreatePositionDTO createDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) return Unauthorized();

                var currentUserId = Guid.Parse(user.Id);

                var newPosition = Position.Create(
                    Guid.NewGuid(),
                    createDto.Title,
                    (Guid)user.CompanyId,
                    createDto.Status,
                    DateTime.UtcNow,
                    currentUserId,
                    createDto?.Description ?? ""
                );

                await _positionRepository.AddAsync(newPosition);

                if (createDto?.Routes is not null && createDto.Routes.Any())
                {
                    var positionRoutes = createDto.Routes
                        .Distinct()
                        .Select(routeId => PositionRoutes.Create(
                            Guid.NewGuid(),
                            newPosition.Id,
                            routeId,
                            DateTime.UtcNow,
                            currentUserId))
                        .ToList();

                    await _context.PositionRoutes.AddRangeAsync(positionRoutes);
                }

                // ✅ THE MISSING LINE — without this, route inserts are never sent to the DB
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return StatusCode(200, new { id = newPosition.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating position");
                return StatusCode(500, "An error occurred while creating the position");
            }
        }
        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdatePositionDTO Dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) return Unauthorized();

                var position = await _positionRepository.GetByIdAsync(Dto.Id);
                if (position == null) return NotFound("Role not found");

                var currentUserId = Guid.Parse(user.Id);

                position.Update(Dto.Title, Dto.Status, DateTime.UtcNow, currentUserId, Dto?.Description ?? "");

                // ✅ Only touch routes if the client explicitly sent a list.
                // A `null` routes field means "don't change routes"; an empty array means "remove all".
                if (Dto.Routes is not null)
                {
                    var existing = _context.PositionRoutes
                        .Where(x => x.PositionId == position.Id); // ⚠️ NO AsNoTracking — see Issue 2
                    _context.PositionRoutes.RemoveRange(existing);

                    if (Dto.Routes.Any())
                    {
                        var positionRoutes = Dto.Routes
                            .Distinct()
                            .Select(routeId => PositionRoutes.Create(
                                Guid.NewGuid(),
                                position.Id,
                                routeId,
                                DateTime.UtcNow,
                                currentUserId))
                            .ToList();

                        await _context.PositionRoutes.AddRangeAsync(positionRoutes);
                    }
                }

                await _positionRepository.UpdateAsync(position);

                await _context.SaveChangesAsync();   // ✅ ensure pending changes are flushed
                await transaction.CommitAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating position");
                return StatusCode(500, "An error occurred while updating the position");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var deleted = await _positionRepository.GetByIdAsync(id);
                if (deleted == null) return NotFound($"Customer = not found");

                await _positionRepository.DeleteAsync(deleted);
                await _positionRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                return StatusCode(500, "An error occurred while deleting the customer");
            }
        }

    }
}

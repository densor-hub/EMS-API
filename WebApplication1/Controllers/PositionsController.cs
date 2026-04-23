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
        public async Task<ActionResult<IEnumerable<GetPositionsDto>>> GetAllEmployees()
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                var positions =  _positionRepository.GetAll();

                var returnData = positions
                    .Include(x=> x.PositionRoutes)
                    .Where(x=>x.CompanyId == user.CompanyId && x.Status == true).Select(p => new GetPositionsDto
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

                var newPosition= Position.Create(Guid.NewGuid(), createDto.Title, (Guid)user?.CompanyId, createDto.Status, DateTime.UtcNow, Guid.Parse(user.Id), createDto?.Description??"");

                 await _positionRepository.AddAsync(newPosition); 

                if (createDto?.Routes is not null)
                {
                    if (createDto.Routes.Any())
                    {
                        var positionRoustes = new List<PositionRoutes>();

                        foreach (var route in createDto.Routes)
                        {
                            var positionRoute = PositionRoutes.Create(Guid.NewGuid(), newPosition.Id, route, DateTime.UtcNow, Guid.Parse(user.Id));
                            positionRoustes.Add(positionRoute);

                        }

                        await _context.PositionRoutes.AddRangeAsync(positionRoustes);
                    }
                }
               

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return StatusCode(200, new { id = newPosition.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "An error occurred while creating the customer");
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

                var position = await _positionRepository.GetByIdAsync(Dto.Id) ;
                if (position == null) return NotFound($"Role not found");

                var currentUserId = await _userRepository.GetCurrentUserId();

                position.Update(Dto.Title, Dto.Status, DateTime.UtcNow, Guid.Parse(user.Id), Dto?.Description ?? "");


                var existing = _context.PositionRoutes.Where(x => x.PositionId == position.Id).AsNoTracking();
                _context.PositionRoutes.RemoveRange(existing);


                if (Dto?.Routes is not null)
                {
                    if (Dto.Routes.Any())
                    {
                        

                        var positionRoustes = new List<PositionRoutes>();

                        foreach (var route in Dto.Routes)
                        {
                            var positionRoute = PositionRoutes.Create(Guid.NewGuid(), position.Id, route, DateTime.UtcNow, Guid.Parse(user.Id));
                            positionRoustes.Add(positionRoute);

                        }

                        await _context.PositionRoutes.AddRangeAsync(positionRoustes);
                    }
                }

                await _positionRepository.UpdateAsync(position);


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating customer");
                return StatusCode(500, "An error occurred while updating the customer");
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

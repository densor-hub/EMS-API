// Controllers/LocationPaymentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Services;
using WebApplication1.Services.ControllerServices;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EmployeeDisbursementsController : ControllerBase
    {
        private readonly IEmployeeDisbursementService _empDisbursementService;
        private readonly ILogger<EmployeeDisbursementsController> _logger;
        private readonly ITransactionService _transactionService;

        public EmployeeDisbursementsController(
            IEmployeeDisbursementService empDisbursementService,
            ILogger<EmployeeDisbursementsController> logger,
            ITransactionService transactionService)
        {
            _empDisbursementService = empDisbursementService;
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDisbursementResponseDto>>> GetAll([FromRoute] Guid locationId, [FromQuery] Guid receiverId, [FromQuery] TransactionType? transactionType)
        {
            try
            {
                var result = await _empDisbursementService.GetAllAsync(locationId, receiverId, transactionType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all location payments");
                return StatusCode(500, new { message = "An error occurred while retrieving location payments" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDisbursementResponseDto>> GetById(Guid id)
        {
            try
            {
                var result = await _empDisbursementService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location payment with ID {Id}", id);
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateTransactionDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                 await _transactionService.CompleteTransationProcess(createDto, null);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location payment");
                return StatusCode(500, new { message = "An error occurred while creating the location payment" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> Update(Guid id, [FromBody] UpdateEmployeeDisbursementDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _empDisbursementService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating location payment with ID {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _empDisbursementService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting location payment with ID {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
// Controllers/EmployeesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetAllEmployees([FromQuery] Guid? locationId, [FromQuery] bool onlyActive = true)
        {
            try
            {
                var result = await _employeeService.GetAllEmployeesAsync(locationId, onlyActive);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all employees");
                return StatusCode(500, new { message = "An error occurred while retrieving employees" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetEmployeeDto>> GetEmployeeById(Guid id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee with ID {Id}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{locatonId:Guid}")]
        public async Task<ActionResult> CreateEmployee([FromBody] CreateEmployeeDto createDto, [FromRoute] Guid locatonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _employeeService.CreateEmployeeAsync(createDto, locatonId);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = ((dynamic)result).Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee for email: {Email}", createDto.Email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _employeeService.UpdateEmployeeAsync(updateDto);
                return Ok(new { message = "Employee updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
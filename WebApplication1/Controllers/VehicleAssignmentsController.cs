using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs;
using WebApplication1.Application.Interfaces;

namespace WebApplication1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleAssignmentsController : ControllerBase
    {
        private readonly IVehicleAssignmentService _assignmentService;

        public VehicleAssignmentsController(IVehicleAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var assignment = await _assignmentService.GetByIdAsync(id);
            return Ok(assignment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid locationId, [FromQuery] string? type, [FromQuery] Guid? driverId, [FromQuery] string? vehicleNumber)
        {
            var assignments = await _assignmentService.GetAllAsync(locationId, type, driverId,  vehicleNumber);
            return Ok(assignments);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVehicleAssignmentDto createDto)
        {
            var assignment = await _assignmentService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = assignment.Id }, assignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateVehicleAssignmentDto updateDto)
        {
            var assignment = await _assignmentService.UpdateAsync(id, updateDto);
            return Ok(assignment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _assignmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
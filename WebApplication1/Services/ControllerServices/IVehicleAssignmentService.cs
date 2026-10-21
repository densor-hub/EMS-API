using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;

namespace WebApplication1.Services.ControllerServices
{
    public interface IVehicleAssignmentService
    {
        Task<VehicleAssignmentResponseDto> GetByIdAsync(Guid id);
        Task<IEnumerable<VehicleAssignmentResponseDto>>  GetAllAsync(Guid locationId, string type, Guid? driverId, string? vehicleNumber);
        Task<VehicleAssignmentResponseDto> CreateAsync(CreateVehicleAssignmentDto createDto);
        Task<VehicleAssignmentResponseDto> UpdateAsync(Guid id, UpdateVehicleAssignmentDto updateDto);
        Task DeleteAsync(Guid id);
    }
}
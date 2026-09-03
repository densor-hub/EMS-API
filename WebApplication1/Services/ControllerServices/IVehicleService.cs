using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Application.Interfaces
{
    public interface IVehicleService
    {
        // Vehicle Management
        Task<VehicleResponseDto> GetByIdAsync(Guid id);
        Task<IEnumerable<VehicleResponseDto>> GetAllAsync();
        Task<VehicleResponseDto> CreateAsync(CreateVehicleDto createDto);
        Task<VehicleResponseDto> UpdateAsync(Guid id, UpdateVehicleDto updateDto);
        Task DeleteAsync(Guid id);
        Task<bool> IsVehicleNumberExistsAsync(string vehicleNumber);
        Task<bool> IsVehicleAssignedAsync(Guid vehicleId);
        //Task<IEnumerable<VehicleResponseDto>> GetByLocationIdAsync(Guid locationId);
        //Task<IEnumerable<VehicleResponseDto>> GetByTypeAsync(VehicleType type);
        //Task<IEnumerable<VehicleResponseDto>> GetAvailableVehiclesAsync();
        //Task<IEnumerable<VehicleResponseDto>> GetAssignedVehiclesAsync();

        // CRUD Operations


        // Assignment Operations
        //Task<VehicleResponseDto> AssignDriverAsync(Guid vehicleId, Guid driverId);
        //Task<VehicleResponseDto> UnassignDriverAsync(Guid vehicleId);

        // Validation

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.DTOs;
using WebApplication1.Application.Interfaces;
using WebApplication1.DAL.Repository;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Interfaces;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IVehicleAssignmentRepository _vehicleAssignmentRepository;
        private readonly IUserRepository _userRepository;
        public VehicleService(
            IVehicleRepository vehicleRepository,
            IEmployeeRepository employeeRepository,
            ILocationRepository locationRepository,
            IVehicleAssignmentRepository vehicleAssignmentRepository,
             IUserRepository userRepository)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _locationRepository = locationRepository ?? throw new ArgumentNullException(nameof(locationRepository));
            _vehicleAssignmentRepository = vehicleAssignmentRepository ?? throw new ArgumentNullException(nameof(vehicleAssignmentRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(vehicleAssignmentRepository));
        }

        public async Task<VehicleResponseDto> GetByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                throw new KeyNotFoundException($"Vehicle not found");

            return await MapToResponseDto(vehicle);
        }

        public async Task<IEnumerable<VehicleResponseDto>> GetAllAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return await MapToResponseDtos(vehicles);
        }

        public async Task<IEnumerable<VehicleResponseDto>> GetByLocationIdAsync(Guid locationId, string typeOfLoc)
        {
            var vehicles = await _vehicleRepository.GetByLocationIdAsync(locationId, typeOfLoc);
            return await MapToResponseDtos(vehicles);
        }


        public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto createDto)
        {
            // Validate
            if (await IsVehicleNumberExistsAsync(createDto.VehicleNumber))
                throw new InvalidOperationException($"Vehicle number {createDto.VehicleNumber} already exists");

            if (!await _locationRepository.ExistsAsync(createDto.LocationId))
                throw new KeyNotFoundException($"Please note that the Shop for this transaction was not found");

            // Create entity
            var vehicle = Vehicle.Create(
                Guid.NewGuid(),
                createDto.VehicleNumber,
                createDto.Code,
                createDto.LocationId,
                createDto.Type
            );

            await _vehicleRepository.AddAsync(vehicle);
            return await MapToResponseDto(vehicle);
        }

        public async Task<VehicleResponseDto> UpdateAsync(Guid id, UpdateVehicleDto updateDto)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                throw new KeyNotFoundException($"Please note that the Vehicle for this transaction was not found");

            // Validate location
            //if (!await _locationRepository.ExistsAsync(updateDto.LocationId))
            //    throw new KeyNotFoundException($"Location with ID {updateDto.LocationId} not found");

            // Update
            vehicle.Update(updateDto.VehicleNumber, updateDto.Type, updateDto.Status);
            await _vehicleRepository.UpdateAsync(vehicle);

            return await MapToResponseDto(vehicle);
        }

        public async Task DeleteAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                throw new KeyNotFoundException($"Please note that the Vehicle for this transaction was not found");

            if (vehicle.IsAssigned())
                throw new InvalidOperationException("Cannot delete vehicle that is currently assigned to a driver");

            await _vehicleRepository.DeleteAsync(id);
        }

        public async Task<bool> IsVehicleNumberExistsAsync(string vehicleNumber)
        {
            return await _vehicleRepository.VehicleNumberExistsAsync(vehicleNumber);
        }

        public async Task<bool> IsVehicleAssignedAsync(Guid vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {vehicleId} not found");

            return vehicle.IsAssigned();
        }

        // Private helper methods
        private async Task<VehicleResponseDto> MapToResponseDto(Vehicle vehicle)
        {
            var dto = new VehicleResponseDto
            {
                Id = vehicle.Id,
                VehicleNumber = vehicle.VehicleNumber,
                Code = vehicle.Code,
                LocationId = vehicle.LocationId,
                Type = vehicle.Type,
                Status = vehicle.GeneralStatus,
                IsAssigned = vehicle.VehicleAssignments.Any(x=> x.GeneralStatus == GeneralStatus.Active)
            };

            // Get location name if available
            if (vehicle.Location != null)
            {
                dto.LocationName = vehicle.Location.Name;
            }
            else if (vehicle.LocationId != Guid.Empty)
            {
                var location = await _locationRepository.GetByIdAsync(vehicle.LocationId);
                dto.LocationName = location?.Name;
            }

            return dto;
        }

        private async Task<IEnumerable<VehicleResponseDto>> MapToResponseDtos(IEnumerable<Vehicle> vehicles)
        {
            var dtos = new List<VehicleResponseDto>();
            foreach (var vehicle in vehicles)
            {
                dtos.Add(await MapToResponseDto(vehicle));
            }
            return dtos;
        }
    }
}
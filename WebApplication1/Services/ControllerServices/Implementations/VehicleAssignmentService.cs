using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Interfaces;
using WebApplication1.Domain.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class VehicleAssignmentService : IVehicleAssignmentService
    {
        private readonly IVehicleAssignmentRepository _assignmentRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _user;

        public VehicleAssignmentService(
            IVehicleAssignmentRepository assignmentRepository,
            IVehicleRepository vehicleRepository,
            IEmployeeRepository employeeRepository,
            IUserRepository user)
        {
            _assignmentRepository = assignmentRepository;
            _vehicleRepository = vehicleRepository;
            _employeeRepository = employeeRepository;
            _user = user;
        }

        public async Task<VehicleAssignmentResponseDto> GetByIdAsync(Guid id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                throw new KeyNotFoundException($"Assignment with ID {id} not found");

            return await MapToResponseDto(assignment);
        }

        public async Task<IEnumerable<VehicleAssignmentResponseDto>> GetAllAsync(Guid locationId, string type, Guid? driverId, string? vehicleNumber)
        {
            var data = await _assignmentRepository.GetAllAsync();

            if (type != null && type.ToLower().Trim() == "company")
            {
                data= data.Where(x => x.Vehicle.Location.CompanyId == locationId);
            }

            if (driverId.HasValue && driverId != Guid.Empty) data = data.Where(x => x.DriverId == driverId);
            if (!string.IsNullOrEmpty(vehicleNumber)) data = data.Where(x => x.Vehicle.VehicleNumber == vehicleNumber);

             data = data.Where(x => x.Vehicle.LocationId == locationId);

            return data.Select(x=> new VehicleAssignmentResponseDto
            {
                AssignedtDate = x.AssignedtDate,
                DriverId = x.DriverId,
                DriverName = $"{x.Driver.FirstName} {x.Driver.LastName}",
                Id = x.Id,
                Status = x.GeneralStatus.ToString(),
                UnassignedDate = x.UnassignedDate,
                VehicleId = x.VehicleId,
                VehicleNumber = x.Vehicle.VehicleNumber,
            });
        }

        public async Task<VehicleAssignmentResponseDto> CreateAsync(CreateVehicleAssignmentDto createDto)
        {
            // Validate driver exists
            var driver = await _employeeRepository.GetByIdAsync(createDto.DriverId);
            if (driver == null)
                throw new KeyNotFoundException($"Driver with ID {createDto.DriverId} not found");

            // Validate vehicle exists
            var vehicle = await _vehicleRepository.GetByIdAsync(createDto.VehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {createDto.VehicleId} not found");

            // Create assignment
            var assignment = VehicleAssignments.Create(
                Guid.NewGuid(),
                createDto.DriverId,
                createDto.VehicleId,
                createDto.AssignedtDate,
                createDto.CreatedBy,
                DateTime.UtcNow
            );

            await _assignmentRepository.AddAsync(assignment);

            await _assignmentRepository.SaveChangesAsync();
            return await MapToResponseDto(assignment);
        }

        public async Task<VehicleAssignmentResponseDto> UpdateAsync(Guid id, UpdateVehicleAssignmentDto updateDto)
        {
            var user = await _user.GetUserByRefreshTokenAsync();
            if (user == null) throw new Exception("Unautorized");
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                throw new KeyNotFoundException($"Assignment with ID {id} not found");

            assignment.AssignMentStatatus(Guid.Parse(user.Id), updateDto.Status);

            await _assignmentRepository.UpdateAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();
            return await MapToResponseDto(assignment);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _assignmentRepository.DeleteAsync(id);
        }

        // Helper methods
        private async Task<VehicleAssignmentResponseDto> MapToResponseDto(VehicleAssignments assignment)
        {
            var dto = new VehicleAssignmentResponseDto
            {
                Id = assignment.Id,
                DriverId = assignment.DriverId,
                VehicleId = assignment.VehicleId,
                AssignedtDate = assignment.AssignedtDate,
                UnassignedDate = assignment.UnassignedDate,
                Status = assignment.GeneralStatus.ToString()
            };

            if (assignment.Driver != null)
            {
                dto.DriverName = $"{assignment.Driver.FirstName} {assignment.Driver.LastName}";
            }
            else if (assignment.DriverId != Guid.Empty)
            {
                var driver = await _employeeRepository.GetByIdAsync(assignment.DriverId);
                if (driver != null)
                {
                    dto.DriverName = $"{driver.FirstName} {driver.LastName}";
                }
            }

            if (assignment.Vehicle != null)
            {
                dto.VehicleNumber = assignment.Vehicle.VehicleNumber;
            }
            else if (assignment.VehicleId != Guid.Empty)
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(assignment.VehicleId);
                if (vehicle != null)
                {
                    dto.VehicleNumber = vehicle.VehicleNumber;
                }
            }

            return dto;
        }

        private async Task<IEnumerable<VehicleAssignmentResponseDto>> MapToResponseDtos(IQueryable<VehicleAssignments> assignments)
        {
            var dtos = new List<VehicleAssignmentResponseDto>();
            foreach (var assignment in assignments)
            {
                dtos.Add(await MapToResponseDto(assignment));
            }
            return dtos;
        }

       
    }
}
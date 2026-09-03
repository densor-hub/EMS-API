// DTOs/VehicleDtos.cs
using WebApplication1.Domain.Enums;

namespace WebApplication1.Application.DTOs
{
    public class CreateVehicleDto
    {
        public string VehicleNumber { get; set; }
        public string Code { get; set; }
        public Guid LocationId { get; set; }
        public VehicleType Type { get; set; }
    }

    public class UpdateVehicleDto
    {
        public string VehicleNumber { get; set; }
        public VehicleType Type { get; set; }
        public GeneralStatus Status { get; set; }
    }

    public class VehicleResponseDto
    {
        public Guid Id { get; set; }
        public string VehicleNumber { get; set; }
        public string Code { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public VehicleType Type { get; set; }
        public bool IsAssigned { get; set; }
        public GeneralStatus Status { get; set; }
    }

    public class AssignVehicleDto
    {
        public Guid VehicleId { get; set; }
        public Guid DriverId { get; set; }
    }

    public class UnassignVehicleDto
    {
        public Guid VehicleId { get; set; }
    }
}
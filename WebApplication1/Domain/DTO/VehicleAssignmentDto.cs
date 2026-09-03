using System;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Application.DTOs
{
    public class VehicleAssignmentResponseDto
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public string DriverName { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime AssignedtDate { get; set; }
        public DateTime UnassignedDate { get; set; }
        public string Status { get; set; }
    }

    public class CreateVehicleAssignmentDto
    {
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime AssignedtDate { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class UpdateVehicleAssignmentDto
    {
        public bool Status { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
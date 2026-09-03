namespace WebApplication1.Domain.Entities
{
    public class VehicleAssignments  : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid DriverId { get; private set; }
        public Employee Driver { get; private set; }
        public Guid VehicleId { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public DateTime AssignedtDate { get; private set; }
        public DateTime UnassignedDate { get; private set; }

        private VehicleAssignments()
        {
            
        }

        private VehicleAssignments(Guid id, Guid driverId, Guid vehicleId, DateTime assignedDate, Guid createdBy, DateTime createdAt )
        {
            Id= id;
            DriverId=driverId;
            VehicleId=vehicleId;
            AssignedtDate=assignedDate;
            CreatedAt=createdAt;
            CreatedBy=createdBy; 
        }

        public static VehicleAssignments Create(Guid id, Guid driverId, Guid vehicleId, DateTime assignedDate, Guid createdBy, DateTime createdAt)
        => new VehicleAssignments(id, driverId, vehicleId, assignedDate, createdBy, createdAt);

        public void AssignMentStatatus (Guid updatedBy, bool status)
        {
            GeneralStatus = !status ? Enums.GeneralStatus.Inactive : Enums.GeneralStatus.Active;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

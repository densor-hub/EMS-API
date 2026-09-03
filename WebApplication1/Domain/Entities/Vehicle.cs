using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public Guid Id { get;  private set; }
        public string VehicleNumber { get; private set; }
        public string Code { get; private set; }
        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        public VehicleType Type { get; private set; }
        public virtual ICollection<VehicleAssignments> VehicleAssignments { get; private set; }

        private Vehicle()
        {
            
        }

        private Vehicle(Guid id, string vehicleNumber, string code, Guid locationId, VehicleType type)
        {
            Id = id;
            VehicleNumber = vehicleNumber;
            Code = code;    
            LocationId = locationId;
            Type = type;
        }

        public static Vehicle Create(Guid id, string vehicleNumber, string code, Guid locationId, VehicleType type)
        => new Vehicle(id, vehicleNumber, code, locationId, type);

        public void Update(string vehicleNumber,  VehicleType type, GeneralStatus status)
        {
            VehicleNumber = vehicleNumber;
            Type = type;
            GeneralStatus = status;
        }

        public bool IsAssigned()
        {
            return VehicleAssignments.Any(x => x.GeneralStatus == GeneralStatus.Active);
        }
    }
}

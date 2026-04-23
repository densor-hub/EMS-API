namespace WebApplication1.Domain.Entities
{
    public class LocationManangement : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid ManagerId { get; private set; }
        public Employee Manager { get; private set; }
        public Guid LocationId { get; private set; }
        public Location Location { get; private set; }
        public bool Status { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsMainManager { get; private set; }

        public LocationManangement()
        {
            
        }

        public LocationManangement(Guid id, Guid managerId, Guid locatiionId, bool status, DateTime startDate, DateTime? endDate, DateTime createdAt, Guid createdBy, bool isMainManager)
        {
            Id = id;
            ManagerId = managerId;
            LocationId = locatiionId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Status = status;
            StartDate = startDate;
            EndDate = endDate;
            IsMainManager = isMainManager;
        }

        public static LocationManangement Create(Guid id, Guid managerId, Guid locatiionId, bool status, DateTime startDate, DateTime? endDate, DateTime createdAt, Guid createdBy, bool isMainManager)
        => new LocationManangement(id, managerId, locatiionId, status, startDate, endDate, createdAt, createdBy, isMainManager );
    }
}

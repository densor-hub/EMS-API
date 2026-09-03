namespace WebApplication1.Domain.Entities
{
    public class EmployeeLocation : BaseEntity
    {
        public Guid EmployeeId { get; private set; }
        public virtual Employee Employee { get; private set; }
        public Guid LocationId { get; private set; }
        public virtual Location Location { get; private set; }
        public bool Status { get; private set; }
        public bool IsManager { get; private set; }
        //public bool status { get; private set; }

        public EmployeeLocation()
        {
            
        }
        public EmployeeLocation(Guid id, Guid locationId, Guid employeeId, DateTime createdAt, Guid createdBy, bool status , bool isManager)
        {
            Id = id;
            LocationId = locationId;
            EmployeeId = employeeId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
            Status = status;
            IsManager = isManager;
        }

        public static EmployeeLocation Create(Guid id, Guid locationId, Guid employeeId, DateTime createdAt, Guid createdBy, bool status, bool isManager)
            => new EmployeeLocation(id, locationId, employeeId, createdAt, createdBy, status, isManager);

        public void RemoveAccess()
        {
            Status = false;
        }
        public void ActivateAccess()
        {
            Status = true;
        }

        public void SetIsManager(bool isManager)
        {
            IsManager = isManager;
        }
    }
}

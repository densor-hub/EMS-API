namespace WebApplication1.Domain.Entities
{
    public class PositionRoutes
    {
        public Guid Id { get;  private set; }
        public Guid PositionId { get; private set;}
        public Position Position { get; private set;}
        public Guid AppRouteId { get; private set;}
        public ApplicationRoutes ApplicationRoutes { get; private set;}
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }
        public virtual ICollection<UserRoutes> UserRoutes { get; private set; }

        public PositionRoutes()
        {
            
        }

        private PositionRoutes(Guid id, Guid positionId, Guid appRouteId, DateTime createdAt,  Guid createdBy)
        {
            Id = id;
            PositionId = positionId;
            AppRouteId = appRouteId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static PositionRoutes Create(Guid id, Guid positionId, Guid appRouteId, DateTime createdAt, Guid createdBy)
        => new PositionRoutes(id, positionId, appRouteId, createdAt, createdBy);
    }
}

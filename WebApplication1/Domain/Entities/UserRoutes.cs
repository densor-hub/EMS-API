namespace WebApplication1.Domain.Entities
{
    public class UserRoutes
    {
        public Guid Id { get;  private set; }
        public string UserId { get; private set;}
        public ApplicationUser User { get; private set;}
        public Guid? PositionRouteId { get; private set;}
        public virtual PositionRoutes PositionRoutes { get; private set;}
        public DateTime CreatedAt { get; private set; }
        public Guid CreatedBy { get; private set; }

        public UserRoutes()
        {
            
        }

        private UserRoutes(Guid id, Guid? positionRouteId, string userId, DateTime createdAt,  Guid createdBy)
        {
            Id = id;
            PositionRouteId = positionRouteId;
            UserId = userId;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public static UserRoutes Create(Guid id, Guid? positionRouteId, string userId, DateTime createdAt, Guid createdBy)
        => new UserRoutes(id, positionRouteId, userId, createdAt, createdBy);
    }
}

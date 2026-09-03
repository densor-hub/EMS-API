using WebApplication1.Domain.DTO;

namespace WebApplication1.Domain.Entities
{
    public class ApplicationRoute
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        public double Level { get; set; }  // This now serves as OrderKey
        public Guid? ParentId { get; set; } = null;
        public virtual ApplicationRoute? Parent { get; set; }
        public string? Path { get; set; }
        public virtual ICollection<ApplicationRoute>? Children { get; set; }
        public virtual ICollection<PositionRoutes>? PositionRoutes { get; set; }

        // Private parameterless constructor for EF Core
        private ApplicationRoute() { }

        // Private constructor with parameters
        private ApplicationRoute(
            Guid id,
            string title,
            bool status,
            double level,
            Guid? parentId,
            string? path)
        {
            Id = id;
            Title = title;
            Status = status;
            Level = level;
            ParentId = parentId;
            Path = path;
        }

        // Factory method to create a new ApplicationRoute
        public static ApplicationRoute Create(
            Guid id,
            string title,
            bool status,
            double level,
            Guid? parentId,
            string? path)
        {
            // Validate required fields
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty", nameof(id));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (level < 0)
                throw new ArgumentException("Level must be 0 or greater", nameof(level));

            return new ApplicationRoute(
                id,
                title,
                status,
                level,
                parentId,
                path);
        }

        // Method to update the ApplicationRoute
        public void Update(
            string title,
            bool status,
            int level,
            Guid? parentId,
            string? path)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            if (level < 0)
                throw new ArgumentException("Level must be 0 or greater", nameof(level));

            Title = title;
            Status = status;
            Level = level;
            ParentId = parentId;
            Path = path;
        }

        // Method to update specific properties if needed
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            Title = title;
        }

        public void UpdateStatus(bool status)
        {
            Status = status;
        }

        public void UpdateLevel(int level)
        {
            if (level < 0)
                throw new ArgumentException("Level must be 0 or greater", nameof(level));
            Level = level;
        }

        public void UpdateParent(Guid? parentId)
        {
            ParentId = parentId;
        }

        public void UpdatePath(string? path)
        {
            Path = path;
        }
    }
}
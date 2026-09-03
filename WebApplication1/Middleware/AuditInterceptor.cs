using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Services.ControllerServices;
using WebApplication1.Domain.Repository;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IAuditService auditService, IHttpContextAccessor httpContextAccessor)
    {
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return result;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity)
            .ToList();

        foreach (var entry in entries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            var currentUser = GetCurrentUser();

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.CreatedBy = Guid.Parse(currentUser.Id);
                    await _auditService.LogChangeAsync(entity, "Create");
                    break;

                case EntityState.Modified:
                    entity.UpdatedAt = DateTime.UtcNow;
                    entity.UpdatedBy = Guid.Parse(currentUser.Id);

                    // Get original values
                    var originalValues = entry.OriginalValues.ToObject();
                    var currentValues = entry.CurrentValues.ToObject();

                    await _auditService.LogChangesAsync(originalValues, currentValues, "Update");
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = DateTime.UtcNow;
                    entity.DeletedBy = Guid.Parse(currentUser.Id);

                    await _auditService.LogDeleteAsync(entity);
                    break;
            }
        }

        return result;
    }

    private (string Id, string Name) GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        if (user == null) return ("System", "System");

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        var userName = user.FindFirst(ClaimTypes.Name)?.Value ?? userId;

        return (userId, userName);
    }
}
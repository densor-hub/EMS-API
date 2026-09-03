using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.ControllerServices
{
    public interface IAuditService
    {
        Task LogChangeAsync<T>(T entity, string action, string reason = null) where T : class;
        Task LogChangesAsync<T>(T oldEntity, T newEntity, string action, string reason = null) where T : class;
        Task LogDeleteAsync<T>(T entity, string reason = null) where T : class;
        Task LogCustomEventAsync(string entityType, string entityId, string action, string reason = null, object details = null);
        Task<List<AuditLog>> GetAuditTrailAsync(string entityType, string entityId);
        Task<AuditLog> GetLatestAuditAsync(string entityType, string entityId, string action = null);
        Task<AuditSnapshot> CreateSnapshotAsync<T>(T entity, string reason = null) where T : class;
        Task<T> RestoreFromSnapshotAsync<T>(Guid snapshotId) where T : class;
        Task<List<AuditLog>> SearchAuditAsync(AuditSearchCriteriaDto criteria);
        Task<AuditStatisticsDto> GetStatisticsAsync(DateTime from, DateTime to);
    }
}

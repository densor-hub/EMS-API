using Microsoft.EntityFrameworkCore;
using StyleCop.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Text.Json;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuditService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // Configure JSON options
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task LogChangeAsync<T>(T entity, string action, string reason = null) where T : class
        {
            try
            {
                var entityType = GetEntityTypeName<T>();
                var entityId = GetEntityId(entity);
                var currentUser = GetCurrentUser();

                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = action,
                    NewValue = JsonSerializer.Serialize(entity, _jsonOptions),
                    ChangedBy = currentUser.Id,
                    ChangedByName = currentUser.Name,
                    ChangedAt = DateTime.UtcNow,
                    Reason = reason,
                    Metadata = GetRequestMetadata(),
                    Details = GetPropertyChanges(null, entity)
                };

                await _context.AuditLogs.AddAsync(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging change for entity type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task LogChangesAsync<T>(T oldEntity, T newEntity, string action, string reason = null) where T : class
        {
            try
            {
                var entityType = GetEntityTypeName<T>();
                var entityId = GetEntityId(newEntity ?? oldEntity);
                var currentUser = GetCurrentUser();

                var oldJson = oldEntity != null ? JsonSerializer.Serialize(oldEntity, _jsonOptions) : null;
                var newJson = newEntity != null ? JsonSerializer.Serialize(newEntity, _jsonOptions) : null;

                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = action,
                    OldValue = oldJson,
                    NewValue = newJson,
                    ChangedBy = currentUser.Id,
                    ChangedByName = currentUser.Name,
                    ChangedAt = DateTime.UtcNow,
                    Reason = reason,
                    Metadata = GetRequestMetadata(),
                    Details = GetPropertyChanges(oldEntity, newEntity)
                };

                await _context.AuditLogs.AddAsync(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging changes for entity type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task LogDeleteAsync<T>(T entity, string reason = null) where T : class
        {
            try
            {
                var entityType = GetEntityTypeName<T>();
                var entityId = GetEntityId(entity);
                var currentUser = GetCurrentUser();

                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = "Delete",
                    OldValue = JsonSerializer.Serialize(entity, _jsonOptions),
                    ChangedBy = currentUser.Id,
                    ChangedByName = currentUser.Name,
                    ChangedAt = DateTime.UtcNow,
                    Reason = reason ?? "Entity deleted",
                    Metadata = GetRequestMetadata(),
                    Details = GetPropertyChanges(entity, null)
                };

                await _context.AuditLogs.AddAsync(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging delete for entity type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task LogCustomEventAsync(string entityType, string entityId, string action,
            string reason = null, object details = null)
        {
            try
            {
                var currentUser = GetCurrentUser();

                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = action,
                    ChangedBy = currentUser.Id,
                    ChangedByName = currentUser.Name,
                    ChangedAt = DateTime.UtcNow,
                    Reason = reason,
                    Metadata = GetRequestMetadata(),
                    Details = details != null ?
                        GetPropertyChanges(null, details) :
                        new List<AuditDetail>()
                };

                if (details != null)
                {
                    audit.NewValue = JsonSerializer.Serialize(details, _jsonOptions);
                }

                await _context.AuditLogs.AddAsync(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging custom event for {EntityType}", entityType);
                throw;
            }
        }

        public async Task<List<AuditLog>> GetAuditTrailAsync(string entityType, string entityId)
        {
            try
            {
                return await _context.AuditLogs
                    .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                    .OrderByDescending(a => a.ChangedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit trail for {EntityType} {EntityId}", entityType, entityId);
                throw;
            }
        }

        public async Task<AuditLog> GetLatestAuditAsync(string entityType, string entityId, string action = null)
        {
            try
            {
                var query = _context.AuditLogs
                    .Where(a => a.EntityType == entityType && a.EntityId == entityId);

                if (!string.IsNullOrEmpty(action))
                    query = query.Where(a => a.Action == action);

                return await query
                    .OrderByDescending(a => a.ChangedAt)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest audit for {EntityType} {EntityId}", entityType, entityId);
                throw;
            }
        }

        public async Task<AuditSnapshot> CreateSnapshotAsync<T>(T entity, string reason = null) where T : class
        {
            try
            {
                var snapshot = new AuditSnapshot
                {
                    Id = Guid.NewGuid(),
                    EntityType = GetEntityTypeName<T>(),
                    EntityId = GetEntityId(entity),
                    Snapshot = JsonSerializer.Serialize(entity, _jsonOptions),
                    SnapshotAt = DateTime.UtcNow,
                    TakenBy = GetCurrentUser().Id,
                    Reason = reason ?? "Snapshot taken"
                };

                await _context.AuditSnapshots.AddAsync(snapshot);
                await _context.SaveChangesAsync();

                return snapshot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating snapshot for entity type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T> RestoreFromSnapshotAsync<T>(Guid snapshotId) where T : class
        {
            try
            {
                var snapshot = await _context.AuditSnapshots
                    .FirstOrDefaultAsync(s => s.Id == snapshotId);

                if (snapshot == null)
                    throw new Exception($"Snapshot {snapshotId} not found");

                return JsonSerializer.Deserialize<T>(snapshot.Snapshot, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring from snapshot {SnapshotId}", snapshotId);
                throw;
            }
        }

        public async Task<List<AuditLog>> SearchAuditAsync(AuditSearchCriteriaDto criteria)
        {
            try
            {
                var query = _context.AuditLogs.AsQueryable();

                if (!string.IsNullOrEmpty(criteria.EntityType))
                    query = query.Where(a => a.EntityType == criteria.EntityType);

                if (!string.IsNullOrEmpty(criteria.EntityId))
                    query = query.Where(a => a.EntityId == criteria.EntityId);

                if (!string.IsNullOrEmpty(criteria.Action))
                    query = query.Where(a => a.Action == criteria.Action);

                if (!string.IsNullOrEmpty(criteria.ChangedBy))
                    query = query.Where(a => a.ChangedBy == criteria.ChangedBy);

                if (criteria.FromDate.HasValue)
                    query = query.Where(a => a.ChangedAt >= criteria.FromDate.Value);

                if (criteria.ToDate.HasValue)
                    query = query.Where(a => a.ChangedAt <= criteria.ToDate.Value);

                if (!string.IsNullOrEmpty(criteria.SearchTerm))
                {
                    var searchTerm = criteria.SearchTerm.ToLower();
                    query = query.Where(a =>
                        (a.NewValue != null && a.NewValue.ToLower().Contains(searchTerm)) ||
                        (a.OldValue != null && a.OldValue.ToLower().Contains(searchTerm)) ||
                        (a.Reason != null && a.Reason.ToLower().Contains(searchTerm)) ||
                        (a.ChangedByName != null && a.ChangedByName.ToLower().Contains(searchTerm))
                    );
                }

                return await query
                    .OrderByDescending(a => a.ChangedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching audit logs");
                throw;
            }
        }

        public async Task<AuditStatisticsDto> GetStatisticsAsync(DateTime from, DateTime to)
        {
            try
            {
                var logs = await _context.AuditLogs
                    .Where(a => a.ChangedAt >= from && a.ChangedAt <= to)
                    .ToListAsync();

                return new AuditStatisticsDto
                {
                    TotalChanges = logs.Count,
                    ByEntityType = logs.GroupBy(a => a.EntityType)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    ByAction = logs.GroupBy(a => a.Action)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    ByUser = logs.GroupBy(a => a.ChangedByName ?? "Unknown")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    Period = $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit statistics");
                throw;
            }
        }

        // Helper Methods
        private string GetEntityTypeName<T>()
        {
            // Simply return the class name
            return typeof(T).Name;
        }

        private string GetEntityId<T>(T entity)
        {
            if (entity == null) return null;

            var property = typeof(T).GetProperty("Id");
            if (property == null) return null;

            var value = property.GetValue(entity);
            return value?.ToString();
        }

        private List<AuditDetail> GetPropertyChanges<T>(T oldEntity, T newEntity)
        {
            var changes = new List<AuditDetail>();

            // If both are null, return empty list
            if (oldEntity == null && newEntity == null)
                return changes;

            // Get properties from the type
            var targetType = typeof(T);
            var properties = targetType.GetProperties();

            foreach (var prop in properties)
            {
                // Skip certain properties
                if (prop.Name == "OriginalValues" ||
                    prop.Name == "Id" ||
                    Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                    continue;

                object oldValue = null;
                object newValue = null;

                try
                {
                    if (oldEntity != null)
                    {
                        var oldProp = oldEntity.GetType().GetProperty(prop.Name);
                        oldValue = oldProp?.GetValue(oldEntity);
                    }

                    if (newEntity != null)
                    {
                        var newProp = newEntity.GetType().GetProperty(prop.Name);
                        newValue = newProp?.GetValue(newEntity);
                    }
                }
                catch
                {
                    // Skip properties that can't be accessed
                    continue;
                }

                // Compare values
                if (!Equals(oldValue, newValue))
                {
                    changes.Add(new AuditDetail
                    {
                        PropertyName = prop.Name,
                        OldValue = oldValue?.ToString() ?? "null",
                        NewValue = newValue?.ToString() ?? "null",
                        DataType = prop.PropertyType.Name,
                        IsSensitive = Attribute.IsDefined(prop, typeof(SensitiveDataAttribute))
                    });
                }
            }

            return changes;
        }

        private Dictionary<string, object> GetRequestMetadata()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null) return new Dictionary<string, object>();

            var metadata = new Dictionary<string, object>();

            try
            {
                if (httpContext.Connection?.RemoteIpAddress != null)
                    metadata["IpAddress"] = httpContext.Connection.RemoteIpAddress.ToString();

                if (httpContext.Request?.Headers != null)
                {
                    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                    if (!string.IsNullOrEmpty(userAgent))
                        metadata["UserAgent"] = userAgent;

                    var referer = httpContext.Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrEmpty(referer))
                        metadata["Referer"] = referer;
                }

                if (httpContext.Request != null)
                {
                    metadata["RequestPath"] = httpContext.Request.Path.ToString();
                    metadata["RequestMethod"] = httpContext.Request.Method;
                    metadata["RequestScheme"] = httpContext.Request.Scheme;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting request metadata");
                // Continue with whatever metadata we could collect
            }

            return metadata;
        }

        private (string Id, string Name) GetCurrentUser()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var user = httpContext?.User;

                if (user == null || !user.Identity.IsAuthenticated)
                    return ("System", "System");

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = user.FindFirst(ClaimTypes.Name)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;

                // If we don't have a name, use email or fallback to ID
                if (string.IsNullOrEmpty(userName))
                    userName = email ?? userId ?? "Unknown";

                return (userId ?? "Unknown", userName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting current user");
                return ("System", "System");
            }
        }
    }
}
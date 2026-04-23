
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using WebApplication1.DAL;
using WebApplication1.Services.Hubs;
using WebApplication1.Services.Hubs.DTO;
using WebApplication1.Services.Hubs.Entities;
using WebApplication1.Services.Hubs.Repositories;

namespace ERDMS.Modules.Messaging.Core.DAL.Repositories;

[Authorize]
internal class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Notification> _notifications;
    //private readonly IUserContext _user;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(
        AppDbContext context,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationRepository> logger
        )
    {
        _context = context;
        _notifications = _context.Notifications;
        _hubContext = hubContext;
        _logger = logger;
    }

    public Task<Notification> GetAsync(Guid userid, string notificationType)
    {
        return _notifications.SingleOrDefaultAsync(x =>x.Receiver == userid && x.Type == notificationType );
    }

    public async Task AddAsync(Notification message)
    {
        await _notifications.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(IEnumerable<Notification> notifications)
    {
        _notifications.UpdateRange(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(Guid receiverId, Guid appId, bool? isRead = false)
    {
        var notificatons = _notifications.Where(x => x.Receiver== receiverId  && x.CreatedAt.Date == DateTime.UtcNow.Date  );

        if (isRead == false || isRead == true)
        {
            notificatons = notificatons.Where(x => x.IsRead == isRead);
        }

        await Task.CompletedTask;
        return notificatons.OrderBy(x => x.CreatedAt); ;
    }

    public Task<Notification> GetAsync(Guid notificationId)
    {
        var notification = _notifications.FirstOrDefaultAsync(x => x.Id == notificationId  && x.IsRead == false);
        if (notification is not null)
        {
            return notification;
        }

        return null;

    }

    public async Task UpdateAsync(Notification notification)
    {
        _notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task ReceiveNotifications(ReceiveNotificationsDTO connection)
    {

        try
        {
            var connectionId = $"{connection.CompanyId}{connection.AppId}";

            var notifications = await GetNotificationsAsync(connection.CompanyId, connection.AppId, false);

            var returnData = notifications.OrderByDescending(x => x.CreatedAt).Select(x => new NotificationDto
            {
                CreatedAt = x.CreatedAt,
                Id = x.Id,
                Message = x.Message,
                Path = x.FrontEndPath,
                ActivityReferenceId = x.ActivityId,
                SenderName = x.SenderName,
                Type = x.Type
                
            }).ToList();

            await _hubContext.Clients.Group(connectionId).SendAsync("ReceiveNotifications", returnData);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending notification: {ex.Message}");
        }



        await Task.CompletedTask;
    }



    public async Task ReadNotification(ReadNotificationDTO connection)
    {
        var notification = await GetAsync((Guid)connection.NotificationId);

        if (notification is not null)
        {
            notification.Read();

            await UpdateAsync(notification);

            await _hubContext.Clients.All.SendAsync("ReadNotification", new NotificationReadDTO { Read = true });
             
}
        else
        {
            await _hubContext.Clients.All.SendAsync("ReadNotification", new NotificationReadDTO { Read = false });
        }

        await Task.CompletedTask;
    }

    public async Task Notify(Guid companyId, string appId, Notification newNotification)
    {
        var notificationConnection = new ReceiveNotificationsDTO
        {
            AppId = Guid.Parse(appId),
            Room = "notifications",
            NotificationId = newNotification.Id,
            CompanyId = companyId
        };

        await AddAsync(newNotification);

        await ReceiveNotifications(notificationConnection);
    }

    //public async Task<IEnumerable<Notification>> GetMobileExtensionNotificationsAsync(string jsonArray)
    //{
    //    List <MobileExtentionNotificationPayload> payload = JsonSerializer.Deserialize<List<MobileExtentionNotificationPayload>>(jsonArray);

    //    var returnData = new List<Notification>();
    //    foreach (var data in payload) {
    //        if (!string.IsNullOrEmpty(data.CompanyId.ToString()) && !string.IsNullOrEmpty(data.AppId.ToString())) 
    //        {
    //            var notificatons = await GetNotificationsAsync(data.CompanyId, data.AppId, false);
    //            if (notificatons.Any())
    //            {
    //                returnData.AddRange(notificatons);
    //            }
    //        }
           
    //    }

    //    return returnData;
        
    //}


    public async Task<IEnumerable<Notification>> GetByActivityReferenceAsync(Guid activityId)
    {
        var notificatons = _notifications.Where(x => x.ActivityId == activityId);

        await Task.CompletedTask;
        return notificatons.OrderBy(x => x.CreatedAt); ;
    }
}
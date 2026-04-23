
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Services.Hubs.DTO;
using WebApplication1.Services.Hubs.Entities;

namespace WebApplication1.Services.Hubs.Repositories;

internal interface INotificationRepository
{
    //Task<Notification> GetAsync(string userid, string notificationType, string reference);
    Task<Notification> GetAsync(Guid notificationId);
    Task UpdateAsync(Notification notification);
    Task AddAsync(Notification notification);
    Task UpdateAsync(IEnumerable<Notification> notification);
    Task<IEnumerable<Notification>> GetNotificationsAsync(Guid comapnyId, Guid appId, bool? isRead);
    Task ReadNotification(ReadNotificationDTO connection);
    Task ReceiveNotifications(ReceiveNotificationsDTO connection);
    Task Notify(Guid companyId, string appId, Notification newNotification);
    //Task<IEnumerable<Notification>> GetMobileExtensionNotificationsAsync(string jsonArray);
    Task<IEnumerable<Notification>> GetByActivityReferenceAsync(Guid activityId);

}
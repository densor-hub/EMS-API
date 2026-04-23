
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Services.Hubs.DTO;
using WebApplication1.Services.Hubs.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebApplication1.Services.Hubs;

[Authorize("TokenScopePolicy")]
internal class NotificationHub : Hub
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationHub(
        INotificationRepository notificationRepository
        )
    {
        _notificationRepository = notificationRepository;
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task ReceiveNotifications(ReceiveNotificationsDTO connection)
    {
        await _notificationRepository.ReceiveNotifications(connection);
        await Task.CompletedTask;
    }

    public async Task ReadNotification(ReadNotificationDTO connection)
    {
        await _notificationRepository.ReadNotification(connection);
        await Task.CompletedTask;
    }

}
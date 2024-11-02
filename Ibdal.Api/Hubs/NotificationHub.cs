using Microsoft.AspNetCore.SignalR;

namespace Ibdal.Api.Hubs;

public class NotificationHub(NotificationCreationContext nCtx) : Hub
{
    public async Task SendNotificationToAll(NotificationToAllForm notificationsForm)
    {
        await nCtx.CreateForAllAsync(notificationsForm);
        await Clients.All.SendAsync("ReceiveNotificationForAll", notificationsForm);
    }

    public async Task SendForUsers(NotificationToAllForm notificationToAllForm)
    {
        await nCtx.CreateForUsersAsync(notificationToAllForm);
        await Clients.Group(Constants.Roles.Customer).SendAsync("ReceiveNotificationForUsers", notificationToAllForm);
    }
    
    public async Task SendForStations(NotificationToAllForm notificationToAllForm)
    {
        await nCtx.CreateForStationsAsync(notificationToAllForm);
        await Clients.Group(Constants.Roles.Station).SendAsync("ReceiveNotificationForStation", notificationToAllForm);
    }
}
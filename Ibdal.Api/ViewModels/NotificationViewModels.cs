namespace Ibdal.Api.ViewModels;

public static class NotificationViewModels
{
    public static Expression<Func<Notification, object>> FlatAdminProjection =>
        notification => new
        {
            notification.Id,
            notification.Title,
            notification.Description
        };

    public static Expression<Func<Notification, object>> FlatUserProjection =>
        notification => new
        {
            notification.Id,
            notification.Title,
            notification.Description,
            IsRead = notification.Users.Select(x => x.IsRead)
        };
    
    public static Expression<Func<Notification, object>> FlatStationProjection =>
        notification => new
        {
            notification.Id,
            notification.Title,
            notification.Description,
            IsRead = notification.Stations.Select(x => x.IsRead)
        };
}
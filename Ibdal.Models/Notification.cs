using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Notification : BaseModel
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public IList<NotificationUser> Users { get; set; } = [];
    public IList<NotificationStation> Stations { get; set; } = [];
}

public class NotificationUser
{
    public required string UserId { get; set; }
    public bool IsRead { get; set; }
}

public class NotificationStation
{
    public required string StationId { get; set; }
    public bool IsRead { get; set; }
}
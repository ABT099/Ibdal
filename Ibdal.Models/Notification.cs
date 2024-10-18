using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Notification : BaseModel<int>
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public IList<NotificationUser> Users { get; set; } = [];
    public IList<NotificationStation> Stations { get; set; } = [];
}

public class NotificationUser
{
    public required User User { get; set; }
    public bool IsRead { get; set; }
}

public class NotificationStation
{
    public required Station Station { get; set; }
    public bool IsRead { get; set; }
}
namespace Ibdal.Api.Forms;

public class UpdateNotificationForm : NotificationToAllForm
{
    public required string Id { get; set; }
    public IEnumerable<string> UsersIds { get; set; } = [];
    public IEnumerable<string> StationsIds { get; set; } = [];
}
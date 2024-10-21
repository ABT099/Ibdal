namespace Ibdal.Api.Forms;

public class CreateNotificationForm : NotificationToAllForm
{
    public IEnumerable<string> UsersIds { get; set; } = [];
    public IEnumerable<string> StationsIds { get; set; } = [];
}
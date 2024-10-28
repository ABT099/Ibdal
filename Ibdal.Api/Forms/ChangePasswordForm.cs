namespace Ibdal.Api.Forms;

public class ChangePasswordForm
{
    public required string AuthId { get; set; }
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}
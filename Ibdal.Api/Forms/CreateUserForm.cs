namespace Ibdal.Api.Forms;

public class CreateUserForm : LoginForm
{
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
}
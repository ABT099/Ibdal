namespace Ibdal.Api.Forms;

public class CreateStationForm : CreateUserForm
{
    public required string City { get; set; }
    public required string Address { get; set; }
}
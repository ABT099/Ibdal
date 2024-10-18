namespace Ibdal.Api.Forms;

public class UpdateCarForm
{
    public int Id { get; set; }
    public required string DriverName { get; set; }
    public required string DriverPhoneNumber { get; set; }
    public required string PlateNumber { get; set; }
    public required string ChaseNumber { get; set; }
    public required string CarType { get; set; }
    public required string CarModel { get; set; }
}
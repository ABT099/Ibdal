namespace Ibdal.Api.Forms;

public class CreateCarForm
{
    public int OwnerId { get; set; }
    public required string PlateNumber { get; set; }
    public required string ChaseNumber { get; set; }
    public required string CarType { get; set; }
    public required string CarModel { get; set; }
}
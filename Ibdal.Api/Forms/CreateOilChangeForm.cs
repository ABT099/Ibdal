namespace Ibdal.Api.Forms;

public class CreateOilChangeForm
{
    public required string CarId { get; set; }
    public required string StationId { get; set; }
    public required string ProductId { get; set; }
    public int Quantity { get; set; }
    public int CurrentCarMeter { get; set; }
    public int NextCarMeter { get; set; }
}
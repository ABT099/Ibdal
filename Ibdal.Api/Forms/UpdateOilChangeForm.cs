namespace Ibdal.Api.Forms;

public class UpdateOilChangeForm
{
    public required string Id { get; set; }
    public string? ProductId { get; set; }
    public int Quantity { get; set; }
    public int CurrentCarMeter { get; set; }
    public int NextCarMeter { get; set; }
}
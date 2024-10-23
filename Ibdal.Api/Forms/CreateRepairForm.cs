namespace Ibdal.Api.Forms;

public class CreateRepairForm
{
    public required string CarId { get; set; }
    public required string StationId { get; set; }
    public required string CategoryId { get; set; }
    public string? Comment { get; set; }
    public IList<string> RepairItems { get; set; } = [];
}
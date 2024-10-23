namespace Ibdal.Api.Forms;

public class UpdateRepairForm 
{
    public required string Id { get; set; }
    public required string CarId { get; set; }
    public required string StationId { get; set; }
    public required string CategoryId { get; set; }
    public string? Comment { get; set; }
}
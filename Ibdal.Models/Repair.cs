using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Repair : BaseModel
{
    public required string CarId { get; set; }
    public required string PlateNumber { get; set; }
    public required string CarType { get; set; }
    public required string CarModel { get; set; }
    
    
    public required string StationId { get; set; }
    public required string StationName { get; set; }
    
    
    public required string CategoryId { get; set; }
    public required string CategoryName { get; set; }
    
    public IList<string> RepairItems { get; set; } = [];
    public string? Comment { get; set; }
    public string Status { get; set; } = "pending";
}
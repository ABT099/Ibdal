using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Car : BaseModel<int>
{
    public required User Owner { get; set; }
    public required string PlateNumber { get; set; }
    public required string ChaseNumber { get; set; }
    public required string CarType { get; set; }
    public required string CarModelName { get; set; }
    public int CarMeter { get; set; } = 0;
    public IList<Repair> RepairHistory { get; set; } = [];
    public IList<OilChange> OilChanges { get; set; } = [];
}
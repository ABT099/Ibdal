using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Repair : BaseModel
{
    public required Car Car { get; set; }
    public required Station Station { get; set; }
    public required Category Category { get; set; }
    public List<string> RepairItems { get; set; } = [];
    public string Comment { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
}
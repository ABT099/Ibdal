using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class OilChange : BaseModel
{
    public required Product Oil { get; set; }
    public int Quantity { get; set; }
    public int CurrentCarMeter { get; set; }
    public int NextCarMeter { get; set; }
}
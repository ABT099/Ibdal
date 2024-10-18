using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Payment : BaseModel
{
    public required Purchase Purchase { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
}
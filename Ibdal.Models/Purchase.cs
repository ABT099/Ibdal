using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Purchase : BaseModel
{
    public required Order Order { get; set; }
    public int QuantityRemaining { get; set; }
    public IList<Payment> Payments { get; set; } = [];
}
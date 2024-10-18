using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Purchase : BaseModel<int>
{
    public required Station Station { get; set; }
    public required Product Product { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public int QuantityWhenBought { get; set; }
    public int QuantityRemaining { get; set; }
    public IList<Payment> Payments { get; set; } = [];
}
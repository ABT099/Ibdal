using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Payment : BaseModel
{
    public required string PurchaseId { get; set; }
    public int Amount { get; set; }
}
namespace Ibdal.Api.Forms;

public class CreateOrderForm
{
    public required string StationId { get; set; }
    public IEnumerable<ProductInfoForm> Products { get; set; } = [];
}
namespace Ibdal.Api.Forms;

public class UpdateProductForm
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public IFormFile? Image { get; set; }
}
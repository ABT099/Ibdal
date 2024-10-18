namespace Ibdal.Api.Forms;

public class CreateCategoryItemForm
{
    public required string CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required IFormFile Image { get; set; }
}
namespace Ibdal.Api.Forms;

public class UpdateCategoryItemForm
{
    public required string Id { get; set; }
    public required string CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IFormFile? Image { get; set; }
}
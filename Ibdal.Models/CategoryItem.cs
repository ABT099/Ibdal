using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class CategoryItem : BaseModel<int>
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ImageUrl { get; set; }
}
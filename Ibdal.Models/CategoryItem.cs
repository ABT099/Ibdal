using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class CategoryItem : BaseModel
{
    public required string CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
}
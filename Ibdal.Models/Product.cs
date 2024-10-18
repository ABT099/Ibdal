using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Product : BaseModel
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
}
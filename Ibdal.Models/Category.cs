using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Category : BaseModel
{
    public required string Name { get; set; }
    public List<CategoryItem> CategoryItems { get; set; } = [];
}
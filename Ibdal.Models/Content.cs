using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Content : BaseModel<int>
{
    public required string Text { get; set; }
    public required string Type { get; set; }
}
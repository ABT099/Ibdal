using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Content : BaseModel
{
    public required string Text { get; set; }
    public required string Type { get; set; }
}
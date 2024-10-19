using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Chat : BaseModel
{
    public required string UserId { get; set; }
    public IList<Message> Messages { get; set; } = [];
}
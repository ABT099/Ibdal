using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Message : BaseModel<int>
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public required string Text { get; set; }
}
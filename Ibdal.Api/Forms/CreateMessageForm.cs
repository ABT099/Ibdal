namespace Ibdal.Api.Forms;

public class CreateMessageForm
{
    public required string ChatId { get; set; }
    public required string SenderId { get; set; }
    public required string Text { get; set; }
}
namespace Ibdal.Api.ViewModels;

public static class MessageViewModels
{
    public static readonly Func<Message, object> CreateFlat = FlatProject.Compile();

    private static Expression<Func<Message, object>> FlatProject =>
        message => new
        {
            message.Id,
            message.SenderId,
            message.Text,
            message.CreatedAt
        };
}
namespace Ibdal.Api.ViewModels;

public static class ChatViewModels
{
    public static Expression<Func<Chat, object>> FlatProjection =>
        chat => new
        {
            chat.Id,
            chat.Name
        };
    
    public static Expression<Func<Chat, object>> Projection =>
        chat => new
        {
            chat.Id,
            Messages = MessageViewModels.CreateFlat
        };
}
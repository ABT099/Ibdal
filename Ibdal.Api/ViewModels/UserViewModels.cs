namespace Ibdal.Api.ViewModels;

public static class UserViewModels
{
    public static Expression<Func<User, object>> FlatProjection =>
        user => new
        {
            user.Id,
            user.Name,
            user.Points
        };
    
    public static Expression<Func<User, object>> Projection =>
        user => new
        {
            user.Id,
            user.Name,
            user.PhoneNumber,
            user.Points,
            user.Cars,
            ChatId = user.Chat.Id
        };
}
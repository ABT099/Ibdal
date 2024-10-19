namespace Ibdal.Api.ViewModels;

public static class ContentViewModels
{
    public static Expression<Func<Content, object>> FlatProjection =>
        content => new
        {
            content.Text,
            content.Type
        };
    
    public static Expression<Func<Content, object>> Projection =>
        content => new
        {
            content.Id,
            content.Text,
            content.Type
        };
}
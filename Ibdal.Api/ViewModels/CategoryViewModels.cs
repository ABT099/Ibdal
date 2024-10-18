namespace Ibdal.Api.ViewModels;

public static class CategoryViewModels
{
    public static Expression<Func<Category, object>> FlatProjection =>
        category => new
        {
            category.Id,
            category.Name
        };

    public static Expression<Func<Category, object>> Projection =>
        category => new
        {
            category.Id,
            category.Name,
            CategoryItems = category.CategoryItems
                .Select(CategoryItemViewModels.CreateFlat)
        };
}
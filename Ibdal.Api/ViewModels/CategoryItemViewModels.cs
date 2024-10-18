namespace Ibdal.Api.ViewModels;

public static class CategoryItemViewModels
{
    public static readonly Func<CategoryItem, object> CreateFlat = FlatProjection.Compile();
    public static Expression<Func<CategoryItem, object>> FlatProjection =>
        categoryItem => new
        {
            categoryItem.Id,
            categoryItem.Name,
            categoryItem.Description,
            categoryItem.ImageUrl
        };

    public static Expression<Func<CategoryItem, object>> Projection =>
        categoryItem => new
        {
            categoryItem.Id,
            categoryItem.CategoryId,
            categoryItem.Name,
            categoryItem.Description,
            categoryItem.ImageUrl
        };
}
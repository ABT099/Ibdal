namespace Ibdal.Api.ViewModels;

public static class ProductViewModels
{
    public static readonly Func<Product, object> CreateFlat = FlatProjection.Compile();
    
    public static Expression<Func<Product, object>> FlatProjection =>
        product => new
        {
            product.Id,
            product.Name,
            product.Category,
            product.Price,
            product.ImageUrl
        };

    public static Expression<Func<Product, object>> Projection =>
        product => new
        {
            product.Id,
            product.Name,
            product.Description,
            product.Category,
            product.Price,
            product.ImageUrl,
        };
}
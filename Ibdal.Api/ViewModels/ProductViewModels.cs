namespace Ibdal.Api.ViewModels;

public static class ProductViewModels
{
    public static readonly Func<Product, object> CreateFlat = FlatProjection.Compile();
    
    public static Expression<Func<Product, object>> FlatProjection =>
        product => new
        {
            product.Id,
            product.Name,
            product.Price,
            product.ImageUrl
        };
}
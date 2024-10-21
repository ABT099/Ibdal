namespace Ibdal.Api.ViewModels;

public static class OrderViewModels
{
    public static Expression<Func<Order, object>> FlatProjection =>
        order => new
        {
            order.Id,
            order.OrderNumber,
            order.Status,
            TotalQuantity = order.ProductsInfo.Select(x => x.Quantity).Sum(),
            TotalPrice = order.ProductsInfo.Select(x => x.Product.Price * x.Quantity).Sum(),
            Date = order.CreatedAt
        };
    
    public static Expression<Func<Order, object>> Projection =>
        order => new
        {
            order.Id,
            order.OrderNumber,
            order.Status,
            TotalQuantity = order.ProductsInfo.Select(x => x.Quantity).Sum(),
            TotalPrice = order.ProductsInfo.Select(x => x.Product.Price * x.Quantity).Sum(),
            ProductDetails = order.ProductsInfo.Select(x => new
            {
                Product = ProductViewModels.CreateFlat(x.Product),
                x.Quantity
            }),
            Date = order.CreatedAt
        };
    
    public static Expression<Func<Order, object>> AdminProjection =>
        order => new
        {
            order.Id,
            order.OrderNumber,
            order.StationId,
            order.StationName,
            order.Status,
            TotalQuantity = order.ProductsInfo.Select(x => x.Quantity).Sum(),
            TotalPrice = order.ProductsInfo.Select(x => x.Product.Price * x.Quantity).Sum(),
            ProductDetails = order.ProductsInfo.Select(x => new
            {
                Product = ProductViewModels.CreateFlat(x.Product),
                x.Quantity
            }),
            Date = order.CreatedAt
        };
}
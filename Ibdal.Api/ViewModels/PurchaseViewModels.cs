namespace Ibdal.Api.ViewModels;

public static class PurchaseViewModels
{
    public static Expression<Func<Purchase, object>> FlatProjection =>
        purchase => new
        {
            purchase.Id,
            purchase.Order.StationName,
            purchase.Order.OrderNumber,
            purchase.Order.Status,
            purchase.QuantityRemaining
        };
    
    public static Expression<Func<Purchase, object>> Projection =>
        purchase => new
        {
            purchase.Id,
            purchase.QuantityRemaining,
            Order = OrderViewModels.CreateProjection(purchase.Order),
            purchase.Payments
        };
}
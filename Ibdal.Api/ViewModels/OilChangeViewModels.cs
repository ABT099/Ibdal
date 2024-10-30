namespace Ibdal.Api.ViewModels;

public static class OilChangeViewModels
{
    public static Expression<Func<OilChange, object>> FlatProjection =>
        oilChange => new
        {
            oilChange.Id,
            oilChange.Name,
            oilChange.Quantity,
            oilChange.ImageUrl
        };

    public static Expression<Func<OilChange, object>> Projection =>
        oilChange => new
        {
            oilChange.Id,
            oilChange.ProductId,
            oilChange.Name,
            oilChange.Quantity,
            oilChange.ImageUrl,
            oilChange.CurrentCarMeter,
            oilChange.NextCarMeter,
            Date = oilChange.CreatedAt,
        };
}
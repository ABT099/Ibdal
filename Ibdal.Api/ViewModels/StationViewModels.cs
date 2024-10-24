﻿namespace Ibdal.Api.ViewModels;

public static class StationViewModels
{
    public static Expression<Func<Station, object>> FlatProjection =>
        station => new
        {
            station.Id,
            station.Name,
            station.City
        };

    public static Expression<Func<Station, object>> Projection =>
        station => new
        {
            station.Id,
            station.Name,
            station.Address,
            station.City,
            station.Repairs,
            station.Purchases
        };
}
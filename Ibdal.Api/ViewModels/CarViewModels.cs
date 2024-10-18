using System.Linq.Expressions;
using Ibdal.Models;

namespace Ibdal.Api.ViewModels;

public static class CarViewModels
{
    public static readonly Func<Car, object> CreateFlat = FlatProjection.Compile();
    public static Expression<Func<Car, object>> FlatProjection =>
        car => new
        {
            car.Id,
            car.PlateNumber,
            car.CarModel,
            car.CarType,
            car.CarMeter
        };
    
    public static readonly Func<Car, object> Create = Projection.Compile();
    public static Expression<Func<Car, object>> Projection =>
        car => new
        {
            car.Id,
            car.OwnerId,
            car.PlateNumber,
            car.CarModel,
            car.CarType,
            car.CarMeter,
            car.OilChanges,
            car.RepairHistory
        };
}
namespace Ibdal.Api.ViewModels;

public static class RepairViewModels
{
    public static Func<Repair, object> CreateFlatProjection = FlatProjection.Compile();
    
    public static Expression<Func<Repair, object>> FlatProjection =>
        repair => new
        {
            repair.Id,
            repair.PlateNumber,
            repair.CarType,
            repair.CarModel,
            repair.CategoryName,
            repair.Status
        };
    
    public static Expression<Func<Repair, object>> Projection =>
        repair => new
        {
            repair.Id,
            repair.PlateNumber,
            repair.CarType,
            repair.CarModel,
            repair.StationName,
            repair.CategoryName,
            repair.RepairItems,
            repair.Comment,
            repair.Status
        };
}
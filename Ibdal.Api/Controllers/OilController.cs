using Ibdal.Api.Data;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OilController(AppDbContext ctx) : ControllerBase
{
    [HttpGet("car/{carId}")]
    public async Task<IActionResult> GetAllByCar(string carId)
    {
        var oilChanges = await ctx.OilChanges
            .Find(x => x.CarId == carId)
            .Project(OilChangeViewModels.FlatProjection)
            .ToListAsync();

        return Ok(oilChanges);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var oilChange = await ctx.OilChanges
            .Find(x => x.Id == id)
            .Project(OilChangeViewModels.Projection)
            .FirstOrDefaultAsync();

        if (oilChange == null)
            return NotFound();

        return Ok(oilChange);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOilChangeForm createOilChangeForm)
    {
        var oilTask = ctx.Products
            .Find(x => x.Id == createOilChangeForm.ProductId)
            .Project(x => new
            {
                x.Name,
                x.ImageUrl
            })
            .FirstOrDefaultAsync();

        var carTask = ctx.Cars
            .FindNonArchived(x => x.Id == createOilChangeForm.CarId)
            .Project(x => x.Id)
            .FirstOrDefaultAsync();

        var stationTask = ctx.Stations
            .FindNonArchived(x => x.Id == createOilChangeForm.StationId)
            .Project(x => x.Id)
            .FirstOrDefaultAsync();

        await Task.WhenAll(oilTask, carTask, stationTask);

        var oilInfo = oilTask.Result;

        if (oilInfo == null || carTask.Result == null || stationTask.Result == null) return NotFound();

        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var oilChange = new OilChange
            {
                CarId = createOilChangeForm.CarId,
                ProductId = createOilChangeForm.ProductId,
                Name = oilInfo.Name,
                ImageUrl = oilInfo.ImageUrl,
                Quantity = createOilChangeForm.Quantity,
                CurrentCarMeter = createOilChangeForm.CurrentCarMeter,
                NextCarMeter = createOilChangeForm.NextCarMeter,
                StationId = createOilChangeForm.StationId
            };

            await ctx.OilChanges.InsertOneAsync(session, oilChange);

            var carUpdateTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == createOilChangeForm.CarId,
                Builders<Car>.Update.Push(x => x.OilChanges, oilChange));

            var stationUpdateTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == createOilChangeForm.StationId,
                Builders<Station>.Update.Push(x => x.OilChanges, oilChange));

            await Task.WhenAll(carUpdateTask, stationUpdateTask);

            if (carUpdateTask.Result == null || stationUpdateTask.Result == null)
            {
                await session.AbortTransactionAsync();
                return BadRequest();
            }

            await session.CommitTransactionAsync();
            return Created();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem();
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateOilChangeForm updateOilChangeForm)
    {
        var oilChange = await ctx.OilChanges
            .Find(x => x.Id == updateOilChangeForm.Id)
            .FirstOrDefaultAsync();

        if (oilChange == null)
            return NotFound("Oil change not found");
        
        var carTask = ctx.Cars
            .FindNonArchived(x => x.Id == oilChange.CarId)
            .Project(x => x.OilChanges)
            .FirstOrDefaultAsync();
        
        var stationTask = ctx.Stations
            .FindNonArchived(x => x.Id == oilChange.StationId)
            .Project(x => x.OilChanges)
            .FirstOrDefaultAsync();
        
        await Task.WhenAll(carTask, stationTask);

        var carOil = carTask.Result;
        var stationOil = stationTask.Result;
        
        if (carOil == null)
        {
            return NotFound("car not found");
        }

        if (stationOil == null)
        {
            return NotFound("station not found");
        }
        
        var carOilIndex = carOil.IndexOf(oilChange);
        var stationOilIndex = stationOil.IndexOf(oilChange);
        
        if (oilChange.ProductId != updateOilChangeForm.ProductId &&
            !string.IsNullOrWhiteSpace(updateOilChangeForm.ProductId))
        {
            var product = await ctx.Products
                .FindNonArchived(x => x.Id == updateOilChangeForm.ProductId)
                .Project(x => new
                {
                    x.Name,
                    x.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound("new product not found");

            oilChange.ProductId = updateOilChangeForm.ProductId;
            oilChange.Name = product.Name;
            oilChange.ImageUrl = product.ImageUrl;
        }

        oilChange.Quantity = updateOilChangeForm.Quantity;
        oilChange.CurrentCarMeter = updateOilChangeForm.CurrentCarMeter;
        oilChange.NextCarMeter = updateOilChangeForm.NextCarMeter;

        carOil[carOilIndex] = oilChange;
        stationOil[stationOilIndex] = oilChange;
        
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var oilTask = ctx.OilChanges.ReplaceOneAsync(session, x => x.Id == updateOilChangeForm.Id, oilChange);
            
            var carUpdateTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == oilChange.CarId,
                Builders<Car>.Update.Set(x => x.OilChanges, carOil));
            
            var stationUpdateTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == oilChange.StationId,
                Builders<Station>.Update.Set(x => x.OilChanges, stationOil));
            
            await Task.WhenAll(oilTask, carUpdateTask, stationUpdateTask);

            if (carUpdateTask.Result == null || stationUpdateTask.Result == null || oilTask.Result == null)
            {
                await session.AbortTransactionAsync();
                return Problem();
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var oilChange = await ctx.OilChanges
            .Find(x => x.Id == id)
            .Project(x => new
            {
                x.CarId,
                x.StationId
            })
            .FirstOrDefaultAsync();

        if (oilChange == null)
        {
            return NotFound("oil change not found");
        }
        
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var oilTask = ctx.OilChanges.DeleteOneAsync(session, x => x.Id == id);
            
            var carUpdateTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == oilChange.CarId,
                Builders<Car>.Update.PullFilter(x => x.OilChanges, y => y.Id == id));
            
            var stationUpdateTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == oilChange.StationId,
                Builders<Station>.Update.PullFilter(x => x.OilChanges, y => y.Id == id));
            
            
            await Task.WhenAll(carUpdateTask, stationUpdateTask);

            if (oilTask.Result == null || stationUpdateTask.Result == null || carUpdateTask.Result == null)
            {
                await session.AbortTransactionAsync();
                return Problem();
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem();
        }
    }
}
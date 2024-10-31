using Ibdal.Api.Data;
using MongoDB.Bson;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RepairsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet("/station/{stationId}")]
    public async Task<IActionResult> GetByStationId(string stationId)
    {
        var repairs = await ctx.Repairs
            .Find(repair => repair.StationId == stationId)
            .Project(RepairViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(repairs);
    }
    
    [HttpGet("/car/{carId}")]
    public async Task<IActionResult> GetByCarId(string carId)
    {
        var repairs = await ctx.Repairs
            .Find(repair => repair.CarId == carId)
            .Project(RepairViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(repairs);
    }
    
    [HttpGet("/category/{categoryId}")]
    public async Task<IActionResult> GetByCategoryId(string categoryId)
    {
        var repairs = await ctx.Repairs
            .Find(repair => repair.CategoryId == categoryId)
            .Project(RepairViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(repairs);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var repair = await ctx.Repairs
            .Find(x => x.Id == id)
            .Project(RepairViewModels.Projection)
            .FirstOrDefaultAsync();
        
        if (repair is null)
        {
            return NotFound();
        }
        
        return Ok(repair);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRepairForm createRepairForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var carTask = ctx.Cars
                .Find(session, x => x.Id == createRepairForm.CarId)
                .FirstOrDefaultAsync();

            var categoryTask = ctx.Categories
                .Find(session, x => x.Id == createRepairForm.CategoryId)
                .Project(x => x.Name)
                .FirstOrDefaultAsync();

            var stationTask = ctx.Stations
                .Find(session, x => x.Id == createRepairForm.StationId)
                .Project(x => x.Name)
                .FirstOrDefaultAsync();

            await Task.WhenAll(carTask, categoryTask, stationTask);

            var car = await carTask;
            var categoryName = await categoryTask;
            var stationName = await stationTask;

            if (car == null || categoryName == null || stationName == null)
            {
                await session.AbortTransactionAsync();
                return BadRequest();
            }

            var repair = new Repair
            {
                CarId = createRepairForm.CarId,
                StationId = createRepairForm.StationId,
                CategoryId = createRepairForm.CategoryId,
                RepairItems = createRepairForm.RepairItems,
                Comment = createRepairForm.Comment,
                PlateNumber = car.PlateNumber,
                CarType = car.CarType,
                CarModel = car.CarModel,
                StationName = stationName,
                CategoryName = categoryName,
            };

            await ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == createRepairForm.CarId,
                Builders<Car>.Update.Push(x => x.RepairHistory, repair));

            await ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == createRepairForm.StationId,
                Builders<Station>.Update.Push(x => x.Repairs, repair));

            await ctx.Repairs.InsertOneAsync(session, repair);

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
    public async Task<IActionResult> Update([FromBody] UpdateRepairForm updateRepairForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var categoryTask = ctx.Categories
                .Find(session, x => x.Id == updateRepairForm.CategoryId)
                .Project(x => x.Name)
                .FirstOrDefaultAsync();

            var stationTask = ctx.Stations
                .Find(session, x => x.Id == updateRepairForm.StationId)
                .Project(x => new
                {
                    x.Repairs,
                    x.Cars
                })
                .FirstOrDefaultAsync();

            await Task.WhenAll(categoryTask, stationTask);

            var categoryName = categoryTask.Result;
            var station = stationTask.Result;

            if (categoryName == null || station == null)
            {
                await session.AbortTransactionAsync();
                return BadRequest("Invalid category, station, or car.");
            }
            
            var repair = station.Repairs.FirstOrDefault(x => x.Id == updateRepairForm.Id);

            if (repair == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found.");
            }
            
            var car = station.Cars.FirstOrDefault(x => x.Id == updateRepairForm.CarId);
            
            if (car == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Car not found.");
            }

            var updateDefinition = Builders<Repair>.Update.Combine(
                Builders<Repair>.Update.Set(x => x.Comment, updateRepairForm.Comment),
                Builders<Repair>.Update.Set(x => x.CategoryId, updateRepairForm.CategoryId),
                Builders<Repair>.Update.Set(x => x.CategoryName, categoryName)
            );

            await ctx.Repairs.UpdateOneAsync(
                session,
                x => x.Id == updateRepairForm.Id,
                updateDefinition
            );

            if (updateRepairForm.Comment != null) repair.Comment = updateRepairForm.Comment;

            repair.CategoryId = updateRepairForm.CategoryId;
            repair.CategoryName = categoryName;
            
            var repairCarIndex = car.RepairHistory.IndexOf(repair);
            if (repairCarIndex == -1)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found in car's history.");
            }

            car.RepairHistory[repairCarIndex] = repair;

            await ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == updateRepairForm.CarId,
                Builders<Car>.Update.Set(x => x.RepairHistory, car.RepairHistory)
            );

            var repairStationIndex = station.Repairs.IndexOf(repair);
            var carIndex = station.Cars.IndexOf(car);
            
            station.Repairs[repairStationIndex] = repair;
            station.Cars[carIndex] = car;

            var stationUpdateDefinition = Builders<Station>.Update.Combine(
                Builders<Station>.Update.Set(x => x.Repairs, station.Repairs),
                Builders<Station>.Update.Set(x => x.Cars, station.Cars));

            await ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == updateRepairForm.StationId,
                stationUpdateDefinition);

            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occurred while updating the repair.");
        }
    }
    
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] string id, [FromBody] string status)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var repairInfo = await ctx.Repairs
            .Find(session, x => x.Id == id)
            .Project(x => new { x.StationId, x.CarId })
            .FirstOrDefaultAsync();

            if (repairInfo == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found.");
            }

            var stationCars = await ctx.Stations
                .Find(session, x => x.Id == repairInfo.StationId)
                .Project(x => x.Cars)
                .FirstOrDefaultAsync();

            if (stationCars == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Station not found.");
            }

            var car = stationCars.FirstOrDefault(x => x.Id == repairInfo.CarId);
            if (car == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Car not found in station.");
            }

            var repair = car.RepairHistory.FirstOrDefault(x => x.Id == id);
            if (repair == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found in car's history.");
            }

            var statusUpdateTask = ctx.Repairs.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Repair>.Update.Set(x => x.Status, status)
            );

            var repairIndex = car.RepairHistory.IndexOf(repair);
            car.RepairHistory[repairIndex].Status = status;
            var carIndex = stationCars.IndexOf(car);
            stationCars[carIndex] = car;

            var stationUpdateDefinition = Builders<Station>.Update.Combine(
                Builders<Station>.Update.Set("Repairs.$[elem].Status", status),
                Builders<Station>.Update.Set(x => x.Cars, stationCars)
            );

            var arrayFilters = new List<ArrayFilterDefinition>
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("elem.Id", id))
            };

            var stationRepairUpdateTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == repairInfo.StationId,
                stationUpdateDefinition,
                new UpdateOptions { ArrayFilters = arrayFilters }
            );

            var carRepairUpdateTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == repairInfo.CarId,
                Builders<Car>.Update.Set("RepairHistory.$[elem].Status", status),
                new UpdateOptions { ArrayFilters = arrayFilters });

            await Task.WhenAll(statusUpdateTask, stationRepairUpdateTask, carRepairUpdateTask);

            if (!statusUpdateTask.Result.IsAcknowledged || !stationRepairUpdateTask.Result.IsAcknowledged || !carRepairUpdateTask.Result.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return Problem("Failed to update the repair status in one or more records.");
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while updating the repair.");
        }
    }
    
    [HttpPatch("{id}/add-repair-item")]
    public async Task<IActionResult> AddRepairItem([FromRoute] string id, [FromBody] string item)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var repair = await ctx.Repairs
                .Find(session, x => x.Id == id)
                .Project(x => new { x.RepairItems, x.CarId, x.StationId })
                .FirstOrDefaultAsync();

            if (repair == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found.");
            }

            var station = await ctx.Stations
                .Find(session, x => x.Id == repair.StationId)
                .Project(x => new { x.Cars, x.Repairs })
                .FirstOrDefaultAsync();

            if (station == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Station not found.");
            }
            
            var car = station.Cars.First(x => x.Id == repair.CarId);
            var newRepair = car.RepairHistory.First(x => x.Id == id);
            
            if (repair.RepairItems.Contains(item) || newRepair.RepairItems.Contains(item))
            {                
                await session.AbortTransactionAsync();
                return BadRequest("Item is already in the list.");
            }
            
            repair.RepairItems.Add(item);
            newRepair.RepairItems.Add(item);
            
            var carIndex = station.Cars.IndexOf(car);
            var carRepairIndex = car.RepairHistory.IndexOf(newRepair);
            var stationRepairIndex = station.Repairs.IndexOf(newRepair);
            
            car.RepairHistory[carRepairIndex] = newRepair;
            station.Repairs[stationRepairIndex] = newRepair;
            station.Cars[carIndex] = car;
            
            var updateRepairTask = ctx.Repairs.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Repair>.Update.Set(x => x.RepairItems, repair.RepairItems));

            var stationUpdateDefinition = Builders<Station>.Update.Combine(
                Builders<Station>.Update.Set(x => x.Cars, station.Cars),
                Builders<Station>.Update.Set(x => x.Repairs, station.Repairs));

            var updateStationTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == repair.StationId,
                stationUpdateDefinition);
            
            var updateCarTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == repair.CarId,
                Builders<Car>.Update.Set(x => x.RepairHistory, car.RepairHistory));
            
            await Task.WhenAll(updateStationTask, updateCarTask, updateRepairTask);
            
            await session.AbortTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while updating the repair.");
        }
    }
    
    [HttpPatch("{id}/remove-repair-item")]
    public async Task<IActionResult> RemoveRepairItem([FromRoute] string id, [FromBody] string item)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var repair = await ctx.Repairs
                .Find(session, x => x.Id == id)
                .Project(x => new { x.RepairItems, x.CarId, x.StationId })
                .FirstOrDefaultAsync();

            if (repair == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found.");
            }

            var station = await ctx.Stations
                .Find(session, x => x.Id == repair.StationId)
                .Project(x => new { x.Cars, x.Repairs })
                .FirstOrDefaultAsync();

            if (station == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Station not found.");
            }
            
            var car = station.Cars.First(x => x.Id == repair.CarId);
            var newRepair = car.RepairHistory.First(x => x.Id == id);

            if (!repair.RepairItems.Contains(item) || !newRepair.RepairItems.Contains(item))
            {                
                await session.AbortTransactionAsync();
                return BadRequest("Item does not in the list.");
            }
            
            repair.RepairItems.Remove(item);
            newRepair.RepairItems.Remove(item);
            
            var carIndex = station.Cars.IndexOf(car);
            var carRepairIndex = car.RepairHistory.IndexOf(newRepair);
            var stationRepairIndex = station.Repairs.IndexOf(newRepair);
            
            car.RepairHistory[carRepairIndex] = newRepair;
            station.Repairs[stationRepairIndex] = newRepair;
            station.Cars[carIndex] = car;
            
            var updateRepairTask = ctx.Repairs.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Repair>.Update.Set(x => x.RepairItems, repair.RepairItems));

            var stationUpdateDefinition = Builders<Station>.Update.Combine(
                Builders<Station>.Update.Set(x => x.Cars, station.Cars),
                Builders<Station>.Update.Set(x => x.Repairs, station.Repairs));

            var updateStationTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == repair.StationId,
                stationUpdateDefinition);
            
            var updateCarTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == repair.CarId,
                Builders<Car>.Update.Set(x => x.RepairHistory, car.RepairHistory));
            
            await Task.WhenAll(updateStationTask, updateCarTask, updateRepairTask);
            
            await session.AbortTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while updating the repair.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var info = await ctx.Repairs
                .Find(session, x => x.Id == id)
                .Project(x => new {x.CarId, x.StationId })
                .FirstOrDefaultAsync();

            if (info == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found.");
            }
            
            var deleteRepairResult = await ctx.Repairs.DeleteOneAsync(session, x => x.Id == id);

            if (!deleteRepairResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound("Repair not found.");
            }

            var deleteStationRepairTask = ctx.Stations.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Station>.Update.PullFilter(x => x.Repairs, y => y.Id == id));
            
            var deleteCarRepairTask = ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Car>.Update.PullFilter(x => x.RepairHistory, y => y.Id == id));

            await Task.WhenAll(deleteCarRepairTask, deleteStationRepairTask);
            
            var deleteStationRepairResult = deleteStationRepairTask.Result;
            var deleteCarRepairResult = deleteCarRepairTask.Result;

            if (!deleteStationRepairResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound("Station not found.");
            }

            if (!deleteCarRepairResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound("Car not found.");
            }
            
            await session.AbortTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occured while deleting the repair.");
        }
    }
}
using Ibdal.Api.Data;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
public class CarsController(AppDbContext ctx) : ApiController
{
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllByUser(string userId)
    {
        var cars = await ctx.Cars
            .FindNonArchived(x => x.OwnerId == userId)
            .Project(CarViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(cars);
    }
    
    [HttpGet("station/{stationId}")]
    public async Task<IActionResult> GetAllByStation(string stationId)
    {
        var cars = await ctx.Stations
            .FindNonArchived(x => x.Id == stationId)
            .Project(x => new
            {
                Cars = x.Cars.Select(y => CarViewModels.CreateFlat(y))
            })
            .ToListAsync();
        
        return Ok(cars);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        List<object>? cars = [];
        
        if (Role == Constants.Roles.Customer)
        {
            cars = await ctx.Cars
                .FindNonArchived(x => x.OwnerId == Id)
                .Project(CarViewModels.FlatProjection)
                .ToListAsync();
        } 
        else if (Role == Constants.Roles.Station)
        {
            cars = [await ctx.Stations
                .FindNonArchived(x => x.Id == Id)
                .Project(x => new
                {
                    Cars = x.Cars.Select(y => CarViewModels.CreateFlat(y))
                })
                .ToListAsync()];
        }
        
        return Ok(cars);
    } 

    [HttpGet("{id:}")]
    public async Task<ActionResult> GetById(string id)
    {
        var car = await ctx.Cars
            .FindNonArchived(x => x.Id == id)
            .Project(CarViewModels.Projection)
            .FirstOrDefaultAsync();
        
        if (car is null)
            return NotFound();
        
        return Ok(car);
    }

    [HttpGet("plate/{plateNumber}")]
    public async Task<ActionResult> GetByPlate(string plateNumber)
    {
        var car = await ctx.Cars
            .FindNonArchived(x => x.PlateNumber == plateNumber)
            .Project(CarViewModels.Projection)
            .FirstOrDefaultAsync();
        
        if (car is null)
            return NotFound();
        
        return Ok(car);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCarForm createCarForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var car = new Car
            {
                OwnerId = createCarForm.OwnerId,
                PlateNumber = createCarForm.PlateNumber,
                ChaseNumber = createCarForm.ChaseNumber,
                CarType = createCarForm.CarType,
                CarModel = createCarForm.CarModel
            };

            var ownerResult = await ctx.Users.UpdateOneAsync(
                session,
                x => x.Id == createCarForm.OwnerId,
                Builders<User>.Update.Push(u => u.Cars, car));

            if (!ownerResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound();
            }
            
            await ctx.Cars.InsertOneAsync(session, car);
            
            await session.CommitTransactionAsync();
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car.Id);
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            return Problem("Something went wrong");
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCarForm updateCarForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var car = await ctx.Cars
                .FindNonArchived(x => x.Id == updateCarForm.Id)
                .FirstOrDefaultAsync();
        
            if (car is null)
                return NotFound("car does not exist");
            
            var owner = await ctx.Users
                .FindNonArchived(x => x.Id == car.OwnerId)
                .FirstOrDefaultAsync();
            
            if (owner is null)
                return NotFound("owner does not exist");
            
            car.PlateNumber = updateCarForm.PlateNumber;
            car.ChaseNumber = updateCarForm.ChaseNumber;
            car.CarType = updateCarForm.CarType;
            car.CarModel = updateCarForm.CarModel;
            
            await ctx.Cars.ReplaceOneAsync(session, x => x.Id == car.Id, car);
            
            owner.PhoneNumber = updateCarForm.DriverPhoneNumber;
            owner.Name = updateCarForm.DriverName;
            
            var carIndex = owner.Cars.IndexOf(car);
            
            if (carIndex != -1)
            {
                owner.Cars[carIndex] = car;
            }
            else
            {
                owner.Cars.Add(car);
            }
            
            await ctx.Users.ReplaceOneAsync(session, x => x.Id == owner.Id, owner);
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            return Problem("Something went wrong");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var userId = await ctx.Cars
                .FindNonArchived(x => x.Id == id)
                .Project(x => x.OwnerId)
                .FirstOrDefaultAsync();

            var carRemoveResult = await ctx.Cars.UpdateOneAsync(
                session,
                x => x.Id == id,
                Builders<Car>.Update.Set(x => x.Archived, true));

            if (!carRemoveResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound();
            }
        
            var userUpdateResult = await ctx.Users.UpdateOneAsync(
                session,
                x => x.Id == userId,
                Builders<User>.Update.PullFilter(x => x.Cars, c => c.Id == id));
            
            if (!userUpdateResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return BadRequest("user does not exist");
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("Something went wrong");
        }
    }
}
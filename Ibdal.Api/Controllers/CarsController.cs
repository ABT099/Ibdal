using Ibdal.Models;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetAllByUser(int userId)
    {
        var cars = await ctx.Cars
            .Find(x => x.OwnerId == userId)
            .Project(CarViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(cars);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id)
    {
        var car = await ctx.Cars
            .Find(x => x.Id == id)
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
            .Find(x => x.PlateNumber == plateNumber)
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
            var owner = await ctx.Users
                .Find(x => x.Id == createCarForm.OwnerId)
                .FirstOrDefaultAsync();
            
            if (owner is null)
                return NotFound("owner does not exist");
            
            var car = new Car
            {
                OwnerId = owner.Id,
                PlateNumber = createCarForm.PlateNumber,
                ChaseNumber = createCarForm.ChaseNumber,
                CarType = createCarForm.CarType,
                CarModel = createCarForm.CarModel
            };
            
            await ctx.Cars.InsertOneAsync(car);
            
            owner.Cars.Add(car);
            
            await ctx.Users.ReplaceOneAsync(session, x => x.Id == owner.Id, owner);
            
            await session.CommitTransactionAsync();
            return CreatedAtAction(nameof(GetByPlate), new { plateNumber = car.PlateNumber }, car.Id);

        }
        catch (Exception)
        {
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
                .Find(x => x.Id == updateCarForm.Id)
                .FirstOrDefaultAsync();
        
            if (car is null)
                return NotFound("car does not exist");
            
            var owner = await ctx.Users
                .Find(x => x.Id == car.OwnerId)
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
            owner.Cars[carIndex] = car;
            
            await ctx.Users.ReplaceOneAsync(session, x => x.Id == owner.Id, owner);
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception)
        {
            return Problem("Something went wrong");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await ctx.Cars
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
        
        if (car is null)
            return NotFound();
        
        car.IsDeleted = true;
        
        await ctx.Cars.ReplaceOneAsync(x => x.Id == id, car);
        
        return Ok();
    }
}
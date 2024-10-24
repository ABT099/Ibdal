namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stations = await ctx.Stations
            .Find(_ => true)
            .Project(StationViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(stations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var station = await ctx.Stations
            .Find(x => x.Id == id)
            .Project(StationViewModels.Projection)
            .FirstOrDefaultAsync();

        if (station == null)
        {
            return NotFound();
        }
        
        return Ok(station);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStationForm createStationForm)
    {
        var station = new Station
        {
            AuthId = string.Empty,
            Name = createStationForm.Name,
            PhoneNumber = createStationForm.PhoneNumber,
            City = createStationForm.City,
            Address = createStationForm.Address
        };
        
        await ctx.Stations.InsertOneAsync(station);
        
        return CreatedAtAction(nameof(GetById), new { id = station.Id }, station);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStationForm updateStationForm)
    {
        var station = await ctx.Stations
            .Find(x => x.Id == updateStationForm.Id)
            .FirstOrDefaultAsync();

        if (station == null)
        {
            return NotFound();
        }
        
        station.Name = updateStationForm.Name;
        station.PhoneNumber = updateStationForm.PhoneNumber;
        station.City = updateStationForm.City;
        station.Address = updateStationForm.Address;
        
        await ctx.Stations.ReplaceOneAsync(x => x.Id == station.Id, station);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleteStationResult = await ctx.Stations.UpdateOneAsync(
            x => x.Id == id,
            Builders<Station>.Update.Set(x => x.IsDeleted, true));

        if (!deleteStationResult.IsAcknowledged)
        {
            return NotFound();
        }

        return Ok();
    }
}
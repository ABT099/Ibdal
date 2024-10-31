using Ibdal.Api.Data;
using Ibdal.Api.Services;
using Microsoft.AspNetCore.Identity;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
public class StationsController(AppDbContext ctx, AuthService authService) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stations = await ctx.Stations
            .FindNonArchived(_ => true)
            .Project(StationViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(stations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var station = await ctx.Stations
            .FindNonArchived(x => x.Id == id)
            .Project(StationViewModels.Projection)
            .FirstOrDefaultAsync();

        if (station == null)
        {
            return NotFound();
        }
        
        return Ok(station);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var station = await ctx.Stations
            .FindNonArchived(x => x.Id == Id)
            .Project(StationViewModels.FullProjection)
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
        var authId = await authService.CreateUser(createStationForm.Username, createStationForm.Password);
        
        if (authId == null) return BadRequest("user already exists");
        
        var station = new Station
        {
            AuthId = authId,
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
            .FindNonArchived(x => x.Id == updateStationForm.Id)
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
            Builders<Station>.Update.Set(x => x.Archived, true));

        if (!deleteStationResult.IsAcknowledged)
        {
            return NotFound();
        }

        return Ok();
    }
}
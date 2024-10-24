﻿namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await ctx.Users
            .Find(_ => true)
            .Project(UserViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await ctx.Users
            .Find(x => x.Id == id)
            .Project(UserViewModels.Projection)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserForm createUserForm)
    {
        var user = new User
        {
            AuthId = string.Empty,
            Name = createUserForm.Name,
            PhoneNumber = createUserForm.PhoneNumber,
        };

        await ctx.Users.InsertOneAsync(user);
        
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserForm updateUserForm)
    {
        var userUpdateDefinition = Builders<User>.Update.Combine(
            Builders<User>.Update.Set(x => x.Name, updateUserForm.Name),
            Builders<User>.Update.Set(x => x.PhoneNumber, updateUserForm.PhoneNumber));

        var updateResult = await ctx.Users.UpdateOneAsync(
            x => x.Id == updateUserForm.Id,
            userUpdateDefinition);

        if (!updateResult.IsAcknowledged)
        {
            return NotFound();
        }
        
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleteResult = await ctx.Users.UpdateOneAsync(
            x => x.Id == id,
            Builders<User>.Update.Set(x => x.IsDeleted, true));

        if (!deleteResult.IsAcknowledged)
        {
            return NotFound();
        }

        return Ok();
    }
}
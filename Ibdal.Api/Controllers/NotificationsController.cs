using Ibdal.Api.Data;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await ctx.Notifications
            .Find(_ => true)
            .Project(NotificationViewModels.FlatAdminProjection)
            .FirstOrDefaultAsync();
        
        return Ok(notifications);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var notifications = await ctx.Notifications
            .FindNonArchived(x => x.Users
                .Select(n => n.UserId)
                .Contains(userId))
            .Project(NotificationViewModels.FlatUserProjection)
            .ToListAsync();
        
        return Ok(notifications);
    }
    
    [HttpGet("stations/{stationId}")]
    public async Task<IActionResult> GetByStationId(string stationId)
    {
        var notifications = await ctx.Notifications
            .FindNonArchived(x => x.Stations
                .Select(n => n.StationId)
                .Contains(stationId))
            .Project(NotificationViewModels.FlatStationProjection)
            .ToListAsync();
        
        return Ok(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationForm createNotificationForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var notification = new Notification
            {
                Title = createNotificationForm.Title,
                Description = createNotificationForm.Description,
                Users = createNotificationForm.UsersIds.Select(u => new NotificationUser { UserId = u, IsRead = false }).ToList(),
                Stations = createNotificationForm.StationsIds.Select(s => new NotificationStation { StationId = s, IsRead = false }).ToList(),
            };
        
            await ctx.Notifications.InsertOneAsync(session, notification);
            
            var userFilter = Builders<User>.Filter.In(x => x.Id, createNotificationForm.UsersIds);
            var userUpdate = Builders<User>.Update.Push(x => x.Notifications, notification);
            var updateUserTask = ctx.Users.UpdateManyAsync(session, userFilter, userUpdate);

            var stationFilter = Builders<Station>.Filter.In(x => x.Id, createNotificationForm.StationsIds);
            var stationUpdate = Builders<Station>.Update.Push(x => x.Notifications, notification);
            var updateStationTask = ctx.Stations.UpdateManyAsync(session, stationFilter, stationUpdate);

            await Task.WhenAll(updateUserTask, updateStationTask);
            
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
    
    [HttpPost("/all")]
    public async Task<IActionResult> CreateForAll([FromBody] NotificationToAllForm notificationsForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var allUsers = await ctx.Users
                .FindNonArchived(session, _ => true)
                .Project(x => x.Id)
                .ToListAsync();

            var allStations = await ctx.Stations
                .FindNonArchived(session, _ => true)
                .Project(x => x.Id)
                .ToListAsync();

            var notification = new Notification
            {
                Title = notificationsForm.Title,
                Description = notificationsForm.Description,
                Users = allUsers.Select(u => new NotificationUser { UserId = u, IsRead = false }).ToList(),
                Stations = allStations.Select(s => new NotificationStation { StationId = s, IsRead = false }).ToList(),
            };

            await ctx.Notifications.InsertOneAsync(session, notification);
            
            var userUpdate = Builders<User>.Update.Push(x => x.Notifications, notification);
            var updateUserTask = ctx.Users.UpdateManyAsync(session, Builders<User>.Filter.Empty, userUpdate);

            var stationUpdate = Builders<Station>.Update.Push(x => x.Notifications, notification);
            var updateStationTask = ctx.Stations.UpdateManyAsync(session, Builders<Station>.Filter.Empty, stationUpdate);

            await Task.WhenAll(updateUserTask, updateStationTask);

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
    
    [HttpPost("/users")]
    public async Task<IActionResult> CreateForAllUsers([FromBody] NotificationToAllForm notificationsForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var allUsers = await ctx.Users
                .FindNonArchived(session, _ => true)
                .Project(x => x.Id)
                .ToListAsync();

            var notification = new Notification
            {
                Title = notificationsForm.Title,
                Description = notificationsForm.Description,
                Users = allUsers.Select(u => new NotificationUser { UserId = u, IsRead = false }).ToList(),
            };
            
            await ctx.Notifications.InsertOneAsync(session, notification);
            
            var update = Builders<User>.Update.Push(x => x.Notifications, notification);
            await ctx.Users.UpdateManyAsync(session, Builders<User>.Filter.Empty, update);

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
    
    [HttpPost("/users/read/{userId}")]
    public async Task<IActionResult> ReadForUser(string userId)
    {
        var filter = Builders<Notification>.Filter.ElemMatch(
            n => n.Users,
            u => u.UserId == userId && !u.IsRead);

        var update = Builders<Notification>.Update.Set(
            n => n.Users[-1].IsRead, true);

        var updateResult = await ctx.Notifications.UpdateManyAsync(filter, update);

        if (updateResult.ModifiedCount == 0)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost("/stations")]
    public async Task<IActionResult> CreateForAllStations([FromBody] NotificationToAllForm notificationsForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var allStations = await ctx.Stations
                .FindNonArchived(session, _ => true)
                .Project(x => x.Id)
                .ToListAsync();
        
            var notification = new Notification
            {
                Title = notificationsForm.Title,
                Description = notificationsForm.Description,
                Stations = allStations.Select(s => new NotificationStation { StationId = s, IsRead = false }).ToList(),
            };
        
            await ctx.Notifications.InsertOneAsync(session, notification);
            
            var update = Builders<Station>.Update.Push(x => x.Notifications, notification);
            await ctx.Stations.UpdateManyAsync(session, Builders<Station>.Filter.Empty, update);

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
    
    [HttpPost("/stations/read/{stationId}")]
    public async Task<IActionResult> ReadForStation(string stationId)
    {
        var filter = Builders<Notification>.Filter.ElemMatch(
            n => n.Stations,
            u => u.StationId == stationId && !u.IsRead);

        var update = Builders<Notification>.Update.Set(
            n => n.Stations[-1].IsRead, true);

        var updateResult = await ctx.Notifications.UpdateManyAsync(filter, update);

        if (updateResult.ModifiedCount == 0)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateNotificationForm updateNotificationForm)
    {
        var notification = await ctx.Notifications
            .FindNonArchived(x => x.Id == updateNotificationForm.Id)
            .FirstOrDefaultAsync();
        
        if (notification == null)
            return NotFound();

        var updateDefinition = Builders<Notification>.Update
            .Set(n => n.Title, updateNotificationForm.Title)
            .Set(n => n.Description, updateNotificationForm.Description);
        
        var currentUserIds = notification.Users.Select(u => u.UserId).ToHashSet();
        var currentStationIds = notification.Stations.Select(s => s.StationId).ToHashSet();

        var tasks = new List<Task>();
        
        if (!currentUserIds.SequenceEqual(updateNotificationForm.UsersIds))
        {
            var fetchUsersTask = ctx.Users
                .FindNonArchived(x => updateNotificationForm.UsersIds.Contains(x.Id))
                .Project(u => new NotificationUser
                {
                    UserId = u.Id,
                    IsRead = false
                }).ToListAsync();

            tasks.Add(fetchUsersTask.ContinueWith(t => 
                updateDefinition = updateDefinition.Set(n => n.Users, t.Result)));
        }

        if (!currentStationIds.SequenceEqual(updateNotificationForm.StationsIds))
        {
            var fetchStationsTask = ctx.Stations
                .FindNonArchived(x => updateNotificationForm.StationsIds.Contains(x.Id))
                .Project(s => new NotificationStation
                {
                    StationId = s.Id,
                    IsRead = false
                }).ToListAsync();

            tasks.Add(fetchStationsTask.ContinueWith(t => 
                updateDefinition = updateDefinition.Set(n => n.Stations, t.Result)));
        }
        
        await Task.WhenAll(tasks);
        
        var updateResult = await ctx.Notifications.UpdateOneAsync(
            x => x.Id == updateNotificationForm.Id, 
            updateDefinition
        );
        
        if (updateResult.ModifiedCount == 0)
            return Problem("Failed to update notification");

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await ctx.Notifications.DeleteOneAsync(x => x.Id == id);

        if (!result.IsAcknowledged || result.DeletedCount == 0)
        {
            return NotFound();
        }
        
        return Ok();
    }
}
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
            .Find(x => x.Users
                .Select(n => n.User.Id)
                .Contains(userId))
            .Project(NotificationViewModels.FlatUserProjection)
            .ToListAsync();
        
        return Ok(notifications);
    }
    
    [HttpGet("stations/{stationId}")]
    public async Task<IActionResult> GetByStationId(string stationId)
    {
        var notifications = await ctx.Notifications
            .Find(x => x.Stations
                .Select(n => n.Station.Id)
                .Contains(stationId))
            .Project(NotificationViewModels.FlatStationProjection)
            .ToListAsync();
        
        return Ok(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationForm createNotificationForm)
    {
        var users = await ctx.Users
            .Find(x => createNotificationForm.UsersIds.Contains(x.Id))
            .ToListAsync();
        
        var stations = await ctx.Stations
            .Find(x => createNotificationForm.StationsIds.Contains(x.Id))
            .ToListAsync();
        
        var notification = new Notification
        {
            Title = createNotificationForm.Title,
            Description = createNotificationForm.Description,
            Users = users.Select(u => new NotificationUser { User = u, IsRead = false }).ToList(),
            Stations = stations.Select(s => new NotificationStation { Station = s, IsRead = false }).ToList(),
        };
        
        await ctx.Notifications.InsertOneAsync(notification);

        return Ok();
    }
    
    [HttpPost("/all")]
    public async Task<IActionResult> CreateForAll([FromBody] NotificationToAllForm notificationsForm)
    {
        var allUsers = await ctx.Users
            .Find(_ => true)
            .ToListAsync();

        var allStations = await ctx.Stations
            .Find(_ => true)
            .ToListAsync();

        var notification = new Notification
        {
            Title = notificationsForm.Title,
            Description = notificationsForm.Description,
            Users = allUsers.Select(u => new NotificationUser { User = u, IsRead = false }).ToList(),
            Stations = allStations.Select(s => new NotificationStation { Station = s, IsRead = false }).ToList(),
        };

        await ctx.Notifications.InsertOneAsync(notification);

        return Ok();
    }
    
    
    [HttpPost("/users")]
    public async Task<IActionResult> CreateForAllUsers([FromBody] NotificationToAllForm notificationsForm)
    {
        var allUsers = await ctx.Users
            .Find(_ => true)
            .ToListAsync();

        var notification = new Notification
        {
            Title = notificationsForm.Title,
            Description = notificationsForm.Description,
            Users = allUsers.Select(u => new NotificationUser { User = u, IsRead = false }).ToList(),
        };
        
        await ctx.Notifications.InsertOneAsync(notification);

        return Ok();
    }
    
    [HttpPost("/users/read/{userId}")]
    public async Task<IActionResult> ReadForUser(string userId)
    {
        var filter = Builders<Notification>.Filter.ElemMatch(
            n => n.Users,
            u => u.User.Id == userId && !u.IsRead);

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
        var allStations = await ctx.Stations
            .Find(_ => true)
            .ToListAsync();
        
        var notification = new Notification
        {
            Title = notificationsForm.Title,
            Description = notificationsForm.Description,
            Stations = allStations.Select(s => new NotificationStation { Station = s, IsRead = false }).ToList(),
        };
        
        await ctx.Notifications.InsertOneAsync(notification);

        return Ok();
    }
    
    [HttpPost("/stations/read/{stationId}")]
    public async Task<IActionResult> ReadForStation(string stationId)
    {
        var filter = Builders<Notification>.Filter.ElemMatch(
            n => n.Stations,
            u => u.Station.Id == stationId && !u.IsRead);

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
            .Find(x => x.Id == updateNotificationForm.Id)
            .FirstOrDefaultAsync();
        
        if (notification == null)
            return NotFound();

        var updateDefinition = Builders<Notification>.Update
            .Set(n => n.Title, updateNotificationForm.Title)
            .Set(n => n.Description, updateNotificationForm.Description);
        
        var currentUserIds = notification.Users.Select(u => u.User.Id).ToList();
        var currentStationIds = notification.Stations.Select(s => s.Station.Id).ToList();

        var tasks = new List<Task>();
        
        if (!currentUserIds.SequenceEqual(updateNotificationForm.UsersIds))
        {
            var fetchUsersTask = ctx.Users
                .Find(x => updateNotificationForm.UsersIds.Contains(x.Id))
                .Project(u => new NotificationUser
                {
                    User = u,
                    IsRead = false
                }).ToListAsync();

            tasks.Add(fetchUsersTask.ContinueWith(t => 
                updateDefinition = updateDefinition.Set(n => n.Users, t.Result)));
        }

        if (!currentStationIds.SequenceEqual(updateNotificationForm.StationsIds))
        {
            var fetchStationsTask = ctx.Stations
                .Find(x => updateNotificationForm.StationsIds.Contains(x.Id))
                .Project(s => new NotificationStation
                {
                    Station = s,
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
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update notification");

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
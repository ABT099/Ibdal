using Ibdal.Api.Data;

namespace Ibdal.Api;

public class NotificationCreationContext(AppDbContext ctx)
{
    public async Task CreateForAllAsync([FromBody] NotificationToAllForm notificationsForm)
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
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task CreateForStationsAsync([FromBody] NotificationToAllForm notificationsForm)
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
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task CreateForUsersAsync([FromBody] NotificationToAllForm notificationsForm)
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
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e); 
            throw;
        }
    }
}
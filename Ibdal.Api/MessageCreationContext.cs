using Ibdal.Api.Data;

namespace Ibdal.Api;

public class MessageCreationContext(AppDbContext ctx)
{
    public async Task CreateAsync(CreateMessageForm createMessageForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var chat = await ctx.Chats
                .Find(session, x => x.Id == createMessageForm.ChatId)
                .FirstOrDefaultAsync();
            
            if (chat == null)
            {
                await session.AbortTransactionAsync();
                throw new ParentNotFoundException("chat not found");
            }
            
            var message = new Message
            {
                ChatId = createMessageForm.ChatId,
                SenderId = createMessageForm.SenderId,
                Text = createMessageForm.Text
            };
            
            var chatUpdateTask = ctx.Chats.UpdateOneAsync(
                session,
                x => x.Id == createMessageForm.ChatId,
                Builders<Chat>.Update.Push(x => x.Messages, message));
            
            var messageInsertTask = ctx.Messages.InsertOneAsync(session, message);

            await Task.WhenAll(chatUpdateTask, messageInsertTask);

            var userResult = await ctx.Users.UpdateOneAsync(
                session,
                x => x.Id == chat.Id,
                Builders<User>.Update.Set(x => x.Chat, chat));

            if (!userResult.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                throw new ParentNotFoundException("user not found");
            }
            
            await session.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            throw;
        }
    }
    
    public class ParentNotFoundException(string message) : Exception(message);
}
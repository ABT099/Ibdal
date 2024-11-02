using Ibdal.Api.Data;

namespace Ibdal.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatsController(AppDbContext ctx) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var chats = await ctx.Chats
            .Find(_ => true)
            .Project(ChatViewModels.FlatProjection)
            .ToListAsync();
        
        return Ok(chats);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var chat = await ctx.Chats
            .Find(x => x.Id == id)
            .Project(ChatViewModels.Projection)
            .FirstOrDefaultAsync();
        
        return Ok(chat);
    }

    [HttpPut("messages")]
    public async Task<IActionResult> UpdateMessage([FromBody] UpdateMessageForm updateMessageForm)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var message = await ctx.Messages
                .Find(x => x.Id == updateMessageForm.Id)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("message not found");
            }
            
            var chat = await ctx.Chats
                .Find(x => x.Id == message.ChatId)
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("chat not found");
            }
            
            var messageIndex = chat.Messages.IndexOf(message);

            if (messageIndex == -1)
            {
                await session.AbortTransactionAsync();
                return NotFound("message not found");
            }
            
            var messageUpdateTask = ctx.Messages.UpdateOneAsync(
                session,
                x => x.Id == updateMessageForm.Id,
                Builders<Message>.Update.Set(x => x.Text, updateMessageForm.Text));
            
            var chatUpdateTask = ctx.Chats.UpdateOneAsync(
                session,
                x => x.Id == message.ChatId,
                Builders<Chat>.Update.Set(x => x.Messages[messageIndex], message));
            
            var userUpdateTask = ctx.Users.UpdateOneAsync(
                session,
                x => x.Id == chat.Id,
                Builders<User>.Update.Set(x => x.Chat, chat));

            await Task.WhenAll(chatUpdateTask, messageUpdateTask, userUpdateTask);

            if (!userUpdateTask.Result.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound("user not found");
            }
            
            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("something went wrong");
        }
    }

    [HttpDelete("messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        using var session = await ctx.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var message = await ctx.Messages
                .Find(x => x.Id == messageId)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Message not found");
            }

            var chat = await ctx.Chats
                .Find(x => x.Id == message.ChatId)
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                await session.AbortTransactionAsync();
                return NotFound("Chat not found");
            }

            var messageIndex = chat.Messages.IndexOf(message);

            if (messageIndex == -1)
            {
                await session.AbortTransactionAsync();
                return NotFound("message not found");
            }

            var chatUpdateTask = ctx.Chats.UpdateOneAsync(
                session,
                x => x.Id == chat.Id,
                Builders<Chat>.Update.PullFilter(x => x.Messages, m => m.Id == messageId)
            );

            var messageDeleteTask = ctx.Messages.DeleteOneAsync(session, x => x.Id == messageId);

            var userUpdateTask = ctx.Users.UpdateOneAsync(
                session,
                x => x.Id == chat.UserId,
                Builders<User>.Update.Set(x => x.Chat, chat)
            );

            await Task.WhenAll(chatUpdateTask, messageDeleteTask, userUpdateTask);

            if (!userUpdateTask.Result.IsAcknowledged)
            {
                await session.AbortTransactionAsync();
                return NotFound("User not found");
            }

            await session.CommitTransactionAsync();
            return Ok();
        }
        catch (Exception e)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(e);
            return Problem("An error occurred while deleting the message.");
        }
    }
}
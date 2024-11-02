using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Ibdal.Api.Hubs;

public class ChatHub(MessageCreationContext mCtx) : Hub
{
    private static readonly ConcurrentDictionary<string, List<CreateMessageForm>> ChatMessages = new();
    
    public async Task SendMessage(CreateMessageForm messageForm)
    {
        await Clients.All.SendAsync("ReceiveMessage", messageForm);
        if (!ChatMessages.TryGetValue(messageForm.ChatId, out var value))
        {
            value = [];
            ChatMessages[messageForm.ChatId] = value;
        }

        value.Add(messageForm);
    }
    
    public async Task EndChat(string chatId)
    {
        if (ChatMessages.TryRemove(chatId, out var value))
        {
            foreach (var message in value)
            {
                try
                {
                    await mCtx.CreateAsync(message);
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
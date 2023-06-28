using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Threading.Tasks;

public class ChatHub : Hub
{

    private readonly IConnectionMultiplexer _redis;

    public ChatHub(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    public async Task SendMessageChat(string groupName, string message)
    {
        var db = _redis.GetDatabase();
        db.ListRightPush(groupName, message);
        var messages = db.ListRange(groupName).Select(x => x.ToString()).ToList();  //db.ListRange("messages").Select(x => x.ToString()).ToList();

        //await Clients.Caller.SendAsync("LoadMessages", messages);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", groupName, messages);
    }

    public async Task JoinGroupChat(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var db = _redis.GetDatabase();
        var messages = db.ListRange(groupName).Select(x => x.ToString()).ToList(); ;

        await Clients.Caller.SendAsync("LoadMessages", messages);
       // await Clients.Group(groupName).SendAsync("ReceiveMessage", messages);
    }

    public async Task LeaveGroupChat(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }


     public async Task DeleteGroupMessagesChat(string groupName)
    {
        var db = _redis.GetDatabase();
        var totalMessages = db.ListLength(groupName);

        // Eliminar todos los mensajes del grupo
        db.ListTrim(groupName, totalMessages, 0);
        var messages = db.ListRange(groupName).Select(x => x.ToString()).ToList();
        await Clients.Group(groupName).SendAsync("ReceiveMessage", messages);
    }
}
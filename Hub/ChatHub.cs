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
    public async Task SendMessage(string groupName, string message)
    {
        var db = _redis.GetDatabase();
        db.ListRightPush(groupName, message);
        var messages = db.ListRange(groupName).Select(x => x.ToString()).ToList();  //db.ListRange("messages").Select(x => x.ToString()).ToList();

        //await Clients.Caller.SendAsync("LoadMessages", messages);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", messages);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var db = _redis.GetDatabase();
        var messages = db.ListRange(groupName).Select(x => x.ToString()).ToList(); ;

        await Clients.Caller.SendAsync("LoadMessages", messages);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
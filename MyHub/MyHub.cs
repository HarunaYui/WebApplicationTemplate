using Microsoft.AspNetCore.SignalR;

namespace WebApplicationTemplate.MyHub;

public class MyHub : Hub
{
    public Task SendPublicMsg(string msg)
    {
        string connId = this.Context.ConnectionId;
        string msgToSend = $"{connId}{DateTime.Now}:{msg}";
        return this.Clients.All.SendAsync("SendPublicMsg", msgToSend);
    }
}


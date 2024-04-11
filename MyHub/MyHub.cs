using Microsoft.AspNetCore.SignalR;

namespace WebApplicationTemplate.MyHub;

/// <summary>
/// SignalR
/// </summary>
public class MyHub : Hub
{
    /// <summary>
    /// SignR发送信息
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public Task SendPublicMsg(string msg)
    {
        string connId = this.Context.ConnectionId;
        string msgToSend = $"{connId}{DateTime.Now}:{msg}";
        return this.Clients.All.SendAsync("SendPublicMsg", msgToSend);
    }
}


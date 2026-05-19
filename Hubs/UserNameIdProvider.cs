using Microsoft.AspNetCore.SignalR;

namespace lab1_4.Hubs;

public class UserNameIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.Identity?.Name;
}

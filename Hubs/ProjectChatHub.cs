using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace lab1_4.Hubs;

[Authorize]
public class ProjectChatHub : Hub
{
    private readonly IChatMessageRepository _messages;
    private readonly IDirectMessageRepository _directMessages;
    private readonly UserManager<AppUser> _userManager;

    public ProjectChatHub(
        IChatMessageRepository messages,
        IDirectMessageRepository directMessages,
        UserManager<AppUser> userManager)
    {
        _messages = messages;
        _directMessages = directMessages;
        _userManager = userManager;
    }

    public async Task JoinProject(int projectId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");

    public async Task LeaveProject(int projectId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project_{projectId}");

    public async Task SendMessage(int projectId, string content, string? fileUrl = null, string? fileName = null)
    {
        var user = await _userManager.GetUserAsync(Context.User!);
        if (user == null) return;

        var message = new ChatMessage
        {
            ProjectId = projectId,
            SenderUserName = user.UserName!,
            SenderDisplayName = user.DisplayName.Length > 0 ? user.DisplayName : user.UserName!,
            Content = content,
            FileUrl = fileUrl,
            FileName = fileName,
            SentAt = DateTime.UtcNow
        };

        await _messages.AddAsync(message);

        var payload = new
        {
            id = message.Id,
            sender = message.SenderDisplayName,
            senderUserName = message.SenderUserName,
            content = message.Content,
            fileUrl = message.FileUrl,
            fileName = message.FileName,
            sentAt = message.SentAt.ToString("HH:mm dd.MM.yyyy")
        };

        await Clients.Group($"project_{projectId}").SendAsync("ReceiveMessage", payload);
    }

    public async Task SendDirectMessage(string recipientUserName, string content, string? fileUrl = null, string? fileName = null)
    {
        var user = await _userManager.GetUserAsync(Context.User!);
        if (user == null) return;

        var message = new DirectMessage
        {
            SenderUserName = user.UserName!,
            SenderDisplayName = user.DisplayName.Length > 0 ? user.DisplayName : user.UserName!,
            RecipientUserName = recipientUserName,
            Content = content,
            FileUrl = fileUrl,
            FileName = fileName,
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _directMessages.AddAsync(message);

        var payload = new
        {
            id = message.Id,
            sender = message.SenderDisplayName,
            senderUserName = message.SenderUserName,
            content = message.Content,
            fileUrl = message.FileUrl,
            fileName = message.FileName,
            sentAt = message.SentAt.ToString("HH:mm dd.MM.yyyy")
        };

        await Clients.User(recipientUserName).SendAsync("ReceiveDirectMessage", payload);
        await Clients.Caller.SendAsync("ReceiveDirectMessage", payload);
    }
}

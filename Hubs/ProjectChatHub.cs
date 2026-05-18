using lab1_4.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Hubs;

[Authorize]
public class ProjectChatHub : Hub
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ProjectChatHub(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task JoinProject(int projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");
    }

    public async Task LeaveProject(int projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project_{projectId}");
    }

    public async Task SendMessage(int projectId, string content, string? recipientUserName = null, string? fileUrl = null, string? fileName = null)
    {
        var user = await _userManager.GetUserAsync(Context.User!);
        if (user == null) return;

        var message = new lab1_4.Models.ChatMessage
        {
            ProjectId = projectId,
            SenderUserName = user.UserName!,
            SenderDisplayName = user.DisplayName.Length > 0 ? user.DisplayName : user.UserName!,
            Content = content,
            FileUrl = fileUrl,
            FileName = fileName,
            RecipientUserName = string.IsNullOrEmpty(recipientUserName) ? null : recipientUserName,
            SentAt = DateTime.UtcNow
        };
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        var payload = new
        {
            id = message.Id,
            sender = message.SenderDisplayName,
            senderUserName = message.SenderUserName,
            content = message.Content,
            fileUrl = message.FileUrl,
            fileName = message.FileName,
            recipientUserName = message.RecipientUserName,
            isPrivate = message.IsPrivate,
            sentAt = message.SentAt.ToString("HH:mm dd.MM.yyyy")
        };

        if (message.IsPrivate)
        {
            await Clients.User(message.RecipientUserName!).SendAsync("ReceiveMessage", payload);
            await Clients.Caller.SendAsync("ReceiveMessage", payload);
        }
        else
        {
            await Clients.Group($"project_{projectId}").SendAsync("ReceiveMessage", payload);
        }
    }
}

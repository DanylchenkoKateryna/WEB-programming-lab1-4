using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Models.ViewModels;
using lab1_4.Repositories.Interfaces;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Services;

public class ChatService : IChatService
{
    private readonly IProjectRepository _projects;
    private readonly IChatMessageRepository _messages;
    private readonly IDirectMessageRepository _directMessages;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public ChatService(
        IProjectRepository projects,
        IChatMessageRepository messages,
        IDirectMessageRepository directMessages,
        UserManager<AppUser> userManager,
        IWebHostEnvironment env)
    {
        _projects = projects;
        _messages = messages;
        _directMessages = directMessages;
        _userManager = userManager;
        _env = env;
    }

    public async Task<(Project? project, IEnumerable<ChatMessage> messages, IEnumerable<AppUser> chatUsers)>
        GetChatDataAsync(int projectId, string userName)
    {
        var project = await _projects.GetByIdWithChatAsync(projectId);
        if (project == null) return (null, [], []);

        var msgs = await _messages.GetProjectMessagesAsync(projectId, userName);

        var projectUserIds = (await _projects.GetProjectUserIdsAsync(projectId))
            .Where(id => id != null)
            .ToHashSet();

        var admins = await _userManager.GetUsersInRoleAsync("Admin");

        var chatUsers = (await _userManager.Users.ToListAsync())
            .Where(u => projectUserIds.Contains(u.Id) || admins.Any(a => a.Id == u.Id))
            .Where(u => u.UserName != userName)
            .ToList();

        return (project, msgs, chatUsers);
    }

    public async Task<(AppUser? otherUser, List<DirectMessage> messages, int unreadCount)>
        GetDirectChatDataAsync(string currentUserName, string otherUserName)
    {
        var otherUser = await _userManager.FindByNameAsync(otherUserName);
        if (otherUser == null) return (null, [], 0);

        var messages = await _directMessages.GetConversationAsync(currentUserName, otherUserName);

        var unread = messages.Count(m => m.RecipientUserName == currentUserName && !m.IsRead);

        await _directMessages.MarkReadAsync(otherUserName, currentUserName);

        return (otherUser, messages, unread);
    }

    public async Task<List<ConversationSummary>> GetInboxAsync(string userName)
    {
        var lastMessages = await _directMessages.GetInboxSummaryAsync(userName);
        var summaries = new List<ConversationSummary>();

        foreach (var msg in lastMessages)
        {
            var otherUserName = msg.SenderUserName == userName
                ? msg.RecipientUserName
                : msg.SenderUserName;

            var otherUser = await _userManager.FindByNameAsync(otherUserName);
            var displayName = otherUser?.DisplayName.Length > 0
                ? otherUser.DisplayName
                : otherUserName;

            var conversation = await _directMessages.GetConversationAsync(userName, otherUserName);
            var unread = conversation.Count(m => m.RecipientUserName == userName && !m.IsRead);

            summaries.Add(new ConversationSummary
            {
                OtherUserName = otherUserName,
                OtherDisplayName = displayName ?? otherUserName,
                LastMessageContent = msg.Content.Length > 60
                    ? msg.Content[..60] + "…"
                    : msg.Content,
                LastMessageAt = msg.SentAt,
                UnreadCount = unread
            });
        }

        return summaries.OrderByDescending(s => s.LastMessageAt).ToList();
    }

    public async Task<(string url, string name)> UploadFileAsync(IFormFile file)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return ($"/uploads/{fileName}", file.FileName);
    }
}

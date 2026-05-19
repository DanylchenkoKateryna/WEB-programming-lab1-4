using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Models.ViewModels;

namespace lab1_4.Services.Interfaces;

public interface IChatService
{
    Task<(Project? project, IEnumerable<ChatMessage> messages, IEnumerable<AppUser> chatUsers)>
        GetChatDataAsync(int projectId, string userName);

    Task<(AppUser? otherUser, List<DirectMessage> messages, int unreadCount)>
        GetDirectChatDataAsync(string currentUserName, string otherUserName);

    Task<List<ConversationSummary>> GetInboxAsync(string userName);

    Task<(string url, string name)> UploadFileAsync(IFormFile file);
}

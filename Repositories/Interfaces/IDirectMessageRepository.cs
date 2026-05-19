using lab1_4.Models;

namespace lab1_4.Repositories.Interfaces;

public interface IDirectMessageRepository
{
    Task<List<DirectMessage>> GetConversationAsync(string user1, string user2);
    Task<List<DirectMessage>> GetInboxSummaryAsync(string userName);
    Task MarkReadAsync(string senderUserName, string recipientUserName);
    Task AddAsync(DirectMessage message);
}

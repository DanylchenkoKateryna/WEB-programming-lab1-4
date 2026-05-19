using lab1_4.Models;

namespace lab1_4.Repositories.Interfaces;

public interface IChatMessageRepository
{
    Task<IEnumerable<ChatMessage>> GetProjectMessagesAsync(int projectId, string userName);
    Task AddAsync(ChatMessage message);
}

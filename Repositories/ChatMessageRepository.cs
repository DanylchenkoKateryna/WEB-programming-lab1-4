using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Repositories;

public class ChatMessageRepository : IChatMessageRepository
{
    private readonly AppDbContext _context;

    public ChatMessageRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<ChatMessage>> GetProjectMessagesAsync(int projectId, string userName) =>
        await _context.ChatMessages
            .Where(m => m.ProjectId == projectId &&
                (m.RecipientUserName == null ||
                 m.SenderUserName == userName ||
                 m.RecipientUserName == userName))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

    public async Task AddAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
    }
}

using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Repositories;

public class DirectMessageRepository : IDirectMessageRepository
{
    private readonly AppDbContext _context;

    public DirectMessageRepository(AppDbContext context) => _context = context;

    public async Task<List<DirectMessage>> GetConversationAsync(string user1, string user2) =>
        await _context.DirectMessages
            .Where(m =>
                (m.SenderUserName == user1 && m.RecipientUserName == user2) ||
                (m.SenderUserName == user2 && m.RecipientUserName == user1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

    /// Returns the latest message from each distinct conversation for the inbox list.
    public async Task<List<DirectMessage>> GetInboxSummaryAsync(string userName)
    {
        var all = await _context.DirectMessages
            .Where(m => m.SenderUserName == userName || m.RecipientUserName == userName)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        // One entry per conversation partner — most recent message
        return all
            .GroupBy(m => m.SenderUserName == userName ? m.RecipientUserName : m.SenderUserName)
            .Select(g => g.First())
            .ToList();
    }

    public async Task MarkReadAsync(string senderUserName, string recipientUserName)
    {
        var unread = await _context.DirectMessages
            .Where(m => m.SenderUserName == senderUserName &&
                        m.RecipientUserName == recipientUserName &&
                        !m.IsRead)
            .ToListAsync();

        foreach (var m in unread)
            m.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(DirectMessage message)
    {
        _context.DirectMessages.Add(message);
        await _context.SaveChangesAsync();
    }
}

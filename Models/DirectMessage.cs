namespace lab1_4.Models;

public class DirectMessage
{
    public int Id { get; set; }
    public string SenderUserName { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
    public string RecipientUserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

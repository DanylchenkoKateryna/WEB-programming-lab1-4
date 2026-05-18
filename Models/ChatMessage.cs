namespace lab1_4.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string SenderUserName { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? RecipientUserName { get; set; }
    public bool IsPrivate => RecipientUserName != null;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

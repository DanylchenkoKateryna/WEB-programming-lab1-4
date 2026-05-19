namespace lab1_4.Models.ViewModels;

public class ConversationSummary
{
    public string OtherUserName { get; set; } = string.Empty;
    public string OtherDisplayName { get; set; } = string.Empty;
    public string LastMessageContent { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
}

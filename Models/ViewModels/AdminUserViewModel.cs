namespace lab1_4.Models.ViewModels;

public class AdminUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public int? LinkedEmployeeId { get; set; }
    public string? LinkedEmployeeName { get; set; }
}

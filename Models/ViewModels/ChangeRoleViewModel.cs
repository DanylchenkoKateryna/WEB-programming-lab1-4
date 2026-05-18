namespace lab1_4.Models.ViewModels;

public class ChangeRoleViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public IList<string> UserRoles { get; set; } = new List<string>();
    public IList<string> AllRoles { get; set; } = new List<string>();
}

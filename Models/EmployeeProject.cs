namespace lab1_4.Models;

public class EmployeeProject
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string Role { get; set; } = "Member";
}

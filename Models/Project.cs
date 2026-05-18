using System.ComponentModel.DataAnnotations;

namespace lab1_4.Models;

public enum ProjectStatus { Planning, Active, OnHold, Completed, Cancelled }
public enum ProjectPriority { Low, Medium, High, Critical }

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be 3-100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    [Required]
    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

    [Required]
    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

    [Range(0, 100_000_000, ErrorMessage = "Budget must be between 0 and 100,000,000")]
    [DataType(DataType.Currency)]
    public decimal Budget { get; set; }

    [Display(Name = "Client Name")]
    [StringLength(100)]
    public string? ClientName { get; set; }

    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();

    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}

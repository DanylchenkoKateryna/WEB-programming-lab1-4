using lab1_4.Models;
using lab1_4.Models.ViewModels;
using lab1_4.Repositories.Interfaces;
using lab1_4.Services.Interfaces;

namespace lab1_4.Services;

public class HomeService : IHomeService
{
    private readonly IEmployeeRepository _employees;
    private readonly IProjectRepository _projects;

    public HomeService(IEmployeeRepository employees, IProjectRepository projects)
    {
        _employees = employees;
        _projects = projects;
    }

    public async Task<DashboardStats> GetStatsAsync()
    {
        var employees = await _employees.CountAsync();
        var projects = await _projects.CountAsync();
        var active = await _projects.CountByStatusAsync(ProjectStatus.Active);
        return new DashboardStats(employees, projects, active);
    }
}

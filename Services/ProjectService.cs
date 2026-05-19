using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using lab1_4.Services.Interfaces;

namespace lab1_4.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projects;

    public ProjectService(IProjectRepository projects) => _projects = projects;

    public Task<IEnumerable<Project>> GetAllAsync() => _projects.GetAllAsync();

    public Task<Project?> GetByIdAsync(int id) => _projects.GetByIdAsync(id);

    public Task CreateAsync(Project project) => _projects.AddAsync(project);

    public async Task<bool> UpdateAsync(int id, Project project)
    {
        if (id != project.Id) return false;
        await _projects.UpdateAsync(project);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _projects.GetByIdAsync(id);
        if (project == null) return false;
        await _projects.DeleteAsync(id);
        return true;
    }

    public async Task<(Project? project, IEnumerable<Employee> availableEmployees)> GetAssignDataAsync(int projectId)
    {
        var project = await _projects.GetByIdAsync(projectId);
        if (project == null) return (null, Enumerable.Empty<Employee>());
        var available = await _projects.GetAvailableEmployeesAsync(projectId);
        return (project, available);
    }

    public async Task<bool> AssignEmployeeAsync(int projectId, int employeeId, string role)
    {
        if (await _projects.IsEmployeeAssignedAsync(projectId, employeeId)) return false;
        await _projects.AssignEmployeeAsync(new EmployeeProject
        {
            ProjectId = projectId,
            EmployeeId = employeeId,
            Role = role
        });
        return true;
    }

    public Task RemoveEmployeeAsync(int projectId, int employeeId) =>
        _projects.RemoveEmployeeAsync(projectId, employeeId);
}

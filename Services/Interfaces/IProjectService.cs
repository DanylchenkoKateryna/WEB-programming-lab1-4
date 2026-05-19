using lab1_4.Models;

namespace lab1_4.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task CreateAsync(Project project);
    Task<bool> UpdateAsync(int id, Project project);
    Task<bool> DeleteAsync(int id);
    Task<(Project? project, IEnumerable<Employee> availableEmployees)> GetAssignDataAsync(int projectId);
    Task<bool> AssignEmployeeAsync(int projectId, int employeeId, string role);
    Task RemoveEmployeeAsync(int projectId, int employeeId);
}

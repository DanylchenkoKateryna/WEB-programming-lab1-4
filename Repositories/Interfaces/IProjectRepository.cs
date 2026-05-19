using lab1_4.Models;

namespace lab1_4.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project?> GetByIdWithChatAsync(int id);
    Task<int> CountAsync();
    Task<int> CountByStatusAsync(ProjectStatus status);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(int id);
    Task<bool> IsEmployeeAssignedAsync(int projectId, int employeeId);
    Task AssignEmployeeAsync(EmployeeProject ep);
    Task RemoveEmployeeAsync(int projectId, int employeeId);
    Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int projectId);
    Task<List<string?>> GetProjectUserIdsAsync(int projectId);
}

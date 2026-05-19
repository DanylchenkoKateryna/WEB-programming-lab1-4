using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Project>> GetAllAsync() =>
        await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects).ThenInclude(ep => ep.Employee)
            .ToListAsync();

    public async Task<Project?> GetByIdAsync(int id) =>
        await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects).ThenInclude(ep => ep.Employee)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project?> GetByIdWithChatAsync(int id) =>
        await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects).ThenInclude(ep => ep.Employee)
            .Include(p => p.ChatMessages)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<int> CountAsync() => _context.Projects.CountAsync();

    public Task<int> CountByStatusAsync(ProjectStatus status) =>
        _context.Projects.CountAsync(p => p.Status == status);

    public async Task AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        var existing = await _context.Projects.FindAsync(project.Id);
        if (existing == null) return;
        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.StartDate = project.StartDate;
        existing.EndDate = project.EndDate;
        existing.Status = project.Status;
        existing.Priority = project.Priority;
        existing.Budget = project.Budget;
        existing.ClientName = project.ClientName;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsEmployeeAssignedAsync(int projectId, int employeeId) =>
        await _context.EmployeeProjects
            .AnyAsync(ep => ep.ProjectId == projectId && ep.EmployeeId == employeeId);

    public async Task AssignEmployeeAsync(EmployeeProject ep)
    {
        _context.EmployeeProjects.Add(ep);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEmployeeAsync(int projectId, int employeeId)
    {
        var ep = await _context.EmployeeProjects.FindAsync(employeeId, projectId);
        if (ep != null)
        {
            _context.EmployeeProjects.Remove(ep);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(int projectId)
    {
        var assignedIds = await _context.EmployeeProjects
            .Where(ep => ep.ProjectId == projectId)
            .Select(ep => ep.EmployeeId)
            .ToListAsync();

        return await _context.Employees
            .Where(e => !assignedIds.Contains(e.Id))
            .ToListAsync();
    }

    public async Task<List<string?>> GetProjectUserIdsAsync(int projectId) =>
        await _context.EmployeeProjects
            .Where(ep => ep.ProjectId == projectId)
            .Include(ep => ep.Employee)
            .Select(ep => ep.Employee.UserId)
            .ToListAsync();
}

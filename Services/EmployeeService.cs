using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly UserManager<AppUser> _userManager;

    public EmployeeService(IEmployeeRepository employees, UserManager<AppUser> userManager)
    {
        _employees = employees;
        _userManager = userManager;
    }

    public Task<IEnumerable<Employee>> GetAllAsync() => _employees.GetAllAsync();

    public Task<Employee?> GetByIdAsync(int id) => _employees.GetByIdAsync(id);

    public Task<Employee?> GetByUserIdAsync(string userId) => _employees.GetByUserIdAsync(userId);

    public Task CreateAsync(Employee employee) => _employees.AddAsync(employee);

    public async Task<bool> UpdateAsync(int id, Employee employee, string? linkedUserId)
    {
        var existing = await _employees.GetByIdAsync(id);
        if (existing == null) return false;

        if (!string.IsNullOrEmpty(linkedUserId))
        {
            var conflicting = await _employees.GetByUserIdAsync(linkedUserId);
            if (conflicting != null && conflicting.Id != id)
            {
                conflicting.UserId = null;
                await _employees.UpdateAsync(conflicting);
            }
        }

        employee.UserId = string.IsNullOrEmpty(linkedUserId) ? null : linkedUserId;
        await _employees.UpdateAsync(employee);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _employees.GetByIdAsync(id);
        if (existing == null) return false;
        await _employees.DeleteAsync(id);
        return true;
    }

    public async Task<(Employee? employee, IEnumerable<AppUser> users)> GetEditDataAsync(int id)
    {
        var employee = await _employees.GetByIdAsync(id);
        var users = await _userManager.Users.ToListAsync();
        return (employee, users);
    }
}

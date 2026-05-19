using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Models.ViewModels;
using lab1_4.Repositories.Interfaces;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmployeeRepository _employees;

    public AdminService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmployeeRepository employees)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _employees = employees;
    }

    public async Task<(IEnumerable<AdminUserViewModel> users, IEnumerable<Employee> employees)> GetDashboardAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var employees = await _employees.GetAllAsync();

        var employeeByUserId = employees
            .Where(e => e.UserId != null)
            .ToDictionary(e => e.UserId!, e => e);

        var viewModels = new List<AdminUserViewModel>();
        foreach (var user in users)
        {
            employeeByUserId.TryGetValue(user.Id, out var emp);
            viewModels.Add(new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                Roles = await _userManager.GetRolesAsync(user),
                LinkedEmployeeId = emp?.Id,
                LinkedEmployeeName = emp?.FullName
            });
        }

        return (viewModels, employees);
    }

    public async Task<ChangeRoleViewModel?> GetEditRoleDataAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        return new ChangeRoleViewModel
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            UserRoles = await _userManager.GetRolesAsync(user),
            AllRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync()
        };
    }

    public async Task UpdateRolesAsync(string userId, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return;
        var current = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, current);
        if (roles.Any())
            await _userManager.AddToRolesAsync(user, roles);
        // Invalidate existing auth cookies for this user on all devices
        await _userManager.UpdateSecurityStampAsync(user);
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
            await _userManager.DeleteAsync(user);
    }

    public async Task LinkEmployeeAsync(string userId, int? employeeId)
    {
        // Unlink any employee currently linked to this user
        var prev = await _employees.GetByUserIdAsync(userId);
        if (prev != null)
        {
            prev.UserId = null;
            await _employees.UpdateAsync(prev);
        }

        if (!employeeId.HasValue) return;

        var emp = await _employees.GetByIdAsync(employeeId.Value);
        if (emp == null) return;

        // Unlink the target employee from its previous user if different
        if (emp.UserId != null && emp.UserId != userId)
        {
            emp.UserId = null;
            await _employees.UpdateAsync(emp);
        }

        emp.UserId = userId;
        await _employees.UpdateAsync(emp);
    }
}

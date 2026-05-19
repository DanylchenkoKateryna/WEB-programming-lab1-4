using lab1_4.Models;
using lab1_4.Models.ViewModels;

namespace lab1_4.Services.Interfaces;

public interface IAdminService
{
    Task<(IEnumerable<AdminUserViewModel> users, IEnumerable<Employee> employees)> GetDashboardAsync();
    Task<ChangeRoleViewModel?> GetEditRoleDataAsync(string userId);
    Task UpdateRolesAsync(string userId, List<string> roles);
    Task DeleteUserAsync(string id);
    Task LinkEmployeeAsync(string userId, int? employeeId);
}

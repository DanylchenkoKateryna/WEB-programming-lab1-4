using lab1_4.Data;
using lab1_4.Models;

namespace lab1_4.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByUserIdAsync(string userId);
    Task CreateAsync(Employee employee);
    Task<bool> UpdateAsync(int id, Employee employee, string? linkedUserId);
    Task<bool> DeleteAsync(int id);
    Task<(Employee? employee, IEnumerable<AppUser> users)> GetEditDataAsync(int id);
}

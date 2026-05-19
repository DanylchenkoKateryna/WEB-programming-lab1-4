using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Employee>> GetAllAsync() =>
        await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeProjects).ThenInclude(ep => ep.Project)
            .ToListAsync();

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeProjects).ThenInclude(ep => ep.Project)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByUserIdAsync(string userId) =>
        await _context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeProjects).ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.UserId == userId);

    public Task<int> CountAsync() => _context.Employees.CountAsync();

    public async Task AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employee)
    {
        var existing = await _context.Employees.FindAsync(employee.Id);
        if (existing == null) return;
        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;
        existing.Email = employee.Email;
        existing.Position = employee.Position;
        existing.Department = employee.Department;
        existing.HireDate = employee.HireDate;
        existing.Salary = employee.Salary;
        existing.UserId = employee.UserId;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}

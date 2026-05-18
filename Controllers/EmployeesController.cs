using lab1_4.Data;
using lab1_4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace lab1_4.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _context;
    private const string SessionKeyPrefix = "Employee_";

    public EmployeesController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .ToListAsync();
        return View(employees);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [Authorize]
    public IActionResult Create()
    {
        var sessionKey = $"{SessionKeyPrefix}Create";
        var json = HttpContext.Session.GetString(sessionKey);
        var model = json != null ? JsonSerializer.Deserialize<Employee>(json) : new Employee();
        return View(model);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        var sessionKey = $"{SessionKeyPrefix}Create";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(employee));
            return View(employee);
        }
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(sessionKey);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var sessionKey = $"{SessionKeyPrefix}Edit_{id}";
        var json = HttpContext.Session.GetString(sessionKey);
        Employee? employee;
        if (json != null)
        {
            employee = JsonSerializer.Deserialize<Employee>(json);
        }
        else
        {
            employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(employee));
        }
        return View(employee);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id) return BadRequest();
        var sessionKey = $"{SessionKeyPrefix}Edit_{id}";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(employee));
            return View(employee);
        }
        _context.Update(employee);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(sessionKey);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.EmployeeProjects)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete"), Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}

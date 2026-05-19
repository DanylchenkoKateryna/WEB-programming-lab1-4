using lab1_4.Models;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lab1_4.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeService _service;
    private const string SessionPrefix = "Employee_";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public EmployeesController(IEmployeeService service) => _service = service;

    public async Task<IActionResult> Index() =>
        View(await _service.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _service.GetByIdAsync(id);
        return employee == null ? NotFound() : View(employee);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        var json = HttpContext.Session.GetString($"{SessionPrefix}Create");
        return View(json != null ? JsonSerializer.Deserialize<Employee>(json, _jsonOptions) : new Employee());
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        var key = $"{SessionPrefix}Create";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(employee, _jsonOptions));
            return View(employee);
        }
        await _service.CreateAsync(employee);
        HttpContext.Session.Remove(key);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var key = $"{SessionPrefix}Edit_{id}";
        var (dbEmployee, users) = await _service.GetEditDataAsync(id);
        if (dbEmployee == null) return NotFound();
        ViewBag.AllUsers = users;

        var json = HttpContext.Session.GetString(key);
        if (json == null)
        {
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(dbEmployee, _jsonOptions));
            return View(dbEmployee);
        }
        return View(JsonSerializer.Deserialize<Employee>(json, _jsonOptions)!);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee, string? linkedUserId)
    {
        var key = $"{SessionPrefix}Edit_{id}";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(employee, _jsonOptions));
            var (_, users) = await _service.GetEditDataAsync(id);
            ViewBag.AllUsers = users;
            return View(employee);
        }
        var updated = await _service.UpdateAsync(id, employee, linkedUserId);
        if (!updated) return NotFound();
        HttpContext.Session.Remove(key);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _service.GetByIdAsync(id);
        return employee == null ? NotFound() : View(employee);
    }

    [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}

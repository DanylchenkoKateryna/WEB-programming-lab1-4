using lab1_4.Models;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace lab1_4.Controllers;

public class ProjectsController : Controller
{
    private readonly IProjectService _service;
    private const string SessionPrefix = "Project_";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public ProjectsController(IProjectService service) => _service = service;

    public async Task<IActionResult> Index() =>
        View(await _service.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var project = await _service.GetByIdAsync(id);
        return project == null ? NotFound() : View(project);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        var json = HttpContext.Session.GetString($"{SessionPrefix}Create");
        return View(json != null ? JsonSerializer.Deserialize<Project>(json, _jsonOptions) : new Project());
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        var key = $"{SessionPrefix}Create";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(project, _jsonOptions));
            return View(project);
        }
        await _service.CreateAsync(project);
        HttpContext.Session.Remove(key);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var key = $"{SessionPrefix}Edit_{id}";
        var json = HttpContext.Session.GetString(key);
        if (json != null)
            return View(JsonSerializer.Deserialize<Project>(json, _jsonOptions));

        var project = await _service.GetByIdAsync(id);
        if (project == null) return NotFound();
        HttpContext.Session.SetString(key, JsonSerializer.Serialize(project, _jsonOptions));
        return View(project);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        var key = $"{SessionPrefix}Edit_{id}";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(key, JsonSerializer.Serialize(project, _jsonOptions));
            return View(project);
        }
        var updated = await _service.UpdateAsync(id, project);
        if (!updated) return BadRequest();
        HttpContext.Session.Remove(key);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _service.GetByIdAsync(id);
        return project == null ? NotFound() : View(project);
    }

    [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignEmployee(int id)
    {
        var (project, available) = await _service.GetAssignDataAsync(id);
        if (project == null) return NotFound();
        ViewBag.AvailableEmployees = available;
        return View(project);
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignEmployee(int id, int employeeId, string role = "Member")
    {
        await _service.AssignEmployeeAsync(id, employeeId, role);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEmployee(int projectId, int employeeId)
    {
        await _service.RemoveEmployeeAsync(projectId, employeeId);
        return RedirectToAction(nameof(Details), new { id = projectId });
    }
}

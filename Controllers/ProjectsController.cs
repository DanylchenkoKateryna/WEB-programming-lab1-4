using lab1_4.Data;
using lab1_4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace lab1_4.Controllers;

public class ProjectsController : Controller
{
    private readonly AppDbContext _context;
    private const string SessionKeyPrefix = "Project_";

    public ProjectsController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .ThenInclude(ep => ep.Employee)
            .ToListAsync();
        return View(projects);
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .ThenInclude(ep => ep.Employee)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return NotFound();
        return View(project);
    }

    [Authorize]
    public IActionResult Create()
    {
        var sessionKey = $"{SessionKeyPrefix}Create";
        var json = HttpContext.Session.GetString(sessionKey);
        var model = json != null ? JsonSerializer.Deserialize<Project>(json) : new Project();
        return View(model);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        var sessionKey = $"{SessionKeyPrefix}Create";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(project));
            return View(project);
        }
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(sessionKey);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var sessionKey = $"{SessionKeyPrefix}Edit_{id}";
        var json = HttpContext.Session.GetString(sessionKey);
        Project? project;
        if (json != null)
        {
            project = JsonSerializer.Deserialize<Project>(json);
        }
        else
        {
            project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(project));
        }
        return View(project);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        if (id != project.Id) return BadRequest();
        var sessionKey = $"{SessionKeyPrefix}Edit_{id}";
        if (!ModelState.IsValid)
        {
            HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(project));
            return View(project);
        }
        _context.Update(project);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(sessionKey);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return NotFound();
        return View(project);
    }

    [HttpPost, ActionName("Delete"), Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> AssignEmployee(int id)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .ThenInclude(ep => ep.Employee)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return NotFound();

        var assignedIds = project.EmployeeProjects.Select(ep => ep.EmployeeId).ToHashSet();
        ViewBag.AvailableEmployees = await _context.Employees
            .Where(e => !assignedIds.Contains(e.Id))
            .ToListAsync();
        return View(project);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignEmployee(int id, int employeeId, string role = "Member")
    {
        var exists = await _context.EmployeeProjects.AnyAsync(ep => ep.ProjectId == id && ep.EmployeeId == employeeId);
        if (!exists)
        {
            _context.EmployeeProjects.Add(new EmployeeProject { ProjectId = id, EmployeeId = employeeId, Role = role });
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEmployee(int projectId, int employeeId)
    {
        var ep = await _context.EmployeeProjects.FindAsync(employeeId, projectId);
        if (ep != null)
        {
            _context.EmployeeProjects.Remove(ep);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = projectId });
    }
}

using lab1_4.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var stats = new
        {
            Employees = await _context.Employees.CountAsync(),
            Projects = await _context.Projects.CountAsync(),
            ActiveProjects = await _context.Projects.CountAsync(p => p.Status == lab1_4.Models.ProjectStatus.Active)
        };
        ViewBag.Stats = stats;
        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new lab1_4.Models.ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

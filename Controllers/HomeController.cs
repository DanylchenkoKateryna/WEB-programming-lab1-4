using lab1_4.Models;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lab1_4.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _service;

    public HomeController(IHomeService service) => _service = service;

    public async Task<IActionResult> Index()
    {
        ViewBag.Stats = await _service.GetStatsAsync();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}

using lab1_4.Data;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace lab1_4.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _service;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AdminController(
        IAdminService service,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _service = service;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        var (users, employees) = await _service.GetDashboardAsync();
        ViewBag.AllEmployees = employees;
        return View(users);
    }

    public async Task<IActionResult> EditRole(string id)
    {
        var model = await _service.GetEditRoleDataAsync(id);
        return model == null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(string userId, List<string> roles)
    {
        await _service.UpdateRolesAsync(userId, roles);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == currentUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _signInManager.RefreshSignInAsync(user);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _service.DeleteUserAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LinkEmployee(string userId, int? employeeId)
    {
        await _service.LinkEmployeeAsync(userId, employeeId);
        return RedirectToAction(nameof(Index));
    }
}

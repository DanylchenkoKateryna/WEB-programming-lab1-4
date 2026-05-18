using lab1_4.Data;
using lab1_4.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<AdminUserViewModel>();
        foreach (var user in users)
        {
            result.Add(new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }
        return View(result);
    }

    public async Task<IActionResult> EditRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var allRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        return View(new ChangeRoleViewModel
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            UserRoles = await _userManager.GetRolesAsync(user),
            AllRoles = allRoles
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(string userId, List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (roles.Any())
            await _userManager.AddToRolesAsync(user, roles);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
            await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }
}

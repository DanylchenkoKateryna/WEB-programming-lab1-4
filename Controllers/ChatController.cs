using lab1_4.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ChatController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .ThenInclude(ep => ep.Employee)
            .Include(p => p.ChatMessages)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        ViewBag.CurrentUser = user?.UserName ?? string.Empty;
        ViewBag.CurrentDisplayName = user?.DisplayName ?? user?.UserName ?? string.Empty;

        var projectUsers = await _context.EmployeeProjects
            .Where(ep => ep.ProjectId == projectId)
            .ToListAsync();

        var messages = await _context.ChatMessages
            .Where(m => m.ProjectId == projectId &&
                (!m.IsPrivate || m.SenderUserName == user!.UserName || m.RecipientUserName == user.UserName))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        ViewBag.Messages = messages;
        ViewBag.ProjectUsers = await _userManager.Users.ToListAsync();
        return View(project);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file, int projectId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file");

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsDir);
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return Json(new { url = $"/uploads/{fileName}", name = file.FileName });
    }
}

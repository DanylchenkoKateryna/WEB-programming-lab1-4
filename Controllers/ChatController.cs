using lab1_4.Data;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _service;
    private readonly UserManager<AppUser> _userManager;

    public ChatController(IChatService service, UserManager<AppUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    // GET /Chat/Index?projectId=1  — project group chat
    public async Task<IActionResult> Index(int projectId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var (project, messages, chatUsers) = await _service.GetChatDataAsync(projectId, user.UserName!);
        if (project == null) return NotFound();

        ViewBag.CurrentUser = user.UserName;
        ViewBag.CurrentDisplayName = user.DisplayName.Length > 0 ? user.DisplayName : user.UserName;
        ViewBag.Messages = messages;
        ViewBag.ProjectUsers = chatUsers;
        return View(project);
    }

    // GET /Chat/Inbox  — list of all DM conversations
    public async Task<IActionResult> Inbox()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var summaries = await _service.GetInboxAsync(user.UserName!);

        // All users except self for "New conversation" list
        ViewBag.AllUsers = await _userManager.Users
            .Where(u => u.UserName != user.UserName)
            .ToListAsync();

        return View(summaries);
    }

    // GET /Chat/Direct?userName=other@example.com  — private chat with one user
    public async Task<IActionResult> Direct(string userName)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var (otherUser, messages, _) = await _service.GetDirectChatDataAsync(currentUser.UserName!, userName);
        if (otherUser == null) return NotFound();

        ViewBag.CurrentUser = currentUser.UserName;
        ViewBag.CurrentDisplayName = currentUser.DisplayName.Length > 0
            ? currentUser.DisplayName : currentUser.UserName;
        ViewBag.OtherUser = otherUser;
        ViewBag.Messages = messages;
        return View();
    }

    // POST /Chat/UploadFile
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file, int projectId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var (url, name) = await _service.UploadFileAsync(file);
        return Json(new { url, name });
    }
}

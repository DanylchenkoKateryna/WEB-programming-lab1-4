using lab1_4.Data;
using lab1_4.Models;
using lab1_4.Repositories.Interfaces;
using lab1_4.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace lab1_4.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmployeeRepository _employees;

    public AccountService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IEmployeeRepository employees)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _employees = employees;
    }

    public async Task<IdentityResult> RegisterAsync(string email, string password, string displayName)
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName ?? email,
            EmailConfirmed = true
        };
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
        return result;
    }

    public async Task<SignInResult> LoginAsync(string email, string password) =>
        await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

    public Task LogoutAsync() => _signInManager.SignOutAsync();

    public async Task<(AppUser? user, Employee? employee, IList<string> roles)> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return (null, null, []);
        var employee = await _employees.GetByUserIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);
        return (user, employee, roles);
    }
}

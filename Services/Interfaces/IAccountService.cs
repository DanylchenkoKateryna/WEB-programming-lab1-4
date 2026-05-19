using lab1_4.Data;
using lab1_4.Models;
using Microsoft.AspNetCore.Identity;

namespace lab1_4.Services.Interfaces;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(string email, string password, string displayName);
    Task<SignInResult> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<(AppUser? user, Employee? employee, IList<string> roles)> GetProfileAsync(string userId);
}

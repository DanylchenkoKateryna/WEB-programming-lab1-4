using Microsoft.AspNetCore.Identity;

namespace lab1_4.Data;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}

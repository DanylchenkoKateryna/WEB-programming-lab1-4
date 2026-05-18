using Microsoft.AspNetCore.Identity;

namespace lab1_4.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByEmailAsync("admin@pm.com") == null)
        {
            var admin = new AppUser { UserName = "admin@pm.com", Email = "admin@pm.com", DisplayName = "Administrator", EmailConfirmed = true };
            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

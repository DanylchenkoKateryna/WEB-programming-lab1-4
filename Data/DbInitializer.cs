using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lab1_4.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();

        // Retry until SQL Server is ready (it starts slower than the app)
        for (int attempt = 1; attempt <= 10; attempt++)
        {
            try
            {
                await context.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (attempt < 10)
            {
                logger.LogWarning("DB not ready (attempt {Attempt}/10): {Message}. Retrying in 3s...", attempt, ex.Message);
                await Task.Delay(3000);
            }
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByEmailAsync("admin@pm.com") == null)
        {
            var admin = new AppUser
            {
                UserName = "admin@pm.com",
                Email = "admin@pm.com",
                DisplayName = "Administrator",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

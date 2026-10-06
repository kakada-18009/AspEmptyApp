namespace AspNetEmptyApp.Data;

using System.Security.Claims;
using AspNetEmptyApp.Feature.Auth.Model;
using AspNetEmptyApp.Feature.Inventory.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DbSeeder
{
    private static readonly string[] Permissions = [
        "product.create",
        "product.read",
        "category.create",
        "category.read"
    ];

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await context.Database.MigrateAsync();

        // 1. Seed Roles & Role Permissions
        string[] roles = ["Admin", "Customer"];
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpper() };
                await roleManager.CreateAsync(role);
            }

            var existingRoleClaims = await roleManager.GetClaimsAsync(role);
            foreach (var perm in Permissions)
            {
                if (!existingRoleClaims.Any(c => c.Type == "Permission" && c.Value == perm))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", perm));
                }
            }
        }

        // 2. Seed Customer User
        var customerEmail = "customer@example.com";
        var customerUser = await userManager.FindByEmailAsync(customerEmail);
        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = "customer",
                Email = customerEmail,
                EmailConfirmed = true,
                PhoneNumber = "123456789"
            };
            await userManager.CreateAsync(customerUser, "Customer123!");
            await userManager.AddToRoleAsync(customerUser, "Customer");
        }

        var customerClaims = await userManager.GetClaimsAsync(customerUser);
        foreach (var perm in Permissions)
        {
            if (!customerClaims.Any(c => c.Type == "Permission" && c.Value == perm))
            {
                await userManager.AddClaimAsync(customerUser, new Claim("Permission", perm));
            }
        }

        // 3. Seed Admin User
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumber = "987654321"
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var adminClaims = await userManager.GetClaimsAsync(adminUser);
        foreach (var perm in Permissions)
        {
            if (!adminClaims.Any(c => c.Type == "Permission" && c.Value == perm))
            {
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", perm));
            }
        }

        // 4. Seed a default Category if none exists
        if (!await context.Categories.AnyAsync())
        {
            var defaultCategory = new Category
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Electronics",
                Description = "Electronic items and gadgets",
                CreatedBy = customerUser?.Id ?? Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            context.Categories.Add(defaultCategory);
            await context.SaveChangesAsync();
        }
    }
}

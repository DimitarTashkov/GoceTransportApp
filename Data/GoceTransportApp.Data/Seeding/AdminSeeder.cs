using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GoceTransportApp.Data;
using GoceTransportApp.Data.Models;
using GoceTransportApp.Data.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public class AdminSeeder : ISeeder
{

    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        const string adminRole = "Administrator";
        const string adminEmail = "mitkoadmin@gmail.com";
        const string adminPassword = "\"mitko123\"";

        // Ensure Admin Role Exists
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new ApplicationRole(adminRole));
        }

        // Check if admin user exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Mitko",
                LastName = "Admin",
                City = "Mosomishte",
                EmailConfirmed = true,
                CreatedOn = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class DbInitializer
{
    public static async Task InitializeDataAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        // Get a logger
        var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

        // Perform any missing migrations.
        await context.Database.MigrateAsync();

        if (context.Pages.Any())
        {
            logger.LogInformation("The database already has a page seeded");
        }
        else
        {
            logger.LogInformation("Seeding a default page");

            string[] pages = new string[] { "Home", "Page1", "Page2", "Page3" };

            for (int i = 0; i < pages.Length; i++)
            {
                string guid = $"00000000-0000-0000-0000-00000000000{i}";
                context.Pages.Add(new Page
                {
                    Id = Guid.Parse(guid),
                    Author = "Administrator",
                    ContentDirectory = pages[i],
                    Title = pages[i],
                    Tags = "Test" + $", Tag{i}",
                });
            }
        }

        if (context.Users.Any())
        {
            logger.LogInformation("The database already has users seeded");
        }
        else
        {
            string?[] firstNames = new string?[] { "Jim", "Mike", "Karen", "Carl" };
            string?[] lastNames = new string[] { "Smith", "Jones", "Alen", "Washington" };
            for (int i = 0; i < 4; i++)
            {
                var email = $"{firstNames[i]}@sample.com";
                var user = new ApplicationUser(email);
                user.GivenName = firstNames[i];
                user.Surname = lastNames[i];
                user.DisplayName = $"{firstNames[i]} {lastNames[i]}";

                context.Users.Add(user);
            }
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }

        logger.LogInformation("Finished Seeding Data");
    }

    public static async Task InitializeUsersAsync(ApplicationDbContext context, IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        // Get a logger
        var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();
        
        //If there are any roles, the database has already been seeded.
        if (context.Roles.Any())
        {
            logger.LogInformation("The database already has roles seeded");
        }
        else
        {
            logger.LogInformation("Seeding roles");
            string[] roleNames = new string[]
            {
                "Admin",
                "Contributor",
                "Reader"
            };

            // Create and seed the new roles
            ApplicationRole[] roles = new ApplicationRole[roleNames.Length];

            for (int i = 0; i < roles.Length; i++)
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(), Name = roleNames[i], NormalizedName = roleNames[i].ToUpper()
                });
            }

            await context.SaveChangesAsync();
        }

        // Check if any users are in the "Administrator" role
        var administrators = await userManager.GetUsersInRoleAsync("Admin");
        if (administrators.Any())
            return;

        var adminUser = await userManager.FindByEmailAsync("admin@connex.com");

        try
        {
            await userManager.DeleteAsync(adminUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
        }
        await context.SaveChangesAsync();
        adminUser = null;

        if (null == adminUser)
        {
            adminUser = new ApplicationUser("admin@connex.com")
            {
                GivenName = "Admin",
                Surname = "User",
                DisplayName = "Administrator",
                TwoFactorEnabled = false
            };

            var result = await userManager.CreateAsync(adminUser, "Password!!11");
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create admin user");
                return;
            }
        }

        await userManager.AddToRoleAsync(adminUser, "Admin");
            await context.SaveChangesAsync();

       

        logger.LogInformation("Finished Seeding Admin User");
    }
}
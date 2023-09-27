using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        // Get a logger
        var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

        // Perform any missing migrations.
        await context.Database.MigrateAsync();

        //If there are any roles, the database has already been seeded.
        // if (context.Roles.Any())
        // {
        //     logger.LogInformation("The database already has roles seeded");
        // }
        // else
        // {
        //     logger.LogInformation("Seeding roles");
        //     string[] roleNames = new string[]
        //     {
        //         "Administrator",
        //         "Contributor",
        //         "Reader"
        //     };
        //
        //     // Create and seed the new roles
        //     ApplicationRole[] roles = new ApplicationRole[roleNames.Length];
        //
        //     for (int i = 0; i < roles.Length; i++)
        //     {
        //         context.Roles.Add(new ApplicationRole
        //         {
        //             Id = Guid.NewGuid().ToString(), Name = roleNames[i], NormalizedName = roleNames[i].ToUpper()
        //         });
        //     }
        //
        //     await context.SaveChangesAsync();
        // }

        if (context.Pages.Any())
        {
            logger.LogInformation("The database already has a page seeded");
        }

        logger.LogInformation("Seeding a default page");

        context.Pages.Add(new Page
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
            Author = "Administrator",
            ContentDirectory = "Home",
            Title = "Home",
            Tags = "Test"
        });

        string[] firstNames = new string[4] { "Jim", "Mike", "Karen", "Carl" };
        string[] lastNames = new string[4] { "Smith", "Jones", "Alen", "Washington" };
        for (int i = 0; i < 4; i++)
        {
            var email = $"{firstNames[i]}@sample.com";
            // context.Users.Add(new ApplicationUser(userName: email, givenName: firstNames[i], surname: lastNames[i], displayName: $"{firstNames[i]} {lastNames[i]}"));
        }

        await context.SaveChangesAsync();

        logger.LogInformation("Finished Seeding");
    }
}
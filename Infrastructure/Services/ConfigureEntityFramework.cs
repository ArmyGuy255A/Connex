using Domain.Common;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

/// <summary>
/// Configuration helper for Entity Framework and SQLite.
/// </summary>
public static partial class ConfigureServices
{
    /// <summary>
    /// Adds and configures Entity Framework with SQLite.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="settings">Application settings.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddEntityFrameworkSql(this IServiceCollection services,
        AppSettings settings)
    {
        // Check for database connection string
        if (string.IsNullOrEmpty(settings.ConnectionStrings?.SqliteConnectionString))
        {
            throw new Exception("SQLite connection string is empty");
        }

        // Add EF Core with SQLite support
        services.AddDbContext<ApplicationDbContext>();

        // // Add Identity
        // services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        //     {
        //         options.SignIn.RequireConfirmedAccount = false;
        //         options.User.RequireUniqueEmail = true;
        //     })
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddDefaultTokenProviders();
        //
        // // Register ApplicationDbContext for DI
        // services.AddScoped<ApplicationDbContext>(provider =>
        //     provider.GetService<ApplicationDbContext>() ?? throw new InvalidOperationException());

        return services;
    }
    
    public static async Task SeedDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            await DbInitializer.InitializeAsync(context, services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<DbInitializer>>();
            logger.LogError($"An error occurred while seeding the database. Message: {ex.Message}");
        }
    }

}
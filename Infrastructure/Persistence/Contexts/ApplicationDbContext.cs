using Domain.Common;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.Contexts;

/// <summary>
/// Entity Framework DB Context.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    private readonly IOptionsMonitor<AppSettings> _settings;


    public ApplicationDbContext(IOptionsMonitor<AppSettings> settings, DbContextOptions options) : base(options)
    {
        _settings = settings;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (_settings.CurrentValue.DatabaseSettings is { UseSqlite: true })
        {
            // connect to sqlite database
            options.UseSqlite(_settings.CurrentValue.ConnectionStrings.SqliteConnectionString, o => o
                .MigrationsAssembly("Infrastructure")
            );
        }
        else
        {
            // connect to sql server database
            options.UseSqlServer(_settings.CurrentValue.ConnectionStrings.SqlConnectionString, o => o
                .MigrationsAssembly("Infrastructure")
            );
        }
    }

    public DbSet<Page> Pages { get; set; } = null!;
}
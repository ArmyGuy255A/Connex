using Domain.Common;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var inAppSettings = builder.Configuration.GetSection("InAppSettings").Get<InAppSettings>();
if (inAppSettings == null) throw new Exception("The InAppSettings section is empty. Please check your appsettings.json file.");

// Add services to the container.
// Setup InAppSettings for Dependency Injection
builder.Services.Configure<InAppSettings>(builder.Configuration.GetSection("InAppSettings"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllersWithViews();

// Add custom services to the container
// builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddEntityFrameworkSql(inAppSettings);
builder.Services.AddWebApiServices();

// Add logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.AddSerilog();

var app = builder.Build();

// Seed the Database
await ConfigureEntityFramework.SeedDatabase(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseOpenApi();
app.UseSwaggerUi3(settings =>
{
    settings.Path = "/swagger";
});

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

Log.CloseAndFlush();


using Domain.Common;
using Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();
if (appSettings == null)
    throw new Exception("The AppSettings section is empty. Please check your appsettings.json file.");

// Add services to the container.

// Setup AppSettings for Dependency Injection
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllersWithViews();

// Add custom services to the container
builder.Services.AddEntityFramework(appSettings);
builder.Services.AddWebApiServices();
builder.Services.AddIdentityServices(appSettings);

// Add logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.AddSerilog();

var app = builder.Build();

// Seed the Database
await ConfigureServices.SeedDatabase(app);
await ConfigureServices.SeedUsers(app);

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
app.UseSwaggerUi3(settings => { settings.Path = "/swagger"; });

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

Log.CloseAndFlush();
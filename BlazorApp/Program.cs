using BlazorApp.Areas.Identity;
using BlazorApp.Data;
using Domain.Common;
using Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();
if (appSettings == null)
    throw new Exception("The InAppSettings section is empty. Please check your appsettings.json file.");

// Setup AppSettings for Dependency Injection
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllersWithViews();

// Add Infrastructure Services here
builder.Services.AddEntityFramework(appSettings);
builder.Services.AddWebApiServices();
builder.Services.AddIdentityServices(appSettings);

// Add Razor Pages and Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add an auth state provider
builder.Services
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

// Add Application services here
builder.Services.AddSingleton<WeatherForecastService>();

// Add logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// This is used for Docker health checks
app.UseHealthChecks("/health");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseOpenApi();
app.UseSwaggerUi3(settings => { settings.Path = "/swagger"; });

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
Log.CloseAndFlush();
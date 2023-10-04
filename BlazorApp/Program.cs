using System.Security.Claims;
using BlazorApp.Areas.Identity;
using BlazorApp.Data;
using Domain.Common;
using Infrastructure.Identity;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//
// var inAppSettings = new InAppSettings();
// builder.Configuration.GetSection("InAppSettings").Bind(inAppSettings);

var appSettings = builder.Configuration.Get<AppSettings>();
if (appSettings == null)
    throw new Exception("The InAppSettings section is empty. Please check your appsettings.json file.");

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.None;
    options.CheckConsentNeeded = _ => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
    })
    .AddOpenIdConnect(options =>
    {
        options.Authority = appSettings.Authentication?.KeyCloak?.Issuer;
        options.ClientId = appSettings.Authentication?.KeyCloak?.ClientId;
        options.ClientSecret = appSettings.Authentication?.KeyCloak?.ClientSecret;
        options.RequireHttpsMetadata = appSettings.Authentication!.KeyCloak!.RequireHttpsMetadata;
        options.ResponseType = appSettings.Authentication.KeyCloak.ResponseType!;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = false;
        options.MapInboundClaims = true;
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("roles");

        options.Events = new OpenIdConnectEvents
        {
            OnUserInformationReceived = async context =>
            {
                // Extract KeyCloak roles from claims
                var keyCloakRoles = context.Principal.FindAll("KeyCloakRoleClaimType").Select(c => c.Value).ToList();

                KeycloakHelpers.MapKeyCloakRolesToRoleClaims(context);


                // Extract email from claims
                var email = context.Principal.FindFirst(ClaimTypes.Email)?.Value;

                // Use UserManager and SignInManager services
                var userManager =
                    context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                var signInManager = context.HttpContext.RequestServices
                    .GetRequiredService<SignInManager<ApplicationUser>>();

                // Check if user already exists
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create user if they don't exist
                    user = new ApplicationUser(email);
                    user.Email = email;
                    user.EmailConfirmed = true;

                    await userManager.CreateAsync(user);

                    // Add KeyCloak roles or fallback roles
                    if (keyCloakRoles.Contains(appSettings.Authentication.KeyCloak.AdminRole ?? "admin"))
                    {
                        await userManager.AddToRoleAsync(user, "Admin"); // 'Admin' is an ASP.NET Identity role
                    }
                }

                // Sign the user in
                await signInManager.SignInAsync(user, isPersistent: false);
            },
            OnTicketReceived = context =>
            {
                // Redirect to home page after login
                context.ReturnUri = "/";
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProviderForSignOut = context =>
            {
                // Logout via KeyCloak
                var logoutUri = $"{appSettings.Authentication.KeyCloak.Issuer}/protocol/openid-connect/logout";
                context.ProtocolMessage.PostLogoutRedirectUri = logoutUri;
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
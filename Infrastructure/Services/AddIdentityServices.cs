using System.Security.Claims;
using Domain.Common;
using Infrastructure.Identity;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public static partial class ConfigureServices
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services,
        AppSettings settings)
    {
        // services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        //     {
        //         options.SignIn.RequireConfirmedAccount = false;
        //         options.User.RequireUniqueEmail = true;
        //     })
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddUserManager<ApplicationUserManager<ApplicationUser>>()
        //     .AddSignInManager<ApplicationSignInManager>()
        //     .AddDefaultTokenProviders();

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.Secure = CookieSecurePolicy.None;
            options.CheckConsentNeeded = _ => false;
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
        });

        services.AddAuthentication(options =>
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
                options.Authority = settings.Authentication?.KeyCloak?.Issuer;
                options.ClientId = settings.Authentication?.KeyCloak?.ClientId;
                options.ClientSecret = settings.Authentication?.KeyCloak?.ClientSecret;
                options.RequireHttpsMetadata = settings.Authentication!.KeyCloak!.RequireHttpsMetadata;
                options.ResponseType = settings.Authentication.KeyCloak.ResponseType!;
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
                        KeycloakHelpers.MapKeyCloakRolesToRoleClaims(context);

                        // Extract KeyCloak roles from claims
                        var keyCloakRoles = context.Principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                        // Extract email from claims
                        var email = context.Principal.FindFirst(ClaimTypes.Email)?.Value;

                        // Use UserManager and SignInManager services
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<ApplicationUser>>();
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
                            if (keyCloakRoles.Contains(settings.Authentication.KeyCloak.AdminRole ?? "admin"))
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
                        var logoutUri = $"{settings.Authentication.KeyCloak.Issuer}/protocol/openid-connect/logout";
                        context.ProtocolMessage.PostLogoutRedirectUri = logoutUri;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddWsFederation("wsfed", options =>
            {
                // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/ws-federation?view=aspnetcore-7.0#add-ws-federation-as-an-external-login-provider-for-aspnet-core-identity
                // MetadataAddress represents the Active Directory instance used to authenticate users.
                // options.MetadataAddress = "https://<ADFS FQDN or AAD tenant>/FederationMetadata/2007-06/FederationMetadata.xml";
                options.MetadataAddress = settings.Authentication?.WSFederation?.MetadataAddress;

                // Wtrealm is the app's identifier in the Active Directory instance.
                // For ADFS, use the relying party's identifier, its WS-Federation Passive protocol URL:
                // options.Wtrealm = "https://localhost:44307/";
                options.Wtrealm = settings.Authentication?.WSFederation?.Wtrealm;

                // For AAD, use the Application ID URI from the app registration's Overview blade:
                // options.Wtrealm = "api://bbd35166-7c13-49f3-8041-9551f2847b69";

                // ... other settings
            });
        
        // services.AddAuthorization();


        return services;
    }
}
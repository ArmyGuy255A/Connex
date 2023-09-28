using Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services;

public static partial class ConfigureServices
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services,
        InAppSettings settings)
    {
        services.AddAuthentication(options => { options.DefaultScheme = "cookie"; })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = settings.Authentication?.OIDC?.Authority;
                options.ClientId = settings.Authentication?.OIDC?.ClientId;
                options.ClientSecret = settings.Authentication?.OIDC?.ClientSecret;
                options.ResponseType = settings.Authentication?.OIDC?.ResponseType;
                options.RequireHttpsMetadata = false;
                // ... other settings
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

        return services;
    }
}
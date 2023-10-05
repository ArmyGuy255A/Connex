using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

// Credit: CSINISA
// 05 OCT 2023
// Source: https://github.com/csinisa/blazor_server_keycloak/blob/master/BlazorAuthSample/Startup.cs

namespace Infrastructure.Identity;

/// <summary>
/// Helper class for Keycloak related operations.
/// </summary>
public static class KeycloakHelpers
{
    /// <summary>
    /// Maps Keycloak roles to role claims.
    /// </summary>
    /// <param name="context">The user information received context.</param>
    public static void MapKeyCloakRolesToRoleClaims(UserInformationReceivedContext context)
    {
        // Exit if the principal's identity is not a ClaimsIdentity
        if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity)
        {
            return;
        }

        // Handle the "preferred_username" property
        MapPreferredUsername(context, claimsIdentity);

        // Handle the "realm_access" and its "roles" properties
        MapGlobalRoles(context, claimsIdentity);

        // Handle the "resource_access" and its roles specific to the client
        MapClientRoles(context, claimsIdentity);
    }

    private static void MapPreferredUsername(UserInformationReceivedContext context, ClaimsIdentity claimsIdentity)
    {
        if (context.User.RootElement.TryGetProperty("preferred_username", out var username))
        {
            string usernameValue = username.ToString();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, usernameValue));
        }
    }

    private static void MapGlobalRoles(UserInformationReceivedContext context, ClaimsIdentity claimsIdentity)
    {
        if (context.User.RootElement.TryGetProperty("realm_access", out var realmAccess) &&
            realmAccess.TryGetProperty("roles", out var globalRoles))
        {
            foreach (var role in globalRoles.EnumerateArray())
            {
                string roleValue = role.ToString();
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
            }
        }
    }

    private static void MapClientRoles(UserInformationReceivedContext context, ClaimsIdentity claimsIdentity)
    {
        if (context.User.RootElement.TryGetProperty("resource_access", out var clientAccess) &&
            clientAccess.TryGetProperty(context.Options.ClientId!, out var client) &&
            client.TryGetProperty("roles", out var clientRoles))
        {
            foreach (var role in clientRoles.EnumerateArray())
            {
                string roleValue = role.ToString();
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
            }
        }
    }
}

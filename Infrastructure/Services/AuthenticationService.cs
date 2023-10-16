using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AuthenticationService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    public event Action<ClaimsPrincipal>? UserChanged;
    private ClaimsPrincipal? currentUser;

    public ClaimsPrincipal CurrentUser
    {
        get { return currentUser ?? new(); }
        set
        {
            currentUser = value;

            if (UserChanged is not null)
            {
                UserChanged(currentUser);
            }
        }
    }

    public AuthenticationService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }
    
    public async Task Login(string userName, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "apiauth"));
            // authStateProvider.NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CurrentUser)));
        }
    }
}
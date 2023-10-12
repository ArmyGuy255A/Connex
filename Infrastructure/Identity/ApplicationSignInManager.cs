using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Infrastructure.Identity;

public class ApplicationSignInManager : SignInManager<ApplicationUser>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<SignInManager<ApplicationUser>> _logger;

    public ApplicationSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
        ILogger<ApplicationSignInManager> logger, IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation) : base(userManager, contextAccessor, claimsFactory,
        optionsAccessor, logger, schemes, confirmation)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }
    
    public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor)
        {
            var result = await base.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
            return result;
        }
        public override async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            var result = await ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, false);
            return result;
        }

        public override async Task SignInAsync(ApplicationUser user, bool isPersistent, string authenticationMethod = null)
        {
            await base.SignInAsync(user, isPersistent, authenticationMethod);
        }
        
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password,
            bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }
        
        public override async Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password,
            bool isPersistent, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);

            if (attempt.Succeeded)
            {
                await SignInAsync(user, true);
            }

            return attempt.Succeeded ? SignInResult.Success : SignInResult.Failed;
        }

        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            var isAuthenticated = principal.Identity.IsAuthenticated;
            if (null != principal.Identity.Name)
            {
                return true;
            }
            return base.IsSignedIn(principal);
        }

        public override async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
        {
            var info = await base.GetExternalLoginInfoAsync(expectedXsrf);

            if (info == null)
            {
                _logger.LogWarning($"Unable to get user information. Attempting to refetch the user.");
                var authProps = _contextAccessor.HttpContext.AuthenticateAsync();
                var authToken = _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                var id = Context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                if (null == id)
                {
                    _logger.LogWarning($"Unable to get external login info from the current HttpContext.");
                    return info;
                }

                //Build the info
                var authScheme = authProps.Result.Properties.Items["LoginProvider"];
                authScheme = string.IsNullOrEmpty(authScheme) ? authProps.Result.Properties.Items[".AuthScheme"] : authScheme;
                info = new ExternalLoginInfo(Context.User, authScheme, id.Value, authScheme);
                info.AuthenticationProperties = authProps.Result.Properties;
            }
            else if (string.IsNullOrEmpty(info.LoginProvider) && !string.IsNullOrEmpty(info.Principal.Identity.Name))
            {
                _logger.LogWarning("Attempting to rebuild the login provider and authentication scheme for {IdentityName}", info.Principal.Identity.Name);
                var authScheme = info.AuthenticationProperties.Items["LoginProvider"];
                authScheme = string.IsNullOrEmpty(authScheme) ? info.AuthenticationProperties.Items[".AuthScheme"] : authScheme;
                var newInfo = new ExternalLoginInfo(info.Principal, authScheme, info.ProviderKey, authScheme);
                newInfo.AuthenticationProperties = info.AuthenticationProperties;
                return newInfo;
            }

            return info;
        }
}
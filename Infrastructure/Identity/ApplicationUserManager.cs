using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;

public class ApplicationUserManager<TUser> : UserManager<TUser>
    where TUser : ApplicationUser
{
    private readonly ILogger<ApplicationUserManager<TUser>> _logger;

    public ApplicationUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<ApplicationUserManager<TUser>> logger) : base(store,
        optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _logger = logger;
    }
    
    public override async Task<TUser> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await base.GetUserAsync(principal);

            if (null == user)
            {
                if (null != principal.Identity.Name)
                {
                    user = await base.FindByEmailAsync(principal.Identity.Name);
                }
            }

            return user;
        }

        public override async Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (null == user)
            {
                return false;
            }

            return await base.IsInRoleAsync(user, role);
        }

        // public async Task<IdentityResult> SetDeploymentInitialsAsync(TUser user, string deploymentInitials)
        // {
        //     ThrowIfDisposed();
        //     if (null == user)
        //     {
        //         throw new ArgumentNullException("user");
        //     }
        //
        //     var users = base.Users.Where(u => u.DeploymentInitials.ToUpper() == deploymentInitials.ToUpper()).ToList();
        //     if (users.Count == 0)
        //     {
        //         user.DeploymentInitials = deploymentInitials.ToUpper();
        //     }
        //     else
        //     {
        //         _logger.LogWarning($"Could not set the deployment initials for {user.UserName}. The initials must be unique.");
        //     }
        //
        //     return await base.UpdateAsync(user);
        // }

        // public Task<string> GetDeploymentInitialsAsync(TUser user)
        // {
        //     ThrowIfDisposed();
        //     if (user == null)
        //     {
        //         throw new ArgumentNullException("user");
        //     }
        //     return Task.FromResult(user.DeploymentInitials);
        // }


        // private string GetUpdatedValue(string oldString, string newString)
        // {
        //     if (null == newString) return oldString;
        //     if (null == oldString) return newString;
        //     return oldString.Equals(newString) ? oldString : newString;
        // }

        public async Task<IdentityResult> UpdateUserInfoAsync(TUser user, ClaimsPrincipal principal)
        {
            return await UpdateUserInfoAsync(user, principal.Identities.FirstOrDefault());
        }
        public async Task<IdentityResult> UpdateUserInfoAsync(TUser user, ClaimsIdentity identity)
        {

            // // GraphUserData graphUserData = await GetGraphData(identity);
            // if (null == graphUserData) { return null; }
            //
            // if (UpdateLoginInformation(user))
            // {
            //     _logger.LogInformation($"Attempting to update user information for {user.UserName}");
            //     user.DisplayName = GetUpdatedValue(user.DisplayName, graphUserData.DisplayName);
            //     user.GivenName = GetUpdatedValue(user.GivenName, graphUserData.GivenName);
            //     user.Surname = GetUpdatedValue(user.Surname, graphUserData.Surname);
            // }
            return await base.UpdateAsync(user);
        }

        public async Task<bool> CreateAsync(ExternalLoginInfo info, ClaimsPrincipal principal)
        {
            return await CreateAsync(info, principal.Identities.FirstOrDefault());
        }
        public async Task<bool> CreateAsync(ExternalLoginInfo info, ClaimsIdentity identity)
        {
            var ErrorMessage = "";

            // GraphUserData graphUserData = await GetGraphData(identity);

            // if (null == graphUserData)
            // {
            //     _logger.LogError($"Unable to create user due to missing Microsoft Graph Data.");
            //     return true;
            // }
            //
            // var username = graphUserData.UserPrincipalName;
            // var email = graphUserData.Mail;
            // var displayName = graphUserData.DisplayName;
            // var givenName = graphUserData.GivenName;
            // var surName = graphUserData.Surname;
            //
            // if (string.IsNullOrEmpty(username)) username = email;
            // if (string.IsNullOrEmpty(email)) email = username;
            // if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
            // {
            //     ErrorMessage = $"User with ID {graphUserData.Id} is missing a username and email address. Please contact the administrator to resolve this issue.";
            //     _logger.LogError(ErrorMessage);
            //     return false; // RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            // }
            //
            // //var username = info.Principal.HasClaim(c => c.Type == ClaimTypes.) ? info.Principal.FindFirstValue(ClaimTypes.Email) : info.Principal.FindFirstValue("preferred_username");
            // var user = new ApplicationUser { UserName = username, Email = username, EmailConfirmed = true, LoginCount = 1, LastLoginDate = DateTime.UtcNow, DisplayName = displayName, GivenName = givenName, Surname = surName };
            //
            // //Ensure deployment Initials are unique
            // bool initialsValidated = false;
            // while (!initialsValidated)
            // {
            //     var initials = ApplicationUser.GenerateDeploymentInitials();
            //     var duplicateInitials = base.Users.Where(u => u.DeploymentInitials.ToUpper() == initials).ToList();
            //     if (duplicateInitials.Count == 0)
            //     {
            //         user.DeploymentInitials = initials;
            //         initialsValidated = true;
            //     }
            // }
            //
            //
            // _logger.LogInformation($"Attempting to create a new user account for {graphUserData.UserPrincipalName}");
            // var userResult = await CreateAsync((TUser)user);
            // if (userResult.Succeeded)
            // {
            //     userResult = await AddLoginAsync((TUser)user, info);
            //     if (userResult.Succeeded)
            //     {
            //         _logger.LogInformation($"Account created for: {graphUserData.UserPrincipalName} using: {info.LoginProvider} provider.");
            //
            //         //Update the user's information on first-time login
            //         await UpdateUserInfoAsync((TUser)user, info.Principal);
            //         return true; // RedirectToPage("/");
            //     }
            // }

            return false;
        }

        // public async Task<GraphUserData> GetGraphData(ClaimsIdentity identity)
        // {
        //     //Initialize the GraphServiceClient.
        //     _logger.LogDebug(String.Format("Getting Graph Data for identity.preferred_username: {0} identity.email: {1}", identity.GetPreferredUsername(), identity.GetEmail()));
        //     Microsoft.Graph.User graphUser;
        //     try
        //     {
        //         var token = await _tokenAcquisition.GetAuthenticationResultForUserAsync(new string[] { "user.read" });
        //         var graphServiceClient = new GraphServiceClient(baseUrl: "https://graph.microsoft.us/v1.0",
        //         new DelegateAuthenticationProvider((requestMessage) =>
        //         {
        //             requestMessage
        //             .Headers
        //             .Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        //
        //             return Task.CompletedTask;
        //         }));
        //
        //         graphUser = await graphServiceClient.Me.Request().GetAsync();
        //         var graphUserData = _mapper.Map(graphUser, new GraphUserData());
        //         return graphUserData;
        //     }
        //     catch (ServiceException e)
        //     {
        //         _logger.LogWarning($"An error occurred accessing Microsoft Graph data for user: {identity.GetPreferredUsername()}");
        //         _logger.LogError($"Microsoft Graph Error Message: {e.Message}");
        //         _logger.LogError($"Microsoft Graph Error InnerException Message: {e.InnerException.Message}");
        //     }
        //     return null;
        // }

        public bool UpdateLoginInformation(TUser user)
        {
            if (user.LastLoginDate.AddSeconds(30) < DateTime.UtcNow)
            {
                _logger.LogInformation("Updating login count and timestamp for {UserUserName}", user.UserName);
                user.LoginCount += 1;
                user.LastLoginDate = DateTime.UtcNow;
                return true;
            }
            else
            {
                return false;
            }

        }
}
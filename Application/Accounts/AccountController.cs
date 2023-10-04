// using AutoMapper;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.OpenIdConnect;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.UI.Services;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Microsoft.Identity.Web;
// using RepositoryService.Identity;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Threading.Tasks;
//
// namespace CoreUI.Mvc.Controllers
// {
//     [Route("[controller]/[action]")]
//     [ApiExplorerSettings(IgnoreApi = true)]
//     [AuthorizeForScopes(Scopes = new[] { "user.read" })]
//     public class AccountController : Controller
//     {
//         private readonly GraphSignInManager _signInManager;
//         private readonly GraphUserManager<ApplicationUser> _userManager;
//         private readonly IEmailSender _emailSender;
//         private readonly ILogger<AccountController> _logger;
//         private readonly IMapper _mapper;
//
//         public AccountController(
//             GraphSignInManager signInManager,
//             GraphUserManager<ApplicationUser> userManager,
//             ILogger<AccountController> logger,
//             IMapper mapper,
//             IEmailSender emailSender)
//         {
//             _signInManager = signInManager;
//             _userManager = userManager;
//             _logger = logger;
//             _mapper = mapper;
//             _emailSender = emailSender;
//         }
//
//         public string ProviderDisplayName { get; set; }
//
//         public string ReturnUrl { get; set; }
//
//         [TempData]
//         public string ErrorMessage { get; set; }
//
//         [HttpGet]
//         public IActionResult SignIn(string provider = "OpenIdConnect", string returnUrl = null)
//         {
//             // Request a redirect to the external login provider.
//             var redirectUrl = "/Account/Callback";
//             var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
//             return new ChallengeResult(provider, properties);
//         }
//
//         [HttpGet]
//         public async Task<IActionResult> Callback(string returnUrl = "/", string remoteError = null)
//         {
//             returnUrl = returnUrl ?? Url.Content("~/");
//             if (remoteError != null)
//             {
//                 ErrorMessage = $"Error from external provider: {remoteError}";
//                 _logger.LogError(ErrorMessage);
//                 return RedirectToPage("/Login", new { ReturnUrl = returnUrl });
//             }
//             var info = await _signInManager.GetExternalLoginInfoAsync();
//             if (info == null)
//             {
//                 //var redirectUrl = Url.Page("/", pageHandler: "Callback", values: new { returnUrl });
//                 //var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
//                 //var challengeResult =  new ChallengeResult(provider, properties);
//
//                 ErrorMessage = "Error loading external login information.";
//                 _logger.LogError(ErrorMessage);
//                 return RedirectToPage("/Login", new { ReturnUrl = returnUrl });
//             }
//
//
//             // Sign in the user with this external login provider if the user already has a login.
//             var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
//             if (result.Succeeded)
//             {
//                 _logger.LogInformation($"{info.Principal.Identity.Name} signed in with {info.LoginProvider} provider.");
//
//                 var user = await _userManager.GetUserAsync(info.Principal);
//                 //var graphUser = await GetGraphUser();
//
//                 if (null == user)
//                 {
//                     // Create a new user account
//                     var redirect = await _userManager.CreateAsync(info, info.Principal);
//                     if (redirect)
//                     {
//                         return RedirectToPage("/Login", new { ReturnUrl = returnUrl });
//                     }
//                 }
//                 else
//                 {
//                     //Sign in the user
//                     _logger.LogInformation($"Attempting to sign in {info.Principal.Identity.Name} locally.");
//                     await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
//
//                     //Update the user info if necessary
//                     await _userManager.UpdateUserInfoAsync(user, info.Principal);
//                 }
//                 return LocalRedirect(returnUrl);
//             }
//             else if (result.IsLockedOut)
//             {
//                 _logger.LogWarning($"{info.Principal.Identity.Name} is currently locked out. Redirecting to /Lockout");
//                 return RedirectToPage("/Lockout");
//             }
//             else
//             {
//                 _logger.LogInformation($"{info.Principal.Identity.Name} could not sign in. Attempting to create a local account.");
//                 // If the user does not have an account, then automatically create an account for the user.
//                 ReturnUrl = returnUrl;
//                 ProviderDisplayName = info.ProviderDisplayName;
//                 //var graphUser = await GetGraphUser();
//
//                 var redirect = await _userManager.CreateAsync(info, info.Principal);
//                 if (redirect)
//                 {
//                     return RedirectToPage("/");
//                 }
//
//
//             }
//             //This redirects to the Email registration page.
//
//             return LocalRedirect(ReturnUrl);
//         }
//
//         [HttpGet]
//         public IActionResult SignOut()
//         {
//             var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
//             return SignOut(
//                 new AuthenticationProperties { RedirectUri = callbackUrl },
//                 CookieAuthenticationDefaults.AuthenticationScheme,
//                 OpenIdConnectDefaults.AuthenticationScheme);
//         }
//
//         [HttpGet]
//         public IActionResult SignedOut()
//         {
//             if (User.Identity.IsAuthenticated)
//             {
//                 // Redirect to home page if the user is authenticated.
//                 return RedirectToAction(nameof(HomeController.Index), "Home");
//             }
//
//             return View();
//         }
//
//         [HttpGet]
//         public IActionResult AccessDenied()
//         {
//             return View();
//         }
//
//         [HttpGet]
//         public async Task<IActionResult> Profile(string id)
//         {
//             ApplicationUser user;
//             if (string.IsNullOrEmpty(id))
//             {
//                 user = await _userManager.GetUserAsync(this.User);
//                 ViewData["Title"] = "My Profile";
//             } else
//             {
//                 user = await _userManager.FindByIdAsync(id);
//                 ViewData["Title"] = $"{user.GivenName} {user.Surname}'s Profile";
//
//             }
//             var viewModel = _mapper.Map<ApplicationUserDTO>(user);
//
//             return View(viewModel);
//         }
//
//         [HttpPost]
//         public async Task<IActionResult> Profile([Bind("Id,DisplayName,FirstName,LastName,DeploymentInitials")] ApplicationUserDTO applicationUserDTO)
//         {
//             if (string.IsNullOrEmpty(applicationUserDTO.DeploymentInitials))
//             {
//                 ShowProfileError("DeploymentInitials", "Deployment Initials must not be empty.");
//             } else if (applicationUserDTO.DeploymentInitials.Length <= 1 || applicationUserDTO.DeploymentInitials.Length > 3)
//             {
//                 ShowProfileError("DeploymentInitials", "Deployment Initials must be between 2-3 characters.");
//             } else
//             {
//                 applicationUserDTO.DeploymentInitials = applicationUserDTO.DeploymentInitials.ToUpper();
//             }
//
//             if (ModelState.IsValid)
//             {
//
//                 var user = await _userManager.GetUserAsync(this.User);
//                 if (user == null)
//                 {
//                     ShowProfileError("Not Found Exception", "Unable to locate the current user.");
//                 }
//
//                 if (user.Id != applicationUserDTO.Id || !(await _userManager.IsInRoleAsync(user, "Administrator")))
//                 {
//                     ShowProfileError ("Permission Denied", "You can't edit another user's profile.");
//                 }
//
//                 //Check for duplicate initials
//                 var duplicateUserInitials = _userManager.Users.Where(u => u.DeploymentInitials == applicationUserDTO.DeploymentInitials.ToUpper()).ToList();
//                 if (duplicateUserInitials.Count == 0)
//                 {
//                     _mapper.Map(applicationUserDTO, user);
//                     var saved = await _userManager.UpdateAsync(user);
//                 }
//
//                 
//                 try
//                 {
//                     await _userManager.UpdateAsync(user);
//                 }
//                 catch (System.Exception)
//                 {
//                     ShowProfileError("General Error", "Unable to update user information.");
//                     //throw;
//                 }
//             }
//             return RedirectToAction(nameof(Profile));
//         }
//
//         private void ShowProfileError(string key, string message)
//         {
//             _logger.LogInformation(key, message);
//             ModelState.AddModelError(key, message);
//         }
//
//     }
// }

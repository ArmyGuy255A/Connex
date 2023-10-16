using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// Authenticate the user.
        /// </summary>
        /// <param name="model">The login model containing username and password.</param>
        /// <returns>A response indicating the authentication result.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Login succeeded" });
                }
                else
                {
                    return Unauthorized(new { Message = "Invalid username or password" });
                }
            }
            return BadRequest(new { Message = "Invalid data" });
        }
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
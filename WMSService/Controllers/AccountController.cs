using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WMSService.Models;

namespace WMSService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> _userManager,
                                 SignInManager<IdentityUser> _signInManager)
        {
            userManager = _userManager;
            signInManager = _signInManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] JsonElement userJson)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(userJson);
            UserCredentials userCredentials = JsonConvert.DeserializeObject<UserCredentials>(json);

            if (string.IsNullOrEmpty(userCredentials.Email))
                return BadRequest("Email is mandatory");

            if (string.IsNullOrEmpty(userCredentials.FirstName))
                return BadRequest("First name is mandatory");

            if (string.IsNullOrEmpty(userCredentials.LastName))
                return BadRequest("Last name name is mandatory");

            if (string.IsNullOrEmpty(userCredentials.Password))
                return BadRequest("Password is mandatory");

            IdentityUser user = new IdentityUser
            {
                Email = userCredentials.Email,
                UserName = userCredentials.Email,
                NormalizedUserName = $"{userCredentials.FirstName} {userCredentials.LastName}"
            };

            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
                return Ok("User registered");
            else
            {
                string details = string.Empty;

                foreach(var error in result.Errors)
                {
                    if (string.IsNullOrEmpty(details))
                        details = error.Description;
                    else
                        details += $" {error.Description}";
                }

                return Problem(details);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] JsonElement userJson)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(userJson);
            UserCredentials userCredentials = JsonConvert.DeserializeObject<UserCredentials>(json);

            if (string.IsNullOrEmpty(userCredentials.Email))
                return BadRequest("Email is mandatory");

            if (string.IsNullOrEmpty(userCredentials.Password))
                return BadRequest("Password is mandatory");

            var result = await signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, userCredentials.RememberMe, false);

            if (result.Succeeded)
                return Ok("User logged in");
            else
            {
                string details = "Wrong username or password.";
                return Problem(details);
            }

        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
            }
            catch(Exception e)
            {
                return Problem(e.Message);
            }

            return Ok("User logged out.");
        }
    }
}

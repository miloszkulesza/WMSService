using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WMSService.Dapper.Abstract;
using WMSService.Dapper.Models;
using WMSService.Models;

namespace WMSService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IUserProfilesTable userProfilesTable;

        public AccountController(UserManager<IdentityUser> _userManager,
                                 SignInManager<IdentityUser> _signInManager,
                                 IUserProfilesTable _userProfilesTable)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            userProfilesTable = _userProfilesTable;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] JsonElement userJson)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(userJson);
            RegisterUserRequest userRequest = JsonConvert.DeserializeObject<RegisterUserRequest>(json);

            if (string.IsNullOrEmpty(userRequest.Email))
                return BadRequest("Email is mandatory");

            if (string.IsNullOrEmpty(userRequest.FirstName))
                return BadRequest("First name is mandatory");

            if (string.IsNullOrEmpty(userRequest.LastName))
                return BadRequest("Last name is mandatory");

            if (string.IsNullOrEmpty(userRequest.Password))
                return BadRequest("Password is mandatory");

            IdentityUser user = new IdentityUser
            {
                Email = userRequest.Email,
                UserName = userRequest.Email
            };

            var result = await userManager.CreateAsync(user, userRequest.Password);

            if (result.Succeeded)
            {
                UserProfile userProfile = new UserProfile
                {
                    FirstName = userRequest.FirstName,
                    LastName = userRequest.LastName,
                    UserId = user.Id
                };

                var userProfileCreateResult = await userProfilesTable.CreateAsync(userProfile);

                if (userProfileCreateResult)
                    return Ok("User registered");
                else
                    return Problem("While creating user profile occurred errors. User is created, but you need to update your profile.");
            }                
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
            LoginUserRequest userRequest = JsonConvert.DeserializeObject<LoginUserRequest>(json);

            if (string.IsNullOrEmpty(userRequest.Email))
                return BadRequest("Email is mandatory");

            if (string.IsNullOrEmpty(userRequest.Password))
                return BadRequest("Password is mandatory");

            var result = await signInManager.PasswordSignInAsync(userRequest.Email, userRequest.Password, false, false);

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

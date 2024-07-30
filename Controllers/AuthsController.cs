using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryAPI6.Models;
using LibraryAPI6.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthsController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // POST: api/Auths/Login
        // Authenticate the user and return a JWT token
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            // Find the user by username
            ApplicationUser applicationUser = await _userManager.FindByNameAsync(userName);

            // Check if the user exists and the password is correct
            if (applicationUser != null && await _userManager.CheckPasswordAsync(applicationUser, password))
            {
                // Generate a token for the authenticated user
                var token = await _tokenService.GenerateToken(applicationUser);
                return Ok(new { token });
            }
            return Unauthorized(); // Return 401 if authentication fails
        }

        // GET: api/Auths/ForgetPassword
        // Generate a password reset token for the user
        [HttpGet("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            // Find the user by username
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;

            // Generate a password reset token
            string token = _userManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
            return token; // Return the reset token
        }

        // GET: api/Auths/ResetPassword
        // Reset the user's password using the reset token
        [HttpGet("ResetPassword")]
        public ActionResult ResetPassword(string userName, string token, string newPassword)
        {
            // Find the user by username
            ApplicationUser applicationUser = _userManager.FindByNameAsync(userName).Result;

            // Reset the user's password using the provided token and new password
            _userManager.ResetPasswordAsync(applicationUser, token, newPassword).Wait();

            return Ok(); // Return 200 OK if the password is successfully reset
        }
    }
}

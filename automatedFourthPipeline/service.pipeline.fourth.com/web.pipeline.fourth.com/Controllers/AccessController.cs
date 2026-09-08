using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    [EnableRateLimiting("site-login")]
    public class AccessController : Controller
    {
        private readonly StaticAdminOptions _adminOptions;
        private readonly UserManager<IdentityUser> _users;

        public AccessController(IOptions<StaticAdminOptions> adminOptions, UserManager<IdentityUser> users)
        {
            _adminOptions = adminOptions.Value;
            _users = users;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(GetSafeReturnUrl(returnUrl));
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new StaticAdminLoginInputModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(StaticAdminLoginInputModel input, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(input);
            }

            Claim[] claims;
            if (CredentialsAreValid(input.Username, input.Password))
            {
                claims = new[] { new Claim(ClaimTypes.Name, _adminOptions.Username), new Claim(ClaimTypes.Role, "Administrator") };
            }
            else
            {
                var username = input.Username?.Trim();
                var user = string.IsNullOrWhiteSpace(username) ? null : await _users.FindByEmailAsync(username);
                if (user == null || !user.EmailConfirmed || !await _users.CheckPasswordAsync(user, input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View(input);
                }
                claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Name, user.Email ?? user.UserName), new Claim(ClaimTypes.Email, user.Email ?? string.Empty) };
            }
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = input.RememberMe });

            return LocalRedirect(GetSafeReturnUrl(returnUrl));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (User.IsInRole("Administrator")) return BadRequest("The platform administrator password is managed in server configuration.");
            return View(new ChangePasswordInputModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel input)
        {
            if (User.IsInRole("Administrator")) return BadRequest("The platform administrator password is managed in server configuration.");
            if (!ModelState.IsValid) return View(input);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = string.IsNullOrWhiteSpace(userId) ? null : await _users.FindByIdAsync(userId);
            if (user == null) return Challenge();
            var result = await _users.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return View(input);
            }
            TempData["Success"] = "Your password has been changed.";
            return RedirectToAction("Index", "ClientSetup");
        }

        private string GetSafeReturnUrl(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "ClientSetup");
        }

        private bool CredentialsAreValid(string username, string password)
        {
            if (!string.Equals(username, _adminOptions.Username, StringComparison.Ordinal) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrWhiteSpace(_adminOptions.PasswordHash))
            {
                return false;
            }

            try
            {
                var expectedHash = Convert.FromHexString(_adminOptions.PasswordHash);
                var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
                return CryptographicOperations.FixedTimeEquals(passwordHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

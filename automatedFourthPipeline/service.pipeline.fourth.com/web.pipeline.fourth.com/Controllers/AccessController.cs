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
using Microsoft.Extensions.Options;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    [EnableRateLimiting("site-login")]
    public class AccessController : Controller
    {
        private readonly StaticAdminOptions _adminOptions;

        public AccessController(IOptions<StaticAdminOptions> adminOptions)
        {
            _adminOptions = adminOptions.Value;
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
            if (!ModelState.IsValid || !CredentialsAreValid(input.Username, input.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(input);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, _adminOptions.Username),
                new Claim(ClaimTypes.Role, "Administrator")
            };
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

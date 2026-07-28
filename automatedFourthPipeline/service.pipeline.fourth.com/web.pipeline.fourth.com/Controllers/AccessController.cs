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
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    [EnableRateLimiting("site-login")]
    public class AccessController : Controller
    {
        private const string AdminUsername = "admin";
        private static readonly byte[] AdminPasswordHash = Convert.FromHexString(
            "02f6bbda1cf2e39d5c674d62b14734490783b9654bce5babe028eee954929725");

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
                new Claim(ClaimTypes.Name, AdminUsername),
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

        private static bool CredentialsAreValid(string username, string password)
        {
            if (!string.Equals(username, AdminUsername, StringComparison.Ordinal) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(passwordHash, AdminPasswordHash);
        }
    }
}

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web.pipeline.fourth.com.Models;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CustomerOnboardingInvitesController : Controller
    {
        private static readonly string[] Words = { "amber", "apple", "birch", "blue", "copper", "coral", "cedar", "dawn", "fern", "gold", "harbour", "hazel", "indigo", "juniper", "maple", "meadow", "mint", "oak", "olive", "orange", "orbit", "pearl", "pine", "river", "sage", "silver", "spruce", "stone", "sunset", "willow", "winter", "yellow" };
        private readonly FourthPipelineContext _context;
        public CustomerOnboardingInvitesController(FourthPipelineContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.CustomerOnboardingInvites.OrderByDescending(x => x.WhenCreatedUTC).ToListAsync());

        [HttpGet]
        public IActionResult Create() => View(new CustomerOnboardingInviteInputModel { ExpiresAtUTC = DateTime.UtcNow.AddDays(14), MaxUses = 1 });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerOnboardingInviteInputModel input)
        {
            input.CustomerName = input.CustomerName?.Trim();
            input.Phrase = input.Phrase?.Trim();
            input.Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim().ToLowerInvariant();
            if (input.ExpiresAtUTC.HasValue && input.ExpiresAtUTC <= DateTime.UtcNow) ModelState.AddModelError(nameof(input.ExpiresAtUTC), "Expiry must be in the future.");
            if (!ModelState.IsValid) return View(input);

            var phrase = string.IsNullOrWhiteSpace(input.Phrase) ? CreatePhrase() : NormalizePhrase(input.Phrase);
            var hash = Hash(phrase);
            if (await _context.CustomerOnboardingInvites.AnyAsync(x => x.KeyHash == hash))
            {
                ModelState.AddModelError(nameof(input.Phrase), "That phrase is already in use. Choose another or leave it blank to generate one.");
                return View(input);
            }
            var now = DateTime.UtcNow;
            _context.CustomerOnboardingInvites.Add(new CustomerOnboardingInvite
            {
                CustomerName = string.IsNullOrWhiteSpace(input.CustomerName) ? "Unassigned customer" : input.CustomerName, Email = input.Email, KeyHash = hash, Active = true,
                MaxUses = input.MaxUses, ExpiresAtUTC = input.ExpiresAtUTC, WhenCreatedUTC = now, WhenUpdatedUTC = now
            });
            await _context.SaveChangesAsync();
            return View("Created", new CustomerOnboardingInviteCreatedViewModel { CustomerName = input.CustomerName, Email = input.Email, Phrase = phrase, ExpiresAtUTC = input.ExpiresAtUTC, MaxUses = input.MaxUses });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Revoke(int id)
        {
            var invite = await _context.CustomerOnboardingInvites.FindAsync(id);
            if (invite == null) return NotFound();
            invite.Active = false;
            invite.WhenUpdatedUTC = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Customer invitation phrase revoked.";
            return RedirectToAction(nameof(Index));
        }

        private static string CreatePhrase()
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var words = Enumerable.Range(0, 4).Select(i => Words[bytes[i] % Words.Length]);
            var number = BitConverter.ToUInt32(bytes, 4) % 90000 + 10000;
            return string.Join("-", words) + "-" + number;
        }
        internal static string NormalizePhrase(string phrase) => string.Join(" ", phrase.Trim().ToLowerInvariant().Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
        internal static string Hash(string phrase) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(NormalizePhrase(phrase)))).ToLowerInvariant();
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web.pipeline.fourth.com.Models;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class SquareOAuthApplicationsController : Controller
    {
        private readonly FourthPipelineContext _context;
        private readonly SquareOAuthConfigurationService _configurationService;

        public SquareOAuthApplicationsController(
            FourthPipelineContext context,
            SquareOAuthConfigurationService configurationService)
        {
            _context = context;
            _configurationService = configurationService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SquareOAuthApplications
                .OrderBy(x => x.Environment)
                .ThenBy(x => x.Name)
                .ToListAsync());
        }

        public IActionResult Create()
        {
            PopulateEnvironmentList(SquareOAuthEnvironment.Sandbox);
            return View(new SquareOAuthApplicationInputModel
            {
                Environment = SquareOAuthEnvironment.Sandbox,
                RedirectUri = "https://squaretofourth.store/oauthredirect/accept",
                Active = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SquareOAuthApplicationInputModel input)
        {
            ValidateEnvironment(input.Environment);
            if (string.IsNullOrWhiteSpace(input.ClientSecret))
            {
                ModelState.AddModelError(nameof(input.ClientSecret), "The Square application secret is required.");
            }

            var application = Map(input, new SquareOAuthApplication());
            if (!_configurationService.TryValidate(application, out _, out var validationError))
            {
                ModelState.AddModelError(string.Empty, validationError);
            }
            if (await _context.SquareOAuthApplications.AnyAsync(x =>
                    x.Environment == application.Environment &&
                    x.ApplicationId == application.ApplicationId))
            {
                ModelState.AddModelError(nameof(input.ApplicationId), "That Square application already exists in this environment.");
            }

            if (ModelState.IsValid)
            {
                application.WhenCreatedUTC = DateTime.UtcNow;
                application.WhenUpdatedUTC = application.WhenCreatedUTC;
                _context.Add(application);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Square OAuth application added.";
                return RedirectToAction(nameof(Index));
            }

            PopulateEnvironmentList(input.Environment);
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var application = await _context.SquareOAuthApplications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            PopulateEnvironmentList(application.Environment);
            return View(new SquareOAuthApplicationInputModel
            {
                Id = application.Id,
                Name = application.Name,
                Environment = application.Environment,
                ApplicationId = application.ApplicationId,
                RedirectUri = application.RedirectUri,
                Active = application.Active
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SquareOAuthApplicationInputModel input)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }

            var application = await _context.SquareOAuthApplications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            ValidateEnvironment(input.Environment);
            var existingSecret = application.ClientSecret;
            Map(input, application);
            if (string.IsNullOrWhiteSpace(input.ClientSecret))
            {
                application.ClientSecret = existingSecret;
            }

            if (!_configurationService.TryValidate(application, out _, out var validationError))
            {
                ModelState.AddModelError(string.Empty, validationError);
            }
            if (await _context.SquareOAuthApplications.AnyAsync(x =>
                    x.Id != application.Id &&
                    x.Environment == application.Environment &&
                    x.ApplicationId == application.ApplicationId))
            {
                ModelState.AddModelError(nameof(input.ApplicationId), "That Square application already exists in this environment.");
            }

            if (ModelState.IsValid)
            {
                application.WhenUpdatedUTC = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Square OAuth application updated.";
                return RedirectToAction(nameof(Index));
            }

            PopulateEnvironmentList(input.Environment);
            return View(input);
        }

        private static SquareOAuthApplication Map(
            SquareOAuthApplicationInputModel input,
            SquareOAuthApplication application)
        {
            application.Name = input.Name?.Trim();
            application.Environment = SquareOAuthEnvironment.Normalize(input.Environment);
            application.ApplicationId = input.ApplicationId?.Trim();
            application.ClientSecret = input.ClientSecret?.Trim();
            application.RedirectUri = input.RedirectUri?.Trim();
            application.Active = input.Active;
            return application;
        }

        private void PopulateEnvironmentList(string selectedEnvironment)
        {
            ViewData["EnvironmentList"] = new SelectList(new[]
            {
                SquareOAuthEnvironment.Sandbox,
                SquareOAuthEnvironment.Production
            }, selectedEnvironment);
        }

        private void ValidateEnvironment(string environment)
        {
            if (environment != SquareOAuthEnvironment.Sandbox &&
                environment != SquareOAuthEnvironment.Production)
            {
                ModelState.AddModelError(nameof(SquareOAuthApplicationInputModel.Environment), "Select Sandbox or Production.");
            }
        }
    }
}

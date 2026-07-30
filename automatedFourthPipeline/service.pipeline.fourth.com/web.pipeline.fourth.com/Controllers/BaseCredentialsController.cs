using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data.pipeline.fourth.com.Models.Credentials;
using domain.pipeline.fourth.com.Models;
using Square;
using Microsoft.AspNetCore.Authorization;
using com.fourth.pipeline.pos.Services.SalesApi;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class BaseCredentialsController : Controller
    {
        private readonly FourthPipelineContext _context;
        private readonly SquareCredentialService _squareCredentialService;

        public BaseCredentialsController(
            FourthPipelineContext context,
            SquareCredentialService squareCredentialService)
        {
            _context = context;
            _squareCredentialService = squareCredentialService;
        }

        [HttpGet]
        public async Task<IActionResult> TestCreds(int id)
        {
            var baseCredential = await _context.CredentialsPool
               .Include(b => b.Brand)
               .Include(b => b.Store)
               .FirstOrDefaultAsync(m => m.Id == id);

            if (baseCredential == null)
            {
                return NotFound();
            }

            //try and do something with these creds depending on type
            var result = false;
            switch (baseCredential.CredentialType)
            {
                case shared.pipeline.fouth.com.Enums.CredentialTypes.None:
                    break;
                case shared.pipeline.fouth.com.Enums.CredentialTypes.RevelApi:
                    break;
                case shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi:
                    result = await TestSquareLocationsConnectivity(baseCredential);
                    break;
                case shared.pipeline.fouth.com.Enums.CredentialTypes.FourthBaseCredential:
                    result = await TestFourthConnectivity(baseCredential);
                    break;
                default:
                    break;
            }

            if (result)
            {
                return Ok(new { message = "Success" });
            }

            return BadRequest(new { message = "It didn't work. Those creds are not valid" });
        }

        private async Task<bool> TestFourthConnectivity(BaseCredential baseCredential)
        {
            try
            {
                var service = CreateFourthApiService(baseCredential);
                await service.Login();
                if (!service.IsLoggedIn())
                {
                    return false;
                }

                baseCredential.LatestAccessToken = service.AccessToken;
                if (!string.IsNullOrWhiteSpace(service.RefreshToken))
                {
                    baseCredential.RefreshToken = service.RefreshToken;
                }

                baseCredential.WhenUpdatedUTC = DateTime.UtcNow;
                _context.Update(baseCredential);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TestSquareLocationsConnectivity(BaseCredential baseCredential)
        {
            try
            {
                var squareClient = await _squareCredentialService.CreateClientAsync(baseCredential);
                var locationsResponse = await squareClient.Locations.ListAsync();
                var locations = locationsResponse.Locations?.ToList() ?? new List<Location>();
                if (locations.Count > 0)
                    return true;
            }
            catch
            {
                return false;
            }

            return false;
        }

        // GET: BaseCredentials
        public async Task<IActionResult> Index()
        {
            var fourthPipelineContext = _context.CredentialsPool.Include(b => b.Brand).Include(b => b.Store);
            return View(await fourthPipelineContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var baseCredential = await _context.CredentialsPool
                .Include(b => b.Brand)
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baseCredential == null) return NotFound();
            return View(baseCredential);
        }

        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name");
            PopulateCredentialTypeList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,ClientId,ClientSecret,LatestAccessToken,RefreshToken,BaseEndpoint,Password,KeySecret,SupplimentalData1,SupplimentalData2,CredentialType,StoreId,BrandId,Active")] BaseCredential baseCredential)
        {
            if (baseCredential.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
            {
                ModelState.AddModelError(nameof(baseCredential.CredentialType),
                    "Square credentials are created and renewed through Client Setup and the Square OAuth flow.");
            }

            if (ModelState.IsValid)
            {
                AssignCorrectStoreOrBrandNullable(baseCredential);
                baseCredential.WhenCreatedUTC = DateTime.UtcNow;
                baseCredential.WhenUpdatedUTC = DateTime.UtcNow;
                _context.Add(baseCredential);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", baseCredential.BrandId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", baseCredential.StoreId);
            PopulateCredentialTypeList(baseCredential.CredentialType);
            return View(baseCredential);
        }

        private static void AssignCorrectStoreOrBrandNullable(BaseCredential baseCredential)
        {
            if (baseCredential.BrandId != null)
            {
                baseCredential.Store = null;
                baseCredential.StoreId = null;
            }
            else
            {
                baseCredential.Brand = null;
                baseCredential.BrandId = null;
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var baseCredential = await _context.CredentialsPool.FindAsync(id);
            if (baseCredential == null) return NotFound();
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", baseCredential.BrandId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", baseCredential.StoreId);
            PopulateCredentialTypeList(baseCredential.CredentialType);
            return View(baseCredential);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,ClientId,ClientSecret,LatestAccessToken,RefreshToken,BaseEndpoint,Password,KeySecret,SupplimentalData1,SupplimentalData2,CredentialType,StoreId,BrandId,Active")] BaseCredential baseCredential)
        {
            if (id != baseCredential.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCredential = await _context.CredentialsPool.FindAsync(id);
                    if (existingCredential == null) return NotFound();

                    if (existingCredential.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
                    {
                        // OAuth owns these values. Allow an operator to disable the connection, but never overwrite its tokens from a generic form.
                        existingCredential.Active = baseCredential.Active;
                        existingCredential.WhenUpdatedUTC = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                    if (baseCredential.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
                    {
                        ModelState.AddModelError(nameof(baseCredential.CredentialType),
                            "Square credentials must be created through Client Setup and the Square OAuth flow.");
                        ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", baseCredential.BrandId);
                        ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", baseCredential.StoreId);
                        PopulateCredentialTypeList(baseCredential.CredentialType);
                        return View(baseCredential);
                    }

                    existingCredential.Username = baseCredential.Username;
                    existingCredential.ClientId = baseCredential.ClientId;
                    existingCredential.ClientSecret = string.IsNullOrWhiteSpace(baseCredential.ClientSecret)
                        ? existingCredential.ClientSecret : baseCredential.ClientSecret;
                    existingCredential.LatestAccessToken = string.IsNullOrWhiteSpace(baseCredential.LatestAccessToken)
                        ? existingCredential.LatestAccessToken : baseCredential.LatestAccessToken;
                    existingCredential.RefreshToken = string.IsNullOrWhiteSpace(baseCredential.RefreshToken)
                        ? existingCredential.RefreshToken : baseCredential.RefreshToken;
                    existingCredential.BaseEndpoint = baseCredential.BaseEndpoint;
                    existingCredential.Password = string.IsNullOrWhiteSpace(baseCredential.Password)
                        ? existingCredential.Password : baseCredential.Password;
                    existingCredential.KeySecret = string.IsNullOrWhiteSpace(baseCredential.KeySecret)
                        ? existingCredential.KeySecret : baseCredential.KeySecret;
                    existingCredential.SupplimentalData1 = string.IsNullOrWhiteSpace(baseCredential.SupplimentalData1)
                        ? existingCredential.SupplimentalData1 : baseCredential.SupplimentalData1;
                    existingCredential.SupplimentalData2 = string.IsNullOrWhiteSpace(baseCredential.SupplimentalData2)
                        ? existingCredential.SupplimentalData2 : baseCredential.SupplimentalData2;
                    existingCredential.CredentialType = baseCredential.CredentialType;
                    existingCredential.StoreId = baseCredential.StoreId;
                    existingCredential.BrandId = baseCredential.BrandId;
                    existingCredential.Active = baseCredential.Active;
                    existingCredential.WhenUpdatedUTC = DateTime.UtcNow;

                    AssignCorrectStoreOrBrandNullable(existingCredential);
                    _context.Update(existingCredential);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaseCredentialExists(baseCredential.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", baseCredential.BrandId);
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Name", baseCredential.StoreId);
            PopulateCredentialTypeList(baseCredential.CredentialType);
            return View(baseCredential);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var baseCredential = await _context.CredentialsPool
                .Include(b => b.Brand)
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baseCredential == null) return NotFound();
            return View(baseCredential);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baseCredential = await _context.CredentialsPool.FindAsync(id);
            if (baseCredential == null) return NotFound();
            _context.CredentialsPool.Remove(baseCredential);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaseCredentialExists(int id)
        {
            return _context.CredentialsPool.Any(e => e.Id == id);
        }

        private void PopulateCredentialTypeList(shared.pipeline.fouth.com.Enums.CredentialTypes? selectedValue = null)
        {
            var credentialTypes = Enum.GetValues(typeof(shared.pipeline.fouth.com.Enums.CredentialTypes))
                .Cast<shared.pipeline.fouth.com.Enums.CredentialTypes>()
                .Where(x => x != shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
                .ToList();
            ViewData["CredentialType"] = new SelectList(credentialTypes, selectedValue);
        }

        private static FourthApiService CreateFourthApiService(BaseCredential credential)
        {
            if (!string.IsNullOrWhiteSpace(credential.ClientId) ||
                !string.IsNullOrWhiteSpace(credential.ClientSecret))
            {
                return new FourthApiService(
                    credential.Username,
                    credential.Password,
                    credential.BaseEndpoint,
                    credential.ClientId,
                    credential.ClientSecret,
                    credential.SupplimentalData2,
                    credential.SupplimentalData1);
            }

            return new FourthApiService(
                credential.Username,
                credential.Password,
                credential.BaseEndpoint);
        }
    }
}

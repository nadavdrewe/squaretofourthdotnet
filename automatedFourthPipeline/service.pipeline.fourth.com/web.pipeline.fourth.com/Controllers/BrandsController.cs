using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Models;
using data.pipeline.fourth.com.Enums;
using shared.pipeline.fourth.com;
using com.fourth.pipeline.pos.Enum;
using Square;
using data.pipeline.fourth.com.Models.Configs.Store;
using web.pipeline.fourth.com.Models;
using Microsoft.AspNetCore.Authorization;
using domain.pipeline.fourth.com.Exceptions;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class BrandsController : Controller
    {

        private readonly FourthPipelineContext _context;
        private readonly SquareCredentialService _squareCredentialService;

        public BrandsController(FourthPipelineContext context,
            SquareCredentialService squareCredentialService)
        {
            _context = context;
            _squareCredentialService = squareCredentialService;
        }

        // GET: Brands
        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = _context.Brands
                .Include(x => x.Stores)
                .Include(x => x.BrandCredentials)
                .Include(x => x.BrandIntegrations).First(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //delete creds for brand
            var creds = _context.CredentialsPool.Where(x => x.BrandId == id).ToList();
            _context.CredentialsPool.RemoveRange(creds);

            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewSquareToFourthSalesIntegration(int whichBrand,
            string unitID,
            FourthSalesRevenueCenterMappingType revenueCenters
            )
        {
            var brand = _context.Brands
                 .Include(x => x.Stores)
                 .Include("Stores.StoreIntegrations.SquareStoreConfigs")
                 .Include("Stores.StoreIntegrations.FourthSalesApiStoreConfigs")
                 .Include(X => X.BrandIntegrations)
                 .Include(x => x.BrandCredentials).First(x => x.Id == whichBrand);

            var squareCredsForBrand = await _context.CredentialsPool
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(x => x.Active
                    && x.BrandId == brand.Id
                    && x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi);
            if (squareCredsForBrand == null ||
                (String.IsNullOrWhiteSpace(squareCredsForBrand.RefreshToken) &&
                 String.IsNullOrWhiteSpace(squareCredsForBrand.LatestAccessToken)))
            {
                throw new NoCreditsException("No Square creds exist for this Square brand: " + brand.Name + " or the oauth access token is empty and you need to authorise first.");
            }

            var accessToken = await _squareCredentialService.GetAccessTokenAsync(squareCredsForBrand);

            //is there a brand integration of this type already? if not create one
            var existingBRandInt = brand.BrandIntegrations.FirstOrDefault(x => x.IntegrationType == IntegrationTypes.SquareToFourthPosSales);
            if (existingBRandInt == null)
            {
                var newBrandIntegration = new BrandIntegration
                {
                    Active = true,
                    IntegrationType = IntegrationTypes.SquareToFourthPosSales,
                    BrandId = brand.Id,
                };
                _context.BrandIntegrations.Add(newBrandIntegration);
                _context.SaveChanges();
            }

            //get stores from square - use SquareClient
            var squareClient = await _squareCredentialService.CreateClientAsync(squareCredsForBrand);
            var locationsResponse = await squareClient.Locations.ListAsync();
            var locations = locationsResponse.Locations?.ToList() ?? new List<Location>();

            // Create stores from Square where they do not exist, and keep existing matches in the setup set.
            var storesAndLocations = new List<StoreAndLocation>();
            foreach (var location in locations)
            {
                var existingStore = brand.Stores.FirstOrDefault(x =>
                    string.Equals(x.Name?.Trim(), location.Name?.Trim(), StringComparison.OrdinalIgnoreCase));

                if (existingStore == null)
                {
                    //create a store for this loc
                    var store = new Store
                    {
                        Active = true,
                        BrandId = brand.Id,
                        Name = location.Name,
                        Timezone = "GMT",
                    };
                    _context.Stores.Add(store);
                    _context.SaveChanges();

                    existingStore = store;
                }

                storesAndLocations.Add(new StoreAndLocation { Store = existingStore, Location = location });
            }

            //now create an integration for square / forth if it was ticked
            var integrationsCreated = 0;
            var integrationsSkipped = 0;
            storesAndLocations.ForEach(storeAndLocation =>
            {
                var existingStoreIntegration = storeAndLocation.Store.StoreIntegrations?
                    .FirstOrDefault(x => x.Active
                        && x.IntegrationType == IntegrationTypes.SquareToFourthPosSales
                        && (x.SquareStoreConfigs ?? new List<SquareStoreConfig>())
                            .Any(y => y.Active && y.LocationId == storeAndLocation.Location.Id));

                if (existingStoreIntegration != null)
                {
                    integrationsSkipped++;
                    return;
                }

                //set vars for which revenue center here
                string thisRevenueCenter = "";
                switch (revenueCenters)
                {
                    case FourthSalesRevenueCenterMappingType.ByStore:
                        thisRevenueCenter = storeAndLocation.Location.Id;
                        break;
                    case FourthSalesRevenueCenterMappingType.ByCategory:
                        break;
                    case FourthSalesRevenueCenterMappingType.SingleRevenueCenter:
                        thisRevenueCenter = "1";
                        break;
                    default:
                        throw new Exception("You didn't provide a revenue center method!");
                }

                _context.StoreIntegrations.Add(new StoreIntegration
                {
                    StoreId = storeAndLocation.Store.Id,
                    IntegrationType = shared.pipeline.fourth.com.IntegrationTypes.SquareToFourthPosSales,
                    IntegrationSubType = shared.pipeline.fourth.com.IntegrationSubTypes.None,
                    Active = true,
                    WhenCreatedUTC = DateTime.UtcNow,
                    WhenUpdatedUTC = DateTime.UtcNow,
                    SquareStoreConfigs = new List<SquareStoreConfig>{
                        new SquareStoreConfig{
                            WhenCreatedUTC = DateTime.UtcNow,
                            WhenUpdatedUTC = DateTime.UtcNow,
                            Active = true,
                            LocationId = storeAndLocation.Location.Id
                        },
                    },
                    FourthSalesApiStoreConfigs = new List<FourthSalesApiStoreConfig>{
                        new FourthSalesApiStoreConfig{
                            WhenCreatedUTC = DateTime.UtcNow,
                            WhenUpdatedUTC = DateTime.UtcNow,
                            Active = true,
                            SiteLocationCode = storeAndLocation.Location.Id,
                            RevenueCenter = thisRevenueCenter,
                            RevenueCenterMappingType = revenueCenters,
                            UnitId = unitID
                        }
                    },
                });

                brand.InitalSetupSytemType = shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi;
                _context.Update(brand);
                _context.SaveChanges();
                integrationsCreated++;
            });


            return Ok($"Completed. Created {integrationsCreated} store integrations; skipped {integrationsSkipped} existing integrations.");
        }

        [HttpGet]
        public async Task<IActionResult> CreateNewSquareToFourthSalesIntegration()
        {
            ViewData["BrandList"] = new SelectList(_context.Brands, "Id", "Name");
            return View(await _context.Brands.ToListAsync());
        }

    }
}

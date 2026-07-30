using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Models;
using shared.pipeline.fourth.com;
using Microsoft.AspNetCore.Authorization;
using Square;
using domain.pipeline.fourth.com.Exceptions;
using domain.pipeline.fourth.com.Services.Square;
using System.IO;
using CsvHelper;
using web.pipeline.fourth.com.Services;

namespace web.pipeline.fourth.com.Controllers
{
    [Authorize]
    public class BrandIntegrationsController : Controller
    {
        private readonly FourthPipelineContext _context;
        private readonly SquareCredentialService _squareCredentialService;

        public BrandIntegrationsController(
            FourthPipelineContext context,
            SquareCredentialService squareCredentialService)
        {
            _context = context;
            _squareCredentialService = squareCredentialService;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateTestData(
            int brandIntegrationId,
           IntegrationTypes integrationType)
        {
            IList<GenerateSquareTestDataResult> results = new List<GenerateSquareTestDataResult>();
            List<string> errorResults = new List<string>();
            switch (integrationType)
            {
                case IntegrationTypes.None:
                    break;
                case IntegrationTypes.SquareToFourthPosSales:
                    results = await _GenerateSquareTestData(brandIntegrationId);
                    break;
                case IntegrationTypes.RevelToFourthPosSales:
                    break;
                default:
                    break;
            }

            if (results.Count() > 0)
                return Ok(results);

            else return BadRequest(errorResults);
        }

        class GenerateSquareTestDataResult
        {
            public string csvName { get; set; }
            public string csvString { get; set; }
            public List<string> errors { get; set; }
        }

        private async Task<IList<GenerateSquareTestDataResult>> _GenerateSquareTestData(int brandIntegrationId)
        {
            var errorResults = new List<string>();
            try
            {
                var integration = _context.BrandIntegrations.Find(brandIntegrationId);

                var startDate = new DateTime(2019, 08, 08);
                var transactionDate = startDate;
                var endDate = startDate.AddDays(1);

                var allBRands = await _context
                    .Brands.Where(x => x.Active)
                    .Where(x => x.Id == integration.BrandId)
                    .Include(x => x.BrandIntegrations)
                    .Include("Stores.StoreIntegrations")
                    .ToListAsync();

                var allPotentialBrands = allBRands.Where(x => x.BrandIntegrations.Count() > 0).ToList();
                foreach (var brand in allPotentialBrands)
                {
                    try
                    {
                        var doesBrandHaveActiveIntegrationOfType = brand.BrandIntegrations.FirstOrDefault(x => x.IntegrationType == IntegrationTypes.SquareToFourthPosSales && x.Active);
                        if (doesBrandHaveActiveIntegrationOfType != null)
                        {
                            var squareCredsForBrand = await _context.CredentialsPool.Where(X => X.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi).FirstOrDefaultAsync(x => x.Active && x.BrandId == brand.Id);
                            if (squareCredsForBrand == null)
                            {
                                throw new NoCreditsException("No Square creds exist for this Square brand: " + brand.Name);
                            }

                            var storeIdsFOrTHisBrand = brand.Stores.Select(x => x.Id).ToList();
                            var storesWithActiveRevelToFourthIntegrationForThisBrand = _context
                                .StoreIntegrations
                                .Include(X => X.FourthSalesApiStoreConfigs)
                                .Include(x => x.Store)
                                .Include(X => X.SquareStoreConfigs)
                                .Where(x => x.IntegrationType == IntegrationTypes.SquareToFourthPosSales)
                                .Where(x => x.Active)
                                .Where(x => storeIdsFOrTHisBrand.Contains(x.StoreId));

                            //now use SquareClient
                            var accessToken = await _squareCredentialService.GetAccessTokenAsync(squareCredsForBrand);
                            var squareClient = await _squareCredentialService.CreateClientAsync(squareCredsForBrand);
                            var locationsResponse = await squareClient.Locations.ListAsync();
                            var allLocationsForBrand = locationsResponse.Locations?.ToList() ?? new List<Location>();
                            var dataGen = new SquareToFourthCSVGenerator(accessToken, _squareCredentialService.GetApiBaseUrl(squareCredsForBrand));
                            await dataGen.GatherDataForBrand(); //brand level


                            var returnedRestults = new List<GenerateSquareTestDataResult>();
                            await storesWithActiveRevelToFourthIntegrationForThisBrand.ForEachAsync(async squareToFourthStoreIntegration =>
                            {
                                if (squareToFourthStoreIntegration.SquareStoreConfigs.Count > 0 && squareToFourthStoreIntegration.FourthSalesApiStoreConfigs.Count > 0)
                                {
                                    var squareConfigForStore = squareToFourthStoreIntegration.SquareStoreConfigs.FirstOrDefault(x => x.Active == true);
                                    if (squareConfigForStore == null)
                                    {
                                        throw new NoCreditsException("No Fourth Sales Api creds exist for this Revel brand: " + brand.Name);
                                    }

                                    var fourthConfigForStore = squareToFourthStoreIntegration.FourthSalesApiStoreConfigs.FirstOrDefault(x => x.Active == true);
                                    if (fourthConfigForStore == null)
                                    {
                                        throw new NoCreditsException("No Fourth Sales Api creds exist for this Revel brand: " + brand.Name);
                                    }
                                    var thisLocation = allLocationsForBrand.First(x => x.Id == squareConfigForStore.LocationId);

                                    await dataGen.GatherDataForLocation(startDate, endDate, thisLocation);

                                    var dataToSend = dataGen.CreateSalesRows(fourthConfigForStore.UnitId);

                                    var baseFileName = @"c:\test\";
                                    var csvName = String.Format("{0}_{1}_{2}_SquareTestsPipeline.csv", transactionDate.ToString("yyyy_MM_dd"), brand.Name, squareToFourthStoreIntegration.Store.Name);
                                    var csvFullPath = Path.Combine(baseFileName, csvName);

                                    var csvAsString = "";
                                    using (var stream = new MemoryStream())
                                    using (var reader = new StreamReader(stream))
                                    using (var writer = new StreamWriter(stream))
                                    using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
                                    {
                                        csv.WriteRecords(dataToSend);
                                        csv.Flush();
                                        stream.Position = 0;
                                        csvAsString = reader.ReadToEnd();
                                    }

                                    using (var writer = new StreamWriter(csvFullPath))
                                    using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
                                    {
                                        csv.WriteRecords(dataToSend);
                                        csv.Flush();
                                    }

                                    returnedRestults.Add(new GenerateSquareTestDataResult
                                    {
                                        csvName = csvName,
                                        csvString = csvAsString,
                                        errors = errorResults
                                    });

                                }
                                else
                                {
                                    Console.WriteLine("This stores didn't have the proper creds for this integraton");
                                }
                            });

                            return returnedRestults;

                        }

                        throw new Exception("Brand doesn't have integration of that type");
                    }
                    catch (Exception)
                    {
                        //A single brand failed, go to next, but sound alarm
                    }
                }
                throw new Exception("There were no brands!");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckMissingStoresForIntegration(int brandIntegrationId,
            IntegrationTypes integrationType)
        {
            bool result = false;
            List<string> errorResults = new List<string>();
            switch (integrationType)
            {
                case IntegrationTypes.None:
                    break;
                case IntegrationTypes.SquareToFourthPosSales:
                    result = await _CheckMissingStoresForSquareIntegration(brandIntegrationId, errorResults);
                    break;
                case IntegrationTypes.RevelToFourthPosSales:
                    break;
                default:
                    break;
            }

            if (result)
                return Ok();

            else return BadRequest(errorResults);
        }

        async Task<bool> _CheckMissingStoresForSquareIntegration(int brandIntegrationId, List<string> errorResults)
        {
            var integration = _context
             .BrandIntegrations.Find(brandIntegrationId);
            var brand = _context.Brands
                .Include(x => x.BrandCredentials)
                .First(x => x.Id == integration.BrandId);

            var allStoresForBrand = _context
                .Stores
                .Where(x => x.BrandId == integration.BrandId)
                .Include(x => x.StoreIntegrations)
                .ToList();

            var activeCreds = brand.BrandCredentials.Where(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi && x.Active).FirstOrDefault();
            if (activeCreds == null)
            {
                errorResults.Add("There are no square creds for this brand!!");
                return false;
            }

            var accessToken = await _squareCredentialService.GetAccessTokenAsync(activeCreds);
            var squareClient = await _squareCredentialService.CreateClientAsync(activeCreds);
            var allLocResponse = await squareClient.Locations.ListAsync();
            var allLocations = allLocResponse.Locations?.ToList() ?? new List<Location>();

            foreach (var locatoin in allLocations)
            {
                var activeStore = allStoresForBrand.FirstOrDefault(x => x.Name == locatoin.Name);
                if (activeStore == null)
                {
                    errorResults.Add(String.Format("There was no active store on file for Square Location {0}", locatoin.Name));
                }
                else
                {
                    var integrationForStore = _context.StoreIntegrations.FirstOrDefault(x => x.StoreId == activeStore.Id && x.IntegrationType == IntegrationTypes.SquareToFourthPosSales);
                    if (integrationForStore == null)
                    {
                        errorResults.Add(String.Format("There was no active integraton for Store {0} of type {1}, even though there was a store.", locatoin.Name, IntegrationTypes.SquareToFourthPosSales.ToString()));
                    }
                }
            }

            return errorResults.Count() == 0 ? true : false;
        }

        [HttpGet]
        public async Task<IActionResult> _VerifyBrandIntegrationStack_SquareToFourth(int brandIntegrationId,
            IntegrationTypes integrationType)
        {

            bool result = false;
            List<string> errorResults = new List<string>();
            switch (integrationType)
            {
                case IntegrationTypes.None:
                    break;
                case IntegrationTypes.SquareToFourthPosSales:
                    result = _VerifySquareToFourthBrandIntegration(brandIntegrationId, out errorResults);
                    break;
                case IntegrationTypes.RevelToFourthPosSales:
                    break;
                default:
                    break;
            }

            if (result)
                return Ok();

            else return BadRequest(errorResults);
        }

        bool _VerifySquareToFourthBrandIntegration(int id, out List<string> errors)
        {
            errors = new List<string>();
            var integrfation = _context
                .BrandIntegrations.Find(id);

            var allStoresForBrand = _context
                .Stores
                .Where(x => x.BrandId == integrfation.BrandId)
                .Include(x => x.StoreIntegrations)
                .ToList();

            foreach (var store in allStoresForBrand)
            {
                var activeIntegrationOfType = store.StoreIntegrations.Where(x => x.Active).FirstOrDefault(x => x.IntegrationType == IntegrationTypes.SquareToFourthPosSales);
                if (activeIntegrationOfType == null)
                {
                    errors.Add(String.Format("There was no active integraton for Store {0} of type {1}", store.Name, IntegrationTypes.SquareToFourthPosSales.ToString()));
                }
                else
                {
                    var configForIntSquare = _context.SquareStoreConfigs.FirstOrDefault(x => x.StoreIntegrationId == activeIntegrationOfType.Id);
                    if (configForIntSquare == null)
                    {
                        errors.Add(String.Format("There was no active integraton for Store {0} of type {1}", store.Name, IntegrationTypes.SquareToFourthPosSales.ToString()));
                    }
                }
            }

            return errors.Count() == 0 ? true : false;
        }

        // GET: BrandIntegrations
        public async Task<IActionResult> Index()
        {
            var fourthPipelineContext = _context.BrandIntegrations.Include(b => b.Brand);
            return View(await fourthPipelineContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var brandIntegration = await _context.BrandIntegrations
                .Include(b => b.Brand)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brandIntegration == null) return NotFound();
            return View(brandIntegration);
        }

        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandIntegration brandIntegration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brandIntegration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", brandIntegration.BrandId);
            return View(brandIntegration);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var brandIntegration = _context
                .BrandIntegrations
                .Include(b => b.Brand.Stores)
                .First(x => x.Id == id);
            if (brandIntegration == null) return NotFound();

            var storeIds = brandIntegration.Brand.Stores.Select(x => x.Id);
            var thisBrandIntType = brandIntegration.IntegrationType;
            var storesAndIntegrations = _context.Stores
            .Include(X => X.StoreIntegrations)
            .Where(x => storeIds.Contains(x.Id))
            .ToList();

            var filteredStoreItnegrations = storesAndIntegrations.SelectMany(x => x.StoreIntegrations.Where(x => x.IntegrationType == thisBrandIntType)).ToList();

            ViewData["integrations"] = filteredStoreItnegrations ?? new List<StoreIntegration>();
            ViewData["stores"] = storesAndIntegrations ?? new List<Store>();

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", brandIntegration.BrandId);
            return View(brandIntegration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BrandIntegration brandIntegration)
        {
            if (id != brandIntegration.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brandIntegration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandIntegrationExists(brandIntegration.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", brandIntegration.BrandId);
            return View(brandIntegration);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var brandIntegration = await _context.BrandIntegrations.FirstOrDefaultAsync(m => m.Id == id);
            if (brandIntegration == null) return NotFound();
            return View(brandIntegration);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brandIntegration = await _context.BrandIntegrations.FindAsync(id);
            _context.BrandIntegrations.Remove(brandIntegration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandIntegrationExists(int id)
        {
            return _context.BrandIntegrations.Any(e => e.Id == id);
        }
    }
}

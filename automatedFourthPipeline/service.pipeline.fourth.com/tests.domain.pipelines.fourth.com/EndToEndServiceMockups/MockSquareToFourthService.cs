using com.fourth.pipeline.pos.Services.SalesApi;
using CsvHelper;
using data.pipeline.fourth.com.Enums;
using domain.pipeline.fourth.com.Exceptions;
using domain.pipeline.fourth.com.Models;
using domain.pipeline.fourth.com.Services.Square;
using domain.pipeline.fourth.com.Services.Square.Oauth;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using shared.pipeline.fourth.com;
using Square;
using Square.TeamMembers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using domain.pipeline.fourth.com.Enums;
using Shouldly;
using domain.pipeline.fourth.com.Helper;

namespace tests.domain.pipelines.fourth.com.EndToEndServiceMockups
{
    [TestFixture]
    public class MockSquareToFourthService
    {
        FourthPipelineContext fourthPipelineContext;

        //Square Creds
        string productionClientID = "";
        string productTionSecret = "";

        FourthApiService fourthSalesApiService;
        SquareOAuthTokenService squareOAuthTokenService;

        [SetUp]
        public async Task Arrange()
        {
            fourthPipelineContext = new FourthPipelineContext();
            squareOAuthTokenService = new SquareOAuthTokenService();

        }

        [Test]
        public async Task LocalDateTimeToUTCTime()
        {

            try
            {
                var localTimeToTest = new DateTime(2020, 01, 27, 08, 00, 00, DateTimeKind.Utc);
                var resultWeExpect = localTimeToTest.AddHours(5);
                var timeZoneInfo = TimeZoneInfo.GetSystemTimeZones();

                var specificTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

                var sytop = "";
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        [Test]
        [Explicit("Requires remote DB credentials and a specific historical Square credential row.")]
        public async Task TestRefreshToken()
        {

            var credsToTest = fourthPipelineContext.CredentialsPool.First(x => x.Id == 44);
            var refreshResult = await squareOAuthTokenService.RefreshSquareToken(productionClientID,
                 productTionSecret, credsToTest.RefreshToken);

        }

        [Test]
        [Explicit("Requires remote DB credentials plus live Square and Fourth credentials.")]
        public async Task RunTheMockSquareToFourthService()
        {
            try
            {
                var startDate = new DateTime(2020, 02, 07, 09, 00, 00, DateTimeKind.Utc);
                var transactionDate = startDate;
                var endDate = startDate.AddDays(1);

                var allBRands = await fourthPipelineContext
                    .Brands.Where(x => x.Active)
                    .Where(x => x.Name.ToLower().Trim() == "junzi")
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
                            var testOrderBy = fourthPipelineContext.CredentialsPool.Where(x => x.Active && x.BrandId == brand.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                            var squareCredsForBrand = await fourthPipelineContext.CredentialsPool.Where(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.SquareApi)
                                .OrderByDescending(x => x.Id).FirstOrDefaultAsync(x => x.Active && x.BrandId == brand.Id);
                            if (squareCredsForBrand == null || String.IsNullOrWhiteSpace(squareCredsForBrand.RefreshToken))
                            {
                                throw new NoCreditsException("No Square creds exist for this Square brand: " + brand.Name + " or the oauth access token is empty and you need to authorise first.");
                            }

                            var resfreshResponse = await squareOAuthTokenService.RefreshSquareToken(productionClientID,
                                 productTionSecret, squareCredsForBrand.RefreshToken);
                            squareCredsForBrand.LatestAccessToken = resfreshResponse.AccessToken;
                            fourthPipelineContext.Update(squareCredsForBrand);

                            var fourthCredsForBrand = await fourthPipelineContext.CredentialsPool.Where(x => x.Active && x.BrandId == brand.Id).OrderByDescending(x => x.Id)
                                .FirstOrDefaultAsync(x => x.CredentialType == shared.pipeline.fouth.com.Enums.CredentialTypes.FourthBaseCredential);
                            if (fourthCredsForBrand == null)
                            {
                                throw new NoCreditsException("No Fourth Sales Api creds exist for this Revel brand: " + brand.Name);
                            }

                            fourthSalesApiService = new FourthApiService(fourthCredsForBrand.Username, fourthCredsForBrand.Password, fourthCredsForBrand.BaseEndpoint);

                            var storeIdsFOrTHisBrand = brand.Stores.Where(x => x.Active).Select(x => x.Id).ToList();
                            var storesWithActiveRevelToFourthIntegrationForThisBrand = fourthPipelineContext
                                .StoreIntegrations
                                .Include(X => X.FourthSalesApiStoreConfigs)
                                .Include(x => x.Store)
                                .Include(X => X.SquareStoreConfigs)
                                .Where(x => x.IntegrationType == IntegrationTypes.SquareToFourthPosSales)
                                .Where(x => x.Active)
                                .Where(x => storeIdsFOrTHisBrand.Contains(x.StoreId));

                            var squareClient = new SquareClient(squareCredsForBrand.LatestAccessToken);
                            var locationsResponse = await squareClient.Locations.ListAsync();
                            var allLocationsForBrand = locationsResponse.Locations?.ToList() ?? new List<Location>();
                            var dataGen = new SquareToFourthCSVGenerator(squareCredsForBrand.LatestAccessToken);
                            await dataGen.GatherDataForBrand();

                            await storesWithActiveRevelToFourthIntegrationForThisBrand.ForEachAsync(async squareToFourthStoreIntegration =>
                             {
                                 try
                                 {
                                     if (squareToFourthStoreIntegration.SquareStoreConfigs.Count > 0 && squareToFourthStoreIntegration.FourthSalesApiStoreConfigs.Count > 0 && squareToFourthStoreIntegration.Store.Active)
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

                                         var dataGatherResult = await dataGen.GatherDataForLocation(startDate, endDate, thisLocation);
                                         if (dataGatherResult.DataGatherResult == DataGatherResult.Complete)
                                         {
                                             var dataToSend = dataGen.CreateSalesRows(fourthConfigForStore.UnitId);

                                             var baseFileName = @"c:\test\";
                                             var csvName = String.Format("{0}_{1}_{2}_SquareFourthSalesPipeline_Gen_{3}.csv", transactionDate.ToString("yyyy_MM_dd"), brand.Name, squareToFourthStoreIntegration.Store.Name, DateTime.UtcNow.ToString("yyyy_MM_dd"));
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

                                             await fourthSalesApiService.Login();
                                             await fourthSalesApiService.SendSalesDataToFourth(csvName, csvFullPath);

                                             true.ShouldBe(true);
                                         }
                                         else if (dataGatherResult.DataGatherResult == DataGatherResult.OrderEmpty)
                                         {
                                         }
                                         else if (dataGatherResult.DataGatherResult == DataGatherResult.Error)
                                         {
                                             throw new Exception("There was an error data in store integration " + squareToFourthStoreIntegration.Store.Name, dataGatherResult.Exception);
                                         }
                                     }
                                     else
                                     {
                                         Console.WriteLine("This stores didn't have the proper creds for this integraton");
                                         throw new NoCreditsException(String.Format("The store {0} didn't have any credits!!", squareToFourthStoreIntegration.Store.Name));
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                 }
                             });

                        }
                    }
                    catch (Exception ex)
                    {
                        var whatHappened = ex.Message;
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

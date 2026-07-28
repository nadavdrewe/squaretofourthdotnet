using domain.geckoboardv2.grind.com.Factory;
using domain.geckoboardv2.grind.com.Models.BoardData;
using domain.geckoboardv2.grind.com.Models.BoardTypeWidgetUrls;
using domain.geckoboardv2.grind.com.Services;
using GeckoboardLibrary.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using Revel._808nd.com.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace tests.domain.geckoboardv2.grind.com.EndToEndLiveTests
{
    [TestFixture]
    public class PushWidgetsToGeckoboardTests
    {

        GrindContext db;
        GeckoboardV2Service SUT;
        IndividualBoardWidgetSetGenerator individualBoardWidgetSetGenerator;
        GrindYesterdayBoardWidgetSetGenerator grindYesterdayBoardWidgetSetGenerator;
        GrindTodayBoardWidgetSetGenerator grindTodayBoardWidgetSetGenerator;
        GeckoboardPushService pushService { get; set; }

        [SetUp]
        public async Task SetUp()
        {
            db = new GrindContext();
            string geckoApiKey = "ab876212d31d37960e3154eb5e2bc0a0";
            SUT = new GeckoboardV2Service();
            individualBoardWidgetSetGenerator = new IndividualBoardWidgetSetGenerator(geckoApiKey);
            grindYesterdayBoardWidgetSetGenerator = new GrindYesterdayBoardWidgetSetGenerator(geckoApiKey);
            grindTodayBoardWidgetSetGenerator = new GrindTodayBoardWidgetSetGenerator(geckoApiKey);

            pushService = new GeckoboardPushService();
            //base.SetUp();


            //var now = DateTime.Now;
            var now = new DateTime(2019, 07, 01, 18, 00, 01);
            await SUT.Bootstrap();
            await SUT.GatherAllDailyAndComparisonRawData(now);
        }

        [Test]
        public async Task TestBudgets()
        {
            var allEsts = db.Establishments.Where(x => x.establishment_id != 2).ToList();
            foreach (var est in allEsts.OrderBy(x => x.establishment_id))
            {
                var budgetToday = SUT.GetBudgetForTodayOnly(est.establishment_id);
                var budgetEntireWeek = SUT.GetBudgetForEntireWTD(est.establishment_id);
            }
        }


        [Test]
        public async Task PushGrindYesterdayWIdgetsToGeckoboard()
        {

            var widgetData = SUT.GenerateYesterdayBoardDataset();
            var widgets = grindYesterdayBoardWidgetSetGenerator.GenerateWidgets(widgetData);
            foreach (var x in widgets)
            {
                await pushService.Push(x);
            };
        }


        [Test]
        public async Task PushGrindTodayWIdgetsToGeckoboard()
        {

            //get the data
            //data gather                                      
            var widgetData = SUT.GenerateTodayBoardDataset();
            var widgets = grindTodayBoardWidgetSetGenerator.GenerateWidgets(widgetData);
            foreach (var x in widgets)
            {
                await pushService.Push(x);
            };

        }

        [Test]
        public async Task PushIndividualWidgetsToGeckoboard()
        {

            var allEsts = db.Establishments.Where(x => x.establishment_id != 2).ToList();
            var allBoardDatasets = new List<IndividualBoardDataset>(); //for parent

            foreach (var est in allEsts.OrderBy(x => x.establishment_id))
            {               
                try
                {
                    var properData = SUT.GenerateIndiviudalBoardDataset(est.establishment_id);

                    var allIndividualBoardWidgetEndpoings = BoardFactory.CreateIndividualGrindStoreBoards();
                    var justShoreditchEndpoints = allIndividualBoardWidgetEndpoings.First(x => x.EstablishmentId == est.establishment_id);

                    var actualWidgets = individualBoardWidgetSetGenerator.GenerateWidgets(properData, justShoreditchEndpoints);

                    //loop and push
                    foreach (var x in actualWidgets)
                    {
                        await pushService.Push(x);
                    };
                }
                catch (Exception ex)
                {
                    var test = "Hey, a board fucked up:" + est.name;
                    //email out
                    //ontot the next one
                }
            }

        }
    }
}

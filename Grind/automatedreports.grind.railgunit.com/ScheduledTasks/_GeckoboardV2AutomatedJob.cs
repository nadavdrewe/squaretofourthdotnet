//using domain.geckoboardv2.grind.com.Factory;
//using domain.geckoboardv2.grind.com.Models.BoardData;
//using domain.geckoboardv2.grind.com.Services;
//using Quartz;
//using Revel._808nd.com.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace automatedreports.grind.railgunit.com.ScheduledTasks
//{
//    public class _GeckoboardV2AutomatedJob : BaseJob
//    {
//        public override async Task Execute(IJobExecutionContext context)
//        {
//            try
//            {
//                GrindContext db;
//                GeckoboardV2Service SUT;
//                IndividualBoardWidgetSetGenerator individualBoardWidgetSetGenerator;
//                GrindYesterdayBoardWidgetSetGenerator grindYesterdayBoardWidgetSetGenerator;
//                GrindTodayBoardWidgetSetGenerator grindTodayBoardWidgetSetGenerator;
//                GeckoboardPushService pushService;


//                db = new GrindContext();
//                string geckoApiKey = "ab876212d31d37960e3154eb5e2bc0a0";
//                SUT = new GeckoboardV2Service();
//                individualBoardWidgetSetGenerator = new IndividualBoardWidgetSetGenerator(geckoApiKey);
//                grindYesterdayBoardWidgetSetGenerator = new GrindYesterdayBoardWidgetSetGenerator(geckoApiKey);
//                grindTodayBoardWidgetSetGenerator = new GrindTodayBoardWidgetSetGenerator(geckoApiKey);

//                pushService = new GeckoboardPushService();
//                //base.SetUp();

//                //var now = DateTime.Now;
//                var now = DateTime.Now;

//                await SUT.Bootstrap();
//                await SUT.GatherAllDailyAndComparisonRawData(now);

//                //do individual boards
//                var allEsts = db.Establishments.Where(x => x.establishment_id != 2).ToList();
//                var allBoardDatasets = new List<IndividualBoardDataset>(); //for parent


//                //do WTD widgets
//                var widgetWTDData = SUT.GenerateWTDBoardDataset();
//                var wtdWidgets = grindTodayBoardWidgetSetGenerator.GenerateWTDWidgets(widgetWTDData);
//                foreach (var x in wtdWidgets)
//                {
//                    await pushService.Push(x);
//                };

//                //do today widget                                              
//                var widgetData = SUT.GenerateTodayBoardDataset();
//                var widgets = grindTodayBoardWidgetSetGenerator.GenerateWidgets(widgetData);
//                foreach (var x in widgets)
//                {
//                    await pushService.Push(x);
//                };




//                //do yesterday widgets
//                var yesterdayWidgetData = SUT.GenerateYesterdayBoardDataset();
//                var widgets2 = grindYesterdayBoardWidgetSetGenerator.GenerateWidgets(yesterdayWidgetData);
//                foreach (var x in widgets2)
//                {
//                    await pushService.Push(x);
//                };

//                foreach (var est in allEsts.OrderBy(x => x.establishment_id))
//                {
//                    try
//                    {
//                        var properData = SUT.GenerateIndiviudalBoardDataset(est.establishment_id);

//                        var allIndividualBoardWidgetEndpoings = BoardFactory.CreateIndividualGrindStoreBoards();
//                        var justShoreditchEndpoints = allIndividualBoardWidgetEndpoings.First(x => x.EstablishmentId == est.establishment_id);

//                        var actualWidgets = individualBoardWidgetSetGenerator.GenerateWidgets(properData, justShoreditchEndpoints);

//                        //loop and push
//                        foreach (var x in actualWidgets)
//                        {
//                            await pushService.Push(x);
//                        };
//                    }
//                    catch (Exception ex)
//                    {
//                        var test = "Hey, a board fucked up:" + est.name;
//                        //email out
//                        //ontot the next one
//                    }
//                }



//            }
//            catch (Exception ex)
//            {
//                using (var client = new GmailClient("grindandco808@gmail.com", "teenpunks23"))
//                {
//                    var listOfPeopleToEmail = new List<string> {
//                    "emailnadz@gmail.com"
//                };

//                    client.Send(listOfPeopleToEmail, "NEW GECKO ERROR!!!", ex.Message + " " + ex.InnerException);
//                }

//            }

//            Console.WriteLine("We're done mate");
//        }
//    }
//}

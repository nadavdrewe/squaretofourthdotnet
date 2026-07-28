using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using GeckoboardLibrary.Classes.WidgetItems;
using GeckoboardLibrary.Classes.Widgets;
using GeckoboardLibrary.Services;
using MailChimp.Types;
using Revel._808nd.com.Classes;

namespace WebReboot.Grind._808nd.com.Controllers
{
    public class DemoController : Controller
    {

        private string APIKEY { get; } = "e8395bfcd1a898c6ebe01776d90ada28";
        private GeckoboardOrganisation org { get; }
        private GeckoboardObjectCreatorFactory factory { get; }


        public class PushWidgetEndpoints
        {
            public string salesTodayEndpoint { get; set; }
            public string numberOfOrdersTodayEndpoint { get; set; }
            public string hotDrinksTodayEndpoint { get; set; }
            public string foodTodayEndpoint { get; set; }
            public string alcoholTodayEndpoint { get; set; }
            public string softDrinksTodayEndpoint { get; set; }
            public string hourAndSpendTodayEndpoint { get; set; }
            //site by site			
            public string site1TodayEndpoint { get; set; }
            public string site2TodayEndpoint { get; set; }
            public string site3TodayEndpoint { get; set; }
            public string site4TodayEndpoint { get; set; }
            public string site5TodayEndpoint { get; set; }
            public string site6TodayEndpoint { get; set; }

            public string last12DaysENdpoint { get; set; }
            public string discountsTodayEndpoint { get; set; }
            public string unpaidOrdersTodayEndpoint { get; set; }
            public string unpaidTopItemsEndpoint { get; set; }
            public string unpaidTopSellersEndpoint { get; set; }

            public string itemsSalesLast12Weeks { get; set; }
            public string yearToDateEndpoint { get; set; }
            public string monthVsBudgetEndpoint { get; set; }
            public string netSalesBulletEndpoint { get; set; }
            public string avgSpendEndpoint { get; set; }
            public string grossSalesLast7Endpoint { get; set; }
            public string lastWeekNetEndpoint { get; set; }
            public string avgServiceTimeEndpoint { get; set; }
            public string yesterdaySameDayLastWeekEndpoint { get; set; }



        }


        public static class DemoVariables
        {

            //daily
            public static decimal salesToday { get; set; }
            public static decimal salesYesterday { get; set; }

            public static decimal numberOfOrdersToday { get; set; }
            public static decimal numberOfOrdersYesterday { get; set; }

            public static decimal hotDrinksToday { get; set; }
            public static decimal hotDrinksYesterday { get; set; }

            public static decimal foodToday { get; set; }
            public static decimal foodYesterday { get; set; }

            public static decimal alcoholToday { get; set; }
            public static decimal alcoholYesterday { get; set; }

            public static decimal softDrinksToday { get; set; }
            public static decimal softDrinksYesterday { get; set; }

            public static decimal hourAndSpendToday { get; set; }

            //site by site
            public static decimal site1Today { get; set; }
            public static decimal site1Yesterday { get; set; }

            public static decimal site2Today { get; set; }
            public static decimal site2Yesterday { get; set; }

            public static decimal site3Today { get; set; }
            public static decimal site3Yesterday { get; set; }

            public static decimal site4Today { get; set; }
            public static decimal site4Yesterday { get; set; }

            public static decimal site5Today { get; set; }
            public static decimal site5Yesterday { get; set; }

            public static decimal site6Today { get; set; }
            public static decimal site6Yesterday { get; set; }

            public static decimal discountsToday { get; set; }
            public static decimal discountsYesterday { get; set; }

            public static decimal unpaidOrdersToday { get; set; }
            public static decimal unpaidOrdersYesterday { get; set; }

            public static decimal yearToDate { get; set; }
            public static decimal YearToDateLast { get; set; }

            public static decimal monthVsBudget { get; set; }
            public static decimal monthVsBudgetLast { get; set; }

            public static decimal avgSpend { get; set; }
            public static decimal avgSpendLast { get; set; }

            public static decimal grossSalesLast7 { get; set; }
            public static decimal grossSalesPrevious7 { get; set; }

            public static decimal lastWeekNet { get; set; }
            public static decimal lastWeekBeforeNet { get; set; }

            public static decimal avgServiceTime { get; set; }
            public static decimal avgServiceTimeLast { get; set; }

            public static decimal yesterdaySameDayLastWeek { get; set; }
            public static decimal yesterdaySameDayLastWeekLast { get; set; }



        }

        public DemoController()
        {

            org =
           new GeckoboardOrganisation(APIKEY, "Railgunit");
            factory = new GeckoboardObjectCreatorFactory(org);
        }


        public async Task SendDemoWidgets()
        {


            //foreach widget set


            List<GeckoboardObject> objects = new List<GeckoboardObject>();
            objects = GetSomeInitalisedWidgets();
            GeckoboardPushService geckoService = new GeckoboardPushService();

            foreach (var widget in objects)
            {
                widget.api_key = APIKEY;
                await geckoService.Push(widget);
            }


        }

        private void InitDemoVariables(decimal baseNumber)
        {
            DemoVariables.salesToday = 2200 + baseNumber;
            DemoVariables.salesYesterday = 1830 + baseNumber;
            DemoVariables.numberOfOrdersToday = 452 + baseNumber;
            DemoVariables.numberOfOrdersYesterday = 382 + baseNumber;
            DemoVariables.hotDrinksToday = 628 + baseNumber;
            DemoVariables.hotDrinksYesterday = 519 + baseNumber;
            DemoVariables.alcoholToday = 172 + baseNumber;
            DemoVariables.alcoholYesterday = 111 + baseNumber;
            DemoVariables.softDrinksToday = 78 + baseNumber;
            DemoVariables.softDrinksYesterday = 71 + baseNumber;

            //site by site - parent
            DemoVariables.site1Today = 600 + baseNumber;
            DemoVariables.site1Yesterday = 500 + baseNumber;
            DemoVariables.site2Today = 300 + baseNumber;
            DemoVariables.site2Yesterday = 400 + baseNumber;
            DemoVariables.site3Today = 750 + baseNumber;
            DemoVariables.site3Yesterday = 610 + baseNumber;

            DemoVariables.site4Today = 140 + baseNumber;
            DemoVariables.site4Yesterday = 310 + baseNumber;
            DemoVariables.site5Yesterday = 670 + baseNumber;
            DemoVariables.site5Yesterday = 590 + baseNumber;
            DemoVariables.site6Yesterday = 780 + baseNumber;
            DemoVariables.site6Yesterday = 490 + baseNumber;

            DemoVariables.discountsToday = 89 + baseNumber;
            DemoVariables.discountsYesterday = 111 + baseNumber;
            DemoVariables.unpaidOrdersToday = 251 + baseNumber;
            DemoVariables.unpaidOrdersYesterday = 123 + baseNumber;

            DemoVariables.yearToDate = 272173 + baseNumber;
            DemoVariables.YearToDateLast = 225546 + baseNumber;

            DemoVariables.monthVsBudget = 42292 + baseNumber;
            DemoVariables.monthVsBudgetLast = 40292 + baseNumber;

            DemoVariables.avgSpend = 281;
            DemoVariables.avgServiceTimeLast = 191;

            DemoVariables.grossSalesLast7 = 9121 + baseNumber;
            DemoVariables.grossSalesPrevious7 = 8541 + baseNumber;

            DemoVariables.lastWeekNet = 11927 + baseNumber;
            DemoVariables.lastWeekBeforeNet = 10285 + baseNumber;

            DemoVariables.avgServiceTime = 54;
            DemoVariables.avgServiceTimeLast = 84;

            DemoVariables.yesterdaySameDayLastWeek = 2800 + baseNumber;
            DemoVariables.yesterdaySameDayLastWeekLast = 2800 + baseNumber;

        }


        private List<GeckoboardObject> GetSomeInitalisedWidgets()
        {
            var endpoints = GetAllBoardEnpointGroups();
            var theWidgets = new List<GeckoboardObject>();

            foreach (var store in endpoints)
            {

                var rnd = new Random();
                var baseNumberDec = (decimal)rnd.Next(50, 150);

                InitDemoVariables(baseNumberDec);

                foreach (PropertyInfo prop in typeof(DemoVariables).GetProperties())
                {
                    var randomVariationDec = (decimal)rnd.Next(1, 180);
                    decimal currentVal = (decimal)prop.GetValue(prop);
                    prop.SetValue(prop, currentVal + randomVariationDec);
                }

                theWidgets.AddRange(GetMockParentLeaderboardData(store));
            }



            return theWidgets;
        }

        private IList<GeckoboardObject> GetMockParentLeaderboardData(PushWidgetEndpoints endpoints)
        {

            var rnd = new Random();
            var amountToAdd = rnd.Next(1, 200);

            var toReturn = new List<GeckoboardObject>();


            //START

            //GROUP SALES DAILY
            var widgetSalesToday = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
               endpoints.salesTodayEndpoint,
               "Today",
               (int)DemoVariables.salesToday,
               "Same Day Last Week",
               (int)Decimal.Round(DemoVariables.salesYesterday));
            toReturn.Add(widgetSalesToday);

            var widgetNoOrders = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
               endpoints.numberOfOrdersTodayEndpoint,
               "Today",
               (int)DemoVariables.numberOfOrdersToday,
               "Same Day Last Week",
               (int)Decimal.Round(DemoVariables.numberOfOrdersYesterday));
            toReturn.Add(widgetNoOrders);

            var widgetHotDrinks = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.hotDrinksTodayEndpoint,
              "Today",
              (int)DemoVariables.hotDrinksToday,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.hotDrinksYesterday));
            toReturn.Add(widgetHotDrinks);

            var widgetFood = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.foodTodayEndpoint,
              "Today",
              (int)DemoVariables.foodToday,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.foodYesterday));
            toReturn.Add(widgetFood);


            var widgetAlcohol = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.alcoholTodayEndpoint,
              "Today",
              (int)DemoVariables.alcoholToday,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.alcoholYesterday));
            toReturn.Add(widgetAlcohol);

            toReturn.Add(widgetFood);


            var widgetSoftDrinks = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.softDrinksTodayEndpoint,
              "Today",
              (int)DemoVariables.softDrinksToday,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.softDrinksYesterday));
            toReturn.Add(widgetSoftDrinks);


            LineV2Widget hourandSpendLineWidget = new LineV2Widget("", endpoints.hourAndSpendTodayEndpoint, "", GeckoboardChartAndItemType.LineV2);
            var hourandSpendLinexAxis = new LineV2XAsis
            {
                type = "",
                labels = new List<string> { "6", "7", "8", "9", "10", "11", "12", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" }
            };


            hourandSpendLineWidget.data.x_axis = hourandSpendLinexAxis;
            hourandSpendLineWidget.data.y_axis = new LineV2YAxis
            {
                format = "currency",
                unit = "GBP"
            };


            var hourandSpendSeriesDecimals = new List<decimal>
            {
                2351,
                6723,
                9128,
                10154,
                15823,
                14848,
                12944,
                11456,
                13283,
                11282,
                10234,
                9571,
                8472,
                8723,
                7824,
                5672,
                2535,
            };

            var hourandSpendSeriesSeries = new LineV2Series { name = "Hour And Spend", data = hourandSpendSeriesDecimals };

            hourandSpendLineWidget.data.series = new List<LineV2Series>
            {
             hourandSpendSeriesSeries
            };
            toReturn.Add(hourandSpendLineWidget);


            ////////////////////////
            //SITE BY SITE - DAILY
            ////////////////////////


            var widgetSite1 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.site1TodayEndpoint,
              "Today",
              (int)DemoVariables.site1Today,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.site1Yesterday));
            toReturn.Add(widgetSite1);


            var widgetSite2 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.site2TodayEndpoint,
              "Today",
              (int)DemoVariables.site2Today,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.site2Yesterday));
            toReturn.Add(widgetSite2);

            var widgetSite3 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
              endpoints.site3TodayEndpoint,
              "Today",
              (int)DemoVariables.site3Today,
              "Same Day Last Week",
              (int)Decimal.Round(DemoVariables.site3Yesterday));
            toReturn.Add(widgetSite3);

            var widgetSite4 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
           endpoints.site4TodayEndpoint,
           "Today",
           (int)DemoVariables.site4Today,
           "Same Day Last Week",
           (int)Decimal.Round(DemoVariables.site4Yesterday));
            toReturn.Add(widgetSite4);

            var widgetSite5 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
            endpoints.site5TodayEndpoint,
            "Today",
            (int)DemoVariables.site5Today,
            "Same Day Last Week",
            (int)Decimal.Round(DemoVariables.site5Yesterday));
            toReturn.Add(widgetSite5);

            var widgetSite6 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
    endpoints.site6TodayEndpoint,
    "Today",
    (int)DemoVariables.site6Today,
    "Same Day Last Week",
    (int)Decimal.Round(DemoVariables.site6Yesterday));
            toReturn.Add(widgetSite6);


            var widgetDiscounts = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.discountsTodayEndpoint,
"Today",
(int)DemoVariables.discountsToday,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.discountsYesterday));
            toReturn.Add(widgetDiscounts);



            var widgetUnpaidOrders = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.unpaidOrdersTodayEndpoint,
"Today",
(int)DemoVariables.unpaidOrdersToday,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.unpaidOrdersYesterday));
            toReturn.Add(widgetUnpaidOrders);



            //top sellers last 12 days
            LineV2Widget last12DaysLineWidget = new LineV2Widget("", endpoints.last12DaysENdpoint, "", GeckoboardChartAndItemType.LineV2);

            var xAxis = new LineV2XAsis
            {
                type = "",
                labels = new List<string> { "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" }
            };


            last12DaysLineWidget.data.x_axis = xAxis;
            last12DaysLineWidget.data.y_axis = new LineV2YAxis
            {
                format = "currency",
                unit = "GBP"
            };

            var series = new List<LineV2Series>();
            var allGroupedItems = new List<LineGraphOrderItemBreakdown>();


            var coffeeSeriesDecimals = new List<decimal>
            {
                12351,
                16723,
                19128,
                22154,
                25823,
                32848,
                33944,
                44456,
                37283,
                34282,
                38234,
                44571
            };

            var foodSeriesDecimals = new List<decimal>
            {
                14351,
                16723,
                17128,
                22154,
                28823,
                30848,
                34944,
                47456,
                47283,
                35282,
                38234,
                42571
            };

            var beverageSeriesDecimals = new List<decimal>
            {
                24351,
                26723,
                27128,
                42154,
                38823,
                20848,
                44944,
                57456,
                57383,
                55482,
                49234,
                45571
            };

            var coffeeSeries = new LineV2Series { name = "Coffee", data = coffeeSeriesDecimals };
            var foodSeries = new LineV2Series { name = "Food", data = foodSeriesDecimals };
            var beverageSeries = new LineV2Series { name = "Beverage", data = beverageSeriesDecimals };
            //end coffee


            last12DaysLineWidget.data.series = new List<LineV2Series>
            {
                coffeeSeries,
                foodSeries,
                beverageSeries

            };
            toReturn.Add(last12DaysLineWidget);


            toReturn.Add(
                new LeaderboardWidget(endpoints.unpaidTopSellersEndpoint)
                {
                    data = new LeaderboardData
                    {
                        items = new List<Leaderboard_Item>
                        {
                            new Leaderboard_Item("James Taylor", 1300 + amountToAdd, 3),
                            new Leaderboard_Item("Alex Johnson", 1201 + amountToAdd, 4),
                            new Leaderboard_Item("Harry Dunne", 972 + amountToAdd, 6),
                            new Leaderboard_Item("Jamie Richter", 860 + amountToAdd, 8),
                            new Leaderboard_Item("Robert Edwars", 832 + amountToAdd, 7),
                            new Leaderboard_Item("Alexa Roberts", 798 + amountToAdd, 5),
                            new Leaderboard_Item("Muhammed Qureshi", 701 + amountToAdd, 2),
                            new Leaderboard_Item("Oliver Jones", 580 + amountToAdd, 1),
                            new Leaderboard_Item("Davos Seawoth", 496 + amountToAdd, 10),
                            new Leaderboard_Item("Stannis Baratheon", 491 + amountToAdd, 9),
                        }
                    }
                });

            //Leaderboard top sales items
            toReturn.Add(
                new LeaderboardWidget(endpoints.unpaidTopItemsEndpoint)
                {
                    data = new LeaderboardData
                    {
                        items = new List<Leaderboard_Item>
                        {
                            new Leaderboard_Item("Skinny Flat White", 900 + amountToAdd, 8),
                            new Leaderboard_Item("Medium Latte", 801 + amountToAdd, 1),
                            new Leaderboard_Item("Medium Cappucino", 761 + amountToAdd, 6),
                            new Leaderboard_Item("Smoked Salmon Bagel", 660 + amountToAdd, 3),
                            new Leaderboard_Item("Hot Chocolate", 547 + amountToAdd, 4),
                            new Leaderboard_Item("Eggs on Toast",  541 + amountToAdd, 7),
                            new Leaderboard_Item("Parma Ham Sandwich", 529 + amountToAdd, 5),
                            new Leaderboard_Item("Brooklyn Beer", 482 + amountToAdd, 9),
                            new Leaderboard_Item("Espresso Martini", 390 + amountToAdd, 10),
                            new Leaderboard_Item("Plain Yoghurt", 270 + amountToAdd, 2),
                        }
                    }
                });



            //GROUP - WEEKLY MONTHLY

            //top sellers last 12 days
            LineV2Widget last12WeekLineWidget = new LineV2Widget("", endpoints.itemsSalesLast12Weeks, "", GeckoboardChartAndItemType.LineV2);

            var last12WeekxAxis = new LineV2XAsis
            {
                type = "",
                labels = new List<string> { "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" }
            };


            last12WeekLineWidget.data.x_axis = last12WeekxAxis;
            last12WeekLineWidget.data.y_axis = new LineV2YAxis
            {
                format = "currency",
                unit = "GBP"
            };

       

            var last12WeekcoffeeSeriesDecimals = new List<decimal>
            {
                52351,
                56723,
                59128,
                32154,
                35823,
                62848,
                63944,
                74456,
                77283,
                84282,
                88234,
                84571
            };

            var last12WeekfoodSeriesDecimals = new List<decimal>
            {
                74351,
                76723,
                57128,
                52154,
                58823,
                60848,
                64944,
                77456,
                77283,
                85282,
                98234,
                82571
            };

            var last12WeekbeverageSeriesDecimals = new List<decimal>
            {
                74351,
                86723,
                97128,
                62154,
                78823,
                60848,
                74944,
                97456,
                77383,
                65482,
                79234,
                75571
            };

            var last12WeekcoffeeSeries = new LineV2Series { name = "Coffee", data = last12WeekcoffeeSeriesDecimals };
            var last12WeekfoodSeries = new LineV2Series { name = "Food", data = last12WeekfoodSeriesDecimals };
            var last12WeekbeverageSeries = new LineV2Series { name = "Beverage", data = last12WeekbeverageSeriesDecimals };
            //end coffee


            last12WeekLineWidget.data.series = new List<LineV2Series>
            {
                last12WeekcoffeeSeries,
                last12WeekfoodSeries,
                last12WeekbeverageSeries

            };
            toReturn.Add(last12WeekLineWidget);

            //this ytd
            var widgetthisYTD = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.yearToDateEndpoint,
"Today",
(int)DemoVariables.yearToDate,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.YearToDateLast));
            toReturn.Add(widgetthisYTD);

            var widgetlastMonthBudget = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.monthVsBudgetEndpoint,
"Today",
(int)DemoVariables.monthVsBudget,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.monthVsBudgetLast));
            toReturn.Add(widgetlastMonthBudget);

            var widgetgrossLast7 = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.grossSalesLast7Endpoint,
"Today",
(int)DemoVariables.grossSalesLast7,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.grossSalesPrevious7));
            toReturn.Add(widgetgrossLast7);


            var widgetAvgSpend = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.avgSpendEndpoint,
"Today",
(int)DemoVariables.avgSpend,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.avgSpendLast));
            toReturn.Add(widgetAvgSpend);

            var widgetLastWeek = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.lastWeekNetEndpoint,
"Today",
(int)DemoVariables.lastWeekNet,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.lastWeekBeforeNet));
            toReturn.Add(widgetLastWeek);


            var widgetAvgServiceTime = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.avgServiceTimeEndpoint,
"Today",
(int)DemoVariables.avgServiceTime,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.avgServiceTimeLast));
            toReturn.Add(widgetAvgServiceTime);

            var widgetYesterdaySameDayLastweek = factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
endpoints.yesterdaySameDayLastWeekEndpoint,
"Today",
(int)DemoVariables.yesterdaySameDayLastWeek,
"Same Day Last Week",
(int)Decimal.Round(DemoVariables.yesterdaySameDayLastWeekLast));
            toReturn.Add(widgetYesterdaySameDayLastweek);

            //bullet chart
            BulletItem item = new BulletItem
            {
                label = "Last Month(NET£)",
                sublabel = "vs Last Month Last Year(NET£) and Budget (Green)",

                axis = new BulletAxis
                {
                    point = new List<string>
                       {
                          "0",(5000).ToString(),(10000).ToString(),(15000).ToString(),(25000).ToString(),(35000).ToString()
                       }
                },

                range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=25000-1},
                               new BulletRange{color="green", start=25000, end=(int)(35000)}

                   },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = "12000" },
                    projected = new BulletMeasureItem { start = "0", end = "0" }

                },

                comparative = new BulletComparative { point = "11000" }
            };

            var BulletMonthlyBudget = factory.CreateBullet(104, "BulletMonthlyBudget", endpoints.netSalesBulletEndpoint, "horizontal", item);

            toReturn.Add(BulletMonthlyBudget);

            return toReturn;
        }


        public List<PushWidgetEndpoints> GetAllBoardEnpointGroups()
        {
            List<PushWidgetEndpoints> endpoints = new List<PushWidgetEndpoints>();

            //parent
            var parenetEndpoints = new PushWidgetEndpoints();
            parenetEndpoints.salesTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-3f19ac5d-9a58-4890-aa9d-da11610ccb54";
            parenetEndpoints.numberOfOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-8ddddabc-6198-4037-9b8d-e5b1b63483c9";
            parenetEndpoints.hotDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-64e39111-5c37-468d-8abb-0dbb298f4ddc";
            parenetEndpoints.foodTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-dddc0eca-b8eb-4e0b-8847-8f90abe08afa";
            parenetEndpoints.alcoholTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-afa941d0-bc06-4ae3-ab08-b2e20b04e7be";
            parenetEndpoints.softDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-bd3f122e-acfa-4f0e-a728-65262c3ce387";
            parenetEndpoints.hourAndSpendTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-0d94b3c4-3e4c-4aaa-a204-c0c92220de0e";
            //site by site			
            parenetEndpoints.site1TodayEndpoint = "https://push.geckoboard.com/v1/send/177274-5e80ba34-9b9f-4a8f-89ba-aec3db04a53b";
            parenetEndpoints.site2TodayEndpoint = "https://push.geckoboard.com/v1/send/177274-c6b2fec7-efa1-4be9-8bc1-c122b1cd99a2";
            parenetEndpoints.site3TodayEndpoint = "https://push.geckoboard.com/v1/send/177274-cca55a63-45c2-4de3-b2a4-9eddad14d36c";
            parenetEndpoints.site4TodayEndpoint = "https://push.geckoboard.com/v1/send/177274-985597bc-7a57-4aa3-b180-5afc0f7648a6";
            parenetEndpoints.site5TodayEndpoint = "https://push.geckoboard.com/v1/send/177274-30f65eaf-e34b-42b0-8bb0-6eb865895dd9";
            parenetEndpoints.site6TodayEndpoint = "https://push.geckoboard.com/v1/send/177274-d6fb2692-7644-43fc-ab90-e27a8cb3aa96";
            parenetEndpoints.discountsTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-dd13025d-d195-430c-9bb7-e590042ed354";
            parenetEndpoints.unpaidOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/177274-2c23eb06-1af2-43d1-97dc-d38baa8a3e3a";
            parenetEndpoints.unpaidTopItemsEndpoint = "https://push.geckoboard.com/v1/send/177274-399eaccb-3ae6-4e8f-8d8c-9b05831e5acb";
            parenetEndpoints.unpaidTopSellersEndpoint = "https://push.geckoboard.com/v1/send/177274-91f6237e-072a-47a3-ad52-e87dcb6d04db";
            parenetEndpoints.last12DaysENdpoint = "https://push.geckoboard.com/v1/send/177274-e05f4408-eec2-446c-9cd9-7b9db9123dd0";
            //weekly monthly

            parenetEndpoints.itemsSalesLast12Weeks = "https://push.geckoboard.com/v1/send/177274-bebee2e9-febb-4607-92ee-09c73c6699fc";
            parenetEndpoints.yearToDateEndpoint = "https://push.geckoboard.com/v1/send/177274-c5f2cc55-8169-419e-9770-67c36a678df0";
            parenetEndpoints.monthVsBudgetEndpoint = "https://push.geckoboard.com/v1/send/177274-5dc08204-6b3f-448d-8bc7-8e6f5448c8b7";
            parenetEndpoints.avgSpendEndpoint = "https://push.geckoboard.com/v1/send/177274-5310214d-4da2-40ac-9bda-2d32ec8e986f";
            parenetEndpoints.netSalesBulletEndpoint = "https://push.geckoboard.com/v1/send/177274-dbb31a39-591b-47a2-8e00-5641b826097e";
            parenetEndpoints.grossSalesLast7Endpoint = "https://push.geckoboard.com/v1/send/177274-3de524bc-7f48-40ea-b823-d96039c2b34c";
            parenetEndpoints.lastWeekNetEndpoint = "https://push.geckoboard.com/v1/send/177274-ac6ed116-d207-4143-bc8d-b4a6540d6e70";
            parenetEndpoints.avgServiceTimeEndpoint = "https://push.geckoboard.com/v1/send/177274-16544954-f52a-4c22-b027-fa0592d2076e";
            parenetEndpoints.yesterdaySameDayLastWeekEndpoint = "https://push.geckoboard.com/v1/send/177274-6c6e2ce0-5452-49c5-9319-acca4190bdbe";

            endpoints.Add(parenetEndpoints);

            //site1
            var site1endpoints = new PushWidgetEndpoints();
            site1endpoints.salesTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23970940-df0c-0133-0a2c-22000b560299";
            site1endpoints.numberOfOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-239845c0-df0c-0133-0a2d-22000b560299";
            site1endpoints.hotDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-2399c260-df0c-0133-0a2e-22000b560299";
            site1endpoints.foodTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-239aeef0-df0c-0133-0a2f-22000b560299";
            site1endpoints.alcoholTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-239c16b0-df0c-0133-0a30-22000b560299";
            site1endpoints.softDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23adeed0-df0c-0133-0a3d-22000b560299";
            site1endpoints.hourAndSpendTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23af3930-df0c-0133-0a3e-22000b560299";

            //site by site			
            site1endpoints.site1TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-239df100-df0c-0133-0a32-22000b560299";
            site1endpoints.site2TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-239f5d50-df0c-0133-0a33-22000b560299";
            site1endpoints.site3TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23b37ae0-df0c-0133-0a41-22000b560299";
            site1endpoints.site4TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23a09dc0-df0c-0133-0a34-22000b560299";
            site1endpoints.site5TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23a254f0-df0c-0133-0a35-22000b560299";
            site1endpoints.site6TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23a457d0-df0c-0133-0a36-22000b560299";
            site1endpoints.discountsTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23a545b0-df0c-0133-0a37-22000b560299";
            site1endpoints.unpaidOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-23b25990-df0c-0133-0a40-22000b560299";
            site1endpoints.unpaidTopItemsEndpoint = "https://push.geckoboard.com/v1/send/108953-23c0c0f0-df0c-0133-0a49-22000b560299";
            site1endpoints.unpaidTopSellersEndpoint = "https://push.geckoboard.com/v1/send/108953-23be5050-df0c-0133-0a47-22000b560299";
            site1endpoints.last12DaysENdpoint = "https://push.geckoboard.com/v1/send/108953-23bfa7c0-df0c-0133-0a48-22000b560299";
            //weekly monthly

            site1endpoints.itemsSalesLast12Weeks = "https://push.geckoboard.com/v1/send/108953-23a6d9b0-df0c-0133-0a38-22000b560299";
            site1endpoints.yearToDateEndpoint = "https://push.geckoboard.com/v1/send/108953-23aa4070-df0c-0133-0a3b-22000b560299";
            site1endpoints.monthVsBudgetEndpoint = "https://push.geckoboard.com/v1/send/108953-23a8ee00-df0c-0133-0a3a-22000b560299";
            site1endpoints.avgSpendEndpoint = "https://push.geckoboard.com/v1/send/108953-23ac6d90-df0c-0133-0a3c-22000b560299";
            site1endpoints.netSalesBulletEndpoint = "https://push.geckoboard.com/v1/send/108953-239cf1c0-df0c-0133-0a31-22000b560299";
            site1endpoints.grossSalesLast7Endpoint = "https://push.geckoboard.com/v1/send/108953-23a7ead0-df0c-0133-0a39-22000b560299";
            site1endpoints.lastWeekNetEndpoint = "https://push.geckoboard.com/v1/send/108953-23badc80-df0c-0133-0a45-22000b560299";
            site1endpoints.avgServiceTimeEndpoint = "https://push.geckoboard.com/v1/send/108953-23b05ae0-df0c-0133-0a3f-22000b560299";
            site1endpoints.yesterdaySameDayLastWeekEndpoint = "https://push.geckoboard.com/v1/send/108953-23bc4680-df0c-0133-0a46-22000b560299";
            
            endpoints.Add(site1endpoints);

            //site2
            var site2endpoints = new PushWidgetEndpoints();
            site2endpoints.salesTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f1d5f0-df15-0133-ec39-22000b4a0396";
            site2endpoints.numberOfOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f2bba0-df15-0133-ec3a-22000b4a0396";
            site2endpoints.hotDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f381f0-df15-0133-ec3b-22000b4a0396";
            site2endpoints.foodTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f48170-df15-0133-ec3c-22000b4a0396";
            site2endpoints.alcoholTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f59960-df15-0133-ec3d-22000b4a0396";
            site2endpoints.softDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-340558a0-df15-0133-ec4a-22000b4a0396";
            site2endpoints.hourAndSpendTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-34064350-df15-0133-ec4b-22000b4a0396";

            //site by site			
            site2endpoints.site1TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f848e0-df15-0133-ec3f-22000b4a0396";
            site2endpoints.site2TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33f974f0-df15-0133-ec40-22000b4a0396";
            site2endpoints.site3TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-3409ec10-df15-0133-ec4e-22000b4a0396";
            site2endpoints.site4TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33fa9da0-df15-0133-ec41-22000b4a0396";
            site2endpoints.site5TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33fbd5c0-df15-0133-ec42-22000b4a0396";
            site2endpoints.site6TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33fca560-df15-0133-ec43-22000b4a0396";
            site2endpoints.discountsTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-33fdafa0-df15-0133-ec44-22000b4a0396";
            site2endpoints.unpaidOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-3408e610-df15-0133-ec4d-22000b4a0396";
            site2endpoints.unpaidTopItemsEndpoint = "https://push.geckoboard.com/v1/send/108953-3415e7f0-df15-0133-ec56-22000b4a0396";
            site2endpoints.unpaidTopSellersEndpoint = "https://push.geckoboard.com/v1/send/108953-3413aeb0-df15-0133-ec54-22000b4a0396";
            site2endpoints.last12DaysENdpoint = "https://push.geckoboard.com/v1/send/108953-3414cb20-df15-0133-ec55-22000b4a0396";
            //weekly monthly
            site2endpoints.itemsSalesLast12Weeks = "https://push.geckoboard.com/v1/send/108953-33fec7b0-df15-0133-ec45-22000b4a0396";
            site2endpoints.yearToDateEndpoint = "https://push.geckoboard.com/v1/send/108953-34033700-df15-0133-ec48-22000b4a0396";
            site2endpoints.monthVsBudgetEndpoint = "https://push.geckoboard.com/v1/send/108953-3401f790-df15-0133-ec47-22000b4a0396";
            site2endpoints.avgSpendEndpoint = "https://push.geckoboard.com/v1/send/108953-34044830-df15-0133-ec49-22000b4a0396";
            site2endpoints.netSalesBulletEndpoint = "https://push.geckoboard.com/v1/send/108953-33f6d790-df15-0133-ec3e-22000b4a0396";
            site2endpoints.grossSalesLast7Endpoint = "https://push.geckoboard.com/v1/send/108953-34008d70-df15-0133-ec46-22000b4a0396";
            site2endpoints.lastWeekNetEndpoint = "https://push.geckoboard.com/v1/send/108953-340febf0-df15-0133-ec52-22000b4a0396";
            site2endpoints.avgServiceTimeEndpoint = "https://push.geckoboard.com/v1/send/108953-34080ce0-df15-0133-ec4c-22000b4a0396";
            site2endpoints.yesterdaySameDayLastWeekEndpoint = "https://push.geckoboard.com/v1/send/108953-34120be0-df15-0133-ec53-22000b4a0396";
            endpoints.Add(site2endpoints);

            //site3
            var site3endpoints = new PushWidgetEndpoints();
            site3endpoints.salesTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-289f9430-df18-0133-ec5d-22000b4a0396";
            site3endpoints.numberOfOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28a105b0-df18-0133-ec5e-22000b4a0396";
            site3endpoints.hotDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28a35280-df18-0133-ec5f-22000b4a0396";
            site3endpoints.foodTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28a54a00-df18-0133-ec60-22000b4a0396";
            site3endpoints.alcoholTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28a75910-df18-0133-ec61-22000b4a0396";
            site3endpoints.softDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28b7b6e0-df18-0133-ec6e-22000b4a0396";
            site3endpoints.hourAndSpendTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28b939d0-df18-0133-ec6f-22000b4a0396";

            //site by site			
            site3endpoints.site1TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28aa4580-df18-0133-ec63-22000b4a0396";
            site3endpoints.site2TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28ab5c10-df18-0133-ec64-22000b4a0396";
            site3endpoints.site3TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28bda490-df18-0133-ec72-22000b4a0396";
            site3endpoints.site4TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28acab40-df18-0133-ec65-22000b4a0396";
            site3endpoints.site5TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28ae8c30-df18-0133-ec66-22000b4a0396";
            site3endpoints.site6TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28afb3f0-df18-0133-ec67-22000b4a0396";
            site3endpoints.discountsTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28b09800-df18-0133-ec68-22000b4a0396";
            site3endpoints.unpaidOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-28bc5140-df18-0133-ec71-22000b4a0396";
            site3endpoints.unpaidTopItemsEndpoint = "https://push.geckoboard.com/v1/send/108953-28ca0dc0-df18-0133-ec7a-22000b4a0396";
            site3endpoints.unpaidTopSellersEndpoint = "https://push.geckoboard.com/v1/send/108953-28c73d60-df18-0133-ec78-22000b4a0396";
            site3endpoints.last12DaysENdpoint = "https://push.geckoboard.com/v1/send/108953-28c8b700-df18-0133-ec79-22000b4a0396";
            //weekly monthly
            site3endpoints.itemsSalesLast12Weeks = "https://push.geckoboard.com/v1/send/108953-28b1d0f0-df18-0133-ec69-22000b4a0396";
            site3endpoints.yearToDateEndpoint = "https://push.geckoboard.com/v1/send/108953-28b5d250-df18-0133-ec6c-22000b4a0396";
            site3endpoints.monthVsBudgetEndpoint = "https://push.geckoboard.com/v1/send/108953-28b49dd0-df18-0133-ec6b-22000b4a0396";
            site3endpoints.avgSpendEndpoint = "https://push.geckoboard.com/v1/send/108953-28b6aa30-df18-0133-ec6d-22000b4a0396";
            site3endpoints.netSalesBulletEndpoint = "https://push.geckoboard.com/v1/send/108953-28a8baf0-df18-0133-ec62-22000b4a0396";
            site3endpoints.grossSalesLast7Endpoint = "https://push.geckoboard.com/v1/send/108953-28b32a30-df18-0133-ec6a-22000b4a0396";
            site3endpoints.lastWeekNetEndpoint = "https://push.geckoboard.com/v1/send/108953-28c42410-df18-0133-ec76-22000b4a0396";
            site3endpoints.avgServiceTimeEndpoint = "https://push.geckoboard.com/v1/send/108953-28babb00-df18-0133-ec70-22000b4a0396";
            site3endpoints.yesterdaySameDayLastWeekEndpoint = "https://push.geckoboard.com/v1/send/108953-28c52920-df18-0133-ec77-22000b4a0396";
            endpoints.Add(site3endpoints);
            //site4
            var site4endpoints = new PushWidgetEndpoints();
            site4endpoints.salesTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69c72940-df18-0133-f492-22000bf8a2ac";
            site4endpoints.numberOfOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69c8f220-df18-0133-f493-22000bf8a2ac";
            site4endpoints.hotDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69c9fad0-df18-0133-f494-22000bf8a2ac";
            site4endpoints.foodTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69cb42b0-df18-0133-f495-22000bf8a2ac";
            site4endpoints.alcoholTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69cc5010-df18-0133-f496-22000bf8a2ac";
            site4endpoints.softDrinksTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69d98560-df18-0133-f4a3-22000bf8a2ac";
            site4endpoints.hourAndSpendTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69da9d80-df18-0133-f4a4-22000bf8a2ac";

            //site by site			
            site4endpoints.site1TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69cf1c10-df18-0133-f498-22000bf8a2ac";
            site4endpoints.site2TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69d02380-df18-0133-f499-22000bf8a2ac";
            site4endpoints.site3TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69deb280-df18-0133-f4a7-22000bf8a2ac";
            site4endpoints.site4TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69d0fa60-df18-0133-f49a-22000bf8a2ac";
            site4endpoints.site5TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69d1dc00-df18-0133-f49b-22000bf8a2ac";
            site4endpoints.site6TodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69d2d3c0-df18-0133-f49c-22000bf8a2ac";
            site4endpoints.discountsTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69d3bff0-df18-0133-f49d-22000bf8a2ac";
            site4endpoints.unpaidOrdersTodayEndpoint = "https://push.geckoboard.com/v1/send/108953-69dd6fb0-df18-0133-f4a6-22000bf8a2ac";
            site4endpoints.unpaidTopItemsEndpoint = "https://push.geckoboard.com/v1/send/108953-69ed51f0-df18-0133-f4af-22000bf8a2ac";
            site4endpoints.unpaidTopSellersEndpoint = "https://push.geckoboard.com/v1/send/108953-69e8e490-df18-0133-f4ad-22000bf8a2ac";
            site4endpoints.last12DaysENdpoint = "https://push.geckoboard.com/v1/send/108953-69eb5df0-df18-0133-f4ae-22000bf8a2ac";
            //weekly monthly
            site4endpoints.itemsSalesLast12Weeks = "https://push.geckoboard.com/v1/send/108953-69d4b110-df18-0133-f49e-22000bf8a2ac";
            site4endpoints.yearToDateEndpoint = "https://push.geckoboard.com/v1/send/108953-69d77350-df18-0133-f4a1-22000bf8a2ac";
            site4endpoints.monthVsBudgetEndpoint = "https://push.geckoboard.com/v1/send/108953-69d69730-df18-0133-f4a0-22000bf8a2ac";
            site4endpoints.avgSpendEndpoint = "https://push.geckoboard.com/v1/send/108953-69d84b20-df18-0133-f4a2-22000bf8a2ac";
            site4endpoints.netSalesBulletEndpoint = "https://push.geckoboard.com/v1/send/108953-69cd6a40-df18-0133-f497-22000bf8a2ac";
            site4endpoints.grossSalesLast7Endpoint = "https://push.geckoboard.com/v1/send/108953-69d5b760-df18-0133-f49f-22000bf8a2ac";
            site4endpoints.lastWeekNetEndpoint = "https://push.geckoboard.com/v1/send/108953-69e612d0-df18-0133-f4ab-22000bf8a2ac";
            site4endpoints.avgServiceTimeEndpoint = "https://push.geckoboard.com/v1/send/108953-69dbd3f0-df18-0133-f4a5-22000bf8a2ac";
            site4endpoints.yesterdaySameDayLastWeekEndpoint = "https://push.geckoboard.com/v1/send/108953-69e73b30-df18-0133-f4ac-22000bf8a2ac";
            endpoints.Add(site4endpoints);
            //site

            return endpoints;
        }
    }
}

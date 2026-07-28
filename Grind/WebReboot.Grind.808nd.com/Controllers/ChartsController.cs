using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting;
using Revel._808nd.com.Classes.Reporting.ReportingFactory;
using Revel._808nd.com.Models;
using Revel._808nd.com.ReportingModel;
using Revel._808nd.com.ReportingModel.ChartModels;

namespace WebReboot.Grind._808nd.com.Controllers
{

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }

    [Authorize]
    public class ChartsController : Controller
    {
        GrindContext db = new GrindContext();
        // GET: Charts

        public ActionResult Test()
        {
            ViewBag.EstablishmentsSelect =
               db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            return View();
        }
        public ActionResult Index()
        {
            return RedirectToAction("DaliesSinglePage");

            /*ViewBag.EstablishmentsSelect =
               db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();*/

            return View();
        }

        [HttpPost]
        public ActionResult Index(int NoOfWeeks, int EstablishmentId = 1)
        {

            var end = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
            var start = end.AddDays(-(NoOfWeeks * 7));

            var revelTimeStart = new DateTime(start.Year, start.Month, start.Day, 02, 00, 00);
            var revelTImeEnd = new DateTime(end.Year, end.Month, end.Day, 02, 00, 00);

            IList<ChartData> numberOfOrdersData = new List<ChartData>();
            IList<ChartData> totalPureSalesEachWeek = new List<ChartData>();
            IList<ChartWeekItemSales> table_totalPureSalesEachWeek = new List<ChartWeekItemSales>();
            IList<ChartWeekItemSales> table_SumOfTotalQuantity = new List<ChartWeekItemSales>();
            IList<ChartWeekItemSales> table_AverageItemValue = new List<ChartWeekItemSales>();
            IList<ChartWeekItemSales> table_AverageNoOfItemsPerOrder = new List<ChartWeekItemSales>();
            IList<ChartWeekItemSales> table_AverageOrderValue = new List<ChartWeekItemSales>();


            //GRAPHS
            ViewBag.ItemStackedColumnData = GetStackedItemsGraph(EstablishmentId, revelTimeStart, revelTImeEnd, out numberOfOrdersData,
                out totalPureSalesEachWeek,
                out table_totalPureSalesEachWeek,
                out table_SumOfTotalQuantity,
                out table_AverageItemValue,
                out table_AverageNoOfItemsPerOrder,
                out table_AverageOrderValue);

            ViewBag.NumberOfOrdersData = numberOfOrdersData;

            ViewBag.WeeklySalesData = totalPureSalesEachWeek;

            //TABLES
            ViewBag.Table_TotalPureSales = table_totalPureSalesEachWeek;
            ViewBag.Table_SumOfTotalQuantity = table_SumOfTotalQuantity;
            ViewBag.Table_AverageItemValue = table_AverageItemValue;
            ViewBag.Table_AverageNoOfItemsPerOrder = table_AverageNoOfItemsPerOrder;
            ViewBag.Table_AverageOrderValue = table_AverageOrderValue;

            ViewBag.HasFired = "true";
            ViewBag.EstablishmentsSelect =
              db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            ViewBag.Establishment = db.Establishments.First(x => x.establishment_id == EstablishmentId).name;

            return View();
        }

        private StackedChartItemGrouping GetStackedItemsGraph(int establishmentId, DateTime start, DateTime end,
            out IList<ChartData> numberOfOrdersPerWeek,
            out IList<ChartData> totalPureSalesForAllItems,
            out IList<ChartWeekItemSales> table_totalPureSales,
            out IList<ChartWeekItemSales> table_SumOfTotalQuantity,
            out IList<ChartWeekItemSales> table_AverageItemValue,
            out IList<ChartWeekItemSales> table_AverageNoOfItemsPerOrder,
            out IList<ChartWeekItemSales> table_AverageOrderValue)
        {

            //INIT
            numberOfOrdersPerWeek = new List<ChartData>();
            totalPureSalesForAllItems = new List<ChartData>();
            table_totalPureSales = new List<ChartWeekItemSales>();
            table_SumOfTotalQuantity = new List<ChartWeekItemSales>();
            table_AverageItemValue = new List<ChartWeekItemSales>();
            table_AverageNoOfItemsPerOrder = new List<ChartWeekItemSales>();
            table_AverageOrderValue = new List<ChartWeekItemSales>();

            var allData = new List<ChartData>();

            var est = db.Establishments.First(x => x.establishment_id == establishmentId);
            var prodCats = db.ProductCategories.Where(x => x.establishment_id == est.establishment_id).ToList();
            //get parent specificItemSalesFigure for all stores

            //////////////////////
            //start week rotations
            //////////////////////
            var numberOfDaysInCycle = 7;
            var listOfCycleStartDates = new List<DateTime>();

            var currentCycleStartDate = start;

            //Generate list of dates for reports
            while (currentCycleStartDate < end)
            {
                listOfCycleStartDates.Add(currentCycleStartDate);
                currentCycleStartDate = currentCycleStartDate.AddDays(numberOfDaysInCycle);
            }

            var currentCycle = 1;

            foreach (var startDate in listOfCycleStartDates)
            {
                var endDate = startDate.AddDays(7);

                var prodClasses = db.ProductClasses.ToList();
                IList<OrderItem> compsAndVoids = new List<OrderItem>();
                IList<Product> productsForEst = new List<Product>();
                var allItemsForEstablishmentProducts = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db, prodCats, startDate, endDate, out compsAndVoids, out productsForEst);


                var totalSalesFigureForAllItems = allItemsForEstablishmentProducts.Sum(x => x.pure_sales);
                var totalOrderIdsForThoseItems = allItemsForEstablishmentProducts.Select(x => x.parent_order_id).Distinct().ToList();

                var averageOrderSizeInPounds = 0.00M;


                try
                {
                    averageOrderSizeInPounds = Convert.ToDecimal(totalSalesFigureForAllItems) /
                                       Convert.ToDecimal(totalOrderIdsForThoseItems.Count());
                }
                catch (Exception)
                {


                }

                var prodWrapperService = new RevelProductAndCategoryWrapper();
                prodWrapperService.Products = productsForEst.ToList();

                OrderItemTypeCategoryBreakdown allItemsAsCategoryBreakdowns =
                    OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(allItemsForEstablishmentProducts, est,
                        productsForEst, prodClasses);

                //TABLES - PURE SALES PER ITEM
                try
                {
                    table_totalPureSales.Add(
                             new ChartWeekItemSales
                             {
                                 CategoryName = currentCycle.ToString(),
                                 WeekDateStart = startDate.ToString(),
                                 ChartData = new List<ChartData>
                                 {
                            new ChartData("Hot Drinks", allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x => x.pure_sales), currentCycle, startDate),
                            new ChartData("Food", allItemsAsCategoryBreakdowns.FoodItems.Sum(x => x.pure_sales), currentCycle, startDate),
                            new ChartData("Soft Drinks", allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x => x.pure_sales), currentCycle, startDate),
                            new ChartData("Alcohol", allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x => x.pure_sales), currentCycle, startDate),
                            new ChartData("Others", allItemsAsCategoryBreakdowns.OtherItems.Sum(x => x.pure_sales), currentCycle, startDate),
                                 }
                             });
                }
                catch (Exception)
                {


                }


                //TABLE - SUM OF TOTAL QUANTITY
                try
                {
                    table_SumOfTotalQuantity.Add(
                           new ChartWeekItemSales
                           {
                               CategoryName = currentCycle.ToString(),
                               WeekDateStart = startDate.ToString(),
                               ChartData = new List<ChartData>{

                new ChartData("Hot Drinks", allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x=>x.quantity), currentCycle, startDate),
                new ChartData("Food", allItemsAsCategoryBreakdowns.FoodItems.Sum(x=>x.quantity), currentCycle, startDate),
                new ChartData("Soft Drinks", allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x=>x.quantity), currentCycle, startDate),
                new ChartData("Alcohol",allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x=>x.quantity), currentCycle, startDate),
                new ChartData("Others",        allItemsAsCategoryBreakdowns.OtherItems.Sum(x=>x.quantity), currentCycle, startDate),
                       }
                           });
                }
                catch (Exception)
                {


                }

                //TABLE - AVERAGE ITEM VALUE
                try
                {
                    table_AverageItemValue.Add(
                              new ChartWeekItemSales
                              {
                                  CategoryName = currentCycle.ToString(),
                                  WeekDateStart = startDate.ToString(),
                                  ChartData = new List<ChartData>
                                  {

                            new ChartData("Hot Drinks",
                                allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x => x.pure_sales)/allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x => x.quantity), currentCycle, startDate),
                            new ChartData("Food", allItemsAsCategoryBreakdowns.FoodItems.Sum(x => x.pure_sales)/allItemsAsCategoryBreakdowns.FoodItems.Sum(x => x.quantity), currentCycle,
                                startDate),
                            new ChartData("Soft Drinks",
                                allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x => x.pure_sales)/allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x => x.quantity), currentCycle, startDate),
                            new ChartData("Alcohol",
                                allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x => x.pure_sales)/allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x => x.quantity), currentCycle, startDate),
                            new ChartData("Others",        allItemsAsCategoryBreakdowns.OtherItems.Sum(x => x.pure_sales)/       allItemsAsCategoryBreakdowns.OtherItems.Sum(x => x.quantity), currentCycle,
                                startDate)
                                  }
                              });
                }
                catch (Exception)
                {


                }


                //ITEMS PER ORDER
                try
                {
                    table_AverageNoOfItemsPerOrder.Add(
                                new ChartWeekItemSales
                                {
                                    CategoryName = currentCycle.ToString(),
                                    WeekDateStart = startDate.ToString(),
                                    ChartData = new List<ChartData>
                                    {
                            new ChartData("Hot Drinks",
                                allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x => x.quantity)/Convert.ToDecimal(totalOrderIdsForThoseItems.Count()),
                                currentCycle, startDate),
                            new ChartData("Food",
                                allItemsAsCategoryBreakdowns.FoodItems.Sum(x => x.quantity)/Convert.ToDecimal(totalOrderIdsForThoseItems.Count()), currentCycle,
                                startDate),
                            new ChartData("Soft Drinks",
                                allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x => x.quantity)/
                                Convert.ToDecimal(totalOrderIdsForThoseItems.Count()), currentCycle, startDate),
                            new ChartData("Alcohol",
                                allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x => x.quantity)/Convert.ToDecimal(totalOrderIdsForThoseItems.Count()),
                                currentCycle, startDate),
                            new ChartData("Others",
                                       allItemsAsCategoryBreakdowns.OtherItems.Sum(x => x.quantity)/Convert.ToDecimal(totalOrderIdsForThoseItems.Count()), currentCycle,
                                startDate)
                                    }
                                });
                }
                catch (Exception)
                {


                }


                var hotDrinksPercentage = OrderItemReportFactory.GetItemsAsPercentageSalesOfTotalOrders(allItemsAsCategoryBreakdowns.HotDrinkItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems, totalOrderIdsForThoseItems.Count()) * averageOrderSizeInPounds / 100.00M;
                var foodPercentage = OrderItemReportFactory.GetItemsAsPercentageSalesOfTotalOrders(allItemsAsCategoryBreakdowns.FoodItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems, totalOrderIdsForThoseItems.Count()) * averageOrderSizeInPounds / 100.00M;
                var softDrinksPercentage = OrderItemReportFactory.GetItemsAsPercentageSalesOfTotalOrders(allItemsAsCategoryBreakdowns.SoftDrinkItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems, totalOrderIdsForThoseItems.Count()) * averageOrderSizeInPounds / 100.00M;
                var alcoholPercentage = OrderItemReportFactory.GetItemsAsPercentageSalesOfTotalOrders(allItemsAsCategoryBreakdowns.AlcoholItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems, totalOrderIdsForThoseItems.Count()) * averageOrderSizeInPounds / 100.00M;
                var otherPercentage = OrderItemReportFactory.GetItemsAsPercentageSalesOfTotalOrders(allItemsAsCategoryBreakdowns.OtherItems.Sum(x => x.pure_sales), totalSalesFigureForAllItems, totalOrderIdsForThoseItems.Count()) * averageOrderSizeInPounds / 100.00M;

                // FINAL TABLE - Average Order Value -NEED THE ABOVE TO CALC
                try
                {
                    table_AverageOrderValue.Add(
                                new ChartWeekItemSales
                                {
                                    CategoryName = currentCycle.ToString(),
                                    WeekDateStart = startDate.ToString(),
                                    ChartData = new List<ChartData>
                                    {

                            new ChartData("Hot Drinks", hotDrinksPercentage, currentCycle, startDate),
                            new ChartData("Food", foodPercentage, currentCycle, startDate),
                            new ChartData("Soft Drinks", softDrinksPercentage, currentCycle, startDate),
                            new ChartData("Alcohol", alcoholPercentage, currentCycle, startDate),
                            new ChartData("Others", otherPercentage, currentCycle, startDate)
                                    }
                                });
                }
                catch (Exception)
                {


                }

                totalPureSalesForAllItems.Add(
                new ChartData("Weekly Sales", totalSalesFigureForAllItems, currentCycle, startDate));

                numberOfOrdersPerWeek.Add(
                    new ChartData("Number Of Orders", Convert.ToDecimal(totalOrderIdsForThoseItems.Count()), currentCycle, startDate));


                allData.Add(new ChartData("Hot Drinks", hotDrinksPercentage, currentCycle, startDate));
                allData.Add(new ChartData("Food", foodPercentage, currentCycle, startDate));
                allData.Add(new ChartData("Soft Drinks", softDrinksPercentage, currentCycle, startDate));
                allData.Add(new ChartData("Alcohol", alcoholPercentage, currentCycle, startDate));
                allData.Add(new ChartData("Others", otherPercentage, currentCycle, startDate));



                currentCycle++;
            }


            var itemTypes = new List<string>
            {
                "Hot Drinks",
                "Food",
                "Soft Drinks",
                "Alcohol",
                "Others"
            };

            return new StackedChartItemGrouping
            {
                NoOfWeeks = 3,
                Data = allData,
                Types = itemTypes
            };
        }





        public ActionResult DaliesSinglePage()
        {
            ViewBag.EstablishmentsSelect =
               db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DaliesSinglePage(int EstablishmentId = 1)
        {
            var currentDate = DateTime.Now;
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 5)
            {
                currentDate = currentDate.AddDays(-1);

            }


            var comparisonDateOneWeekBack = currentDate.AddDays(-7);
            var comparisonDateTwoWeeksBack = comparisonDateOneWeekBack.AddDays(-7);
            var comparisonDateThreeWeeksBack = comparisonDateTwoWeeksBack.AddDays(-7);
            var comparisonDateFourWeeksBack = comparisonDateThreeWeeksBack.AddDays(-7);
            var comparisonDateFiveWeeksBack = comparisonDateFourWeeksBack.AddDays(-7);

            //get all items for each date

            var est = db.Establishments.First(x => x.establishment_id == EstablishmentId);
            var prodCats = db.ProductCategories.Where(x => x.establishment_id == EstablishmentId).ToList();
            var prodClasses = db.ProductClasses.ToList();
            var users = db.Users.ToList();

            var currentDateStart = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 02, 00, 00);
            var currentDateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute, currentDate.Second);

            var comparisonDateOneWeekBackStart = new DateTime(comparisonDateOneWeekBack.Year, comparisonDateOneWeekBack.Month, comparisonDateOneWeekBack.Day, 02, 00, 00);
            var comparisonDateOneWeekBackEnd = new DateTime(comparisonDateOneWeekBack.Year, comparisonDateOneWeekBack.Month, comparisonDateOneWeekBack.Day, comparisonDateOneWeekBack.Hour, comparisonDateOneWeekBack.Minute, comparisonDateOneWeekBack.Second);

            var comparisonDateTwoWeeksBackStart = new DateTime(comparisonDateTwoWeeksBack.Year, comparisonDateTwoWeeksBack.Month, comparisonDateTwoWeeksBack.Day, 02, 00, 00);
            var comparisonDateTwoWeeksBackEnd = new DateTime(comparisonDateTwoWeeksBack.Year, comparisonDateTwoWeeksBack.Month, comparisonDateTwoWeeksBack.Day, comparisonDateTwoWeeksBack.Hour, comparisonDateTwoWeeksBack.Minute, comparisonDateTwoWeeksBack.Second);

            var comparisonDateThreeWeeksBackStart = new DateTime(comparisonDateThreeWeeksBack.Year, comparisonDateThreeWeeksBack.Month, comparisonDateThreeWeeksBack.Day, 02, 00, 00);
            var comparisonDateThreeWeeksBackEnd = new DateTime(comparisonDateThreeWeeksBack.Year, comparisonDateThreeWeeksBack.Month, comparisonDateThreeWeeksBack.Day, comparisonDateThreeWeeksBack.Hour, comparisonDateThreeWeeksBack.Minute, comparisonDateThreeWeeksBack.Second);

            var comparisonDateFourWeeksBackStart = new DateTime(comparisonDateFourWeeksBack.Year, comparisonDateFourWeeksBack.Month, comparisonDateFourWeeksBack.Day, 02, 00, 00);
            var comparisonDateFourWeeksBackEnd = new DateTime(comparisonDateFourWeeksBack.Year, comparisonDateFourWeeksBack.Month, comparisonDateFourWeeksBack.Day, comparisonDateFourWeeksBack.Hour, comparisonDateFourWeeksBack.Minute, comparisonDateFourWeeksBack.Second);

            var comparisonDateFiveWeeksBackStart = new DateTime(comparisonDateFiveWeeksBack.Year, comparisonDateFiveWeeksBack.Month, comparisonDateFiveWeeksBack.Day, 02, 00, 00);
            var comparisonDateFiveWeeksBackEnd = new DateTime(comparisonDateFiveWeeksBack.Year, comparisonDateFiveWeeksBack.Month, comparisonDateFiveWeeksBack.Day, comparisonDateFiveWeeksBack.Hour, comparisonDateFiveWeeksBack.Minute, comparisonDateFiveWeeksBack.Second);


            //THIS WEEK
            IList<OrderItem> todayCompsAndVoids = new List<OrderItem>();
            IList<Product> todaysProducts = new List<Product>();
            var todaysItems = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db,
                prodCats, currentDateStart, currentDateEnd, out todayCompsAndVoids, out todaysProducts);

            //ONE WEEK BACK
            IList<OrderItem> comparisonDateOneWeekBackCompsAndVoids = new List<OrderItem>();
            IList<Product> comparisonDateOneWeekBackProducts = new List<Product>();
            var comparisonDateOneWeekBackItems = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db,
                prodCats, comparisonDateOneWeekBackStart, comparisonDateOneWeekBackEnd, out comparisonDateOneWeekBackCompsAndVoids, out comparisonDateOneWeekBackProducts);

            //TWO WEEKS BACK
            IList<OrderItem> comparisonDateTwoWeekBackCompsAndVoids = new List<OrderItem>();
            IList<Product> comparisonDateTwoWeekBackProducts = new List<Product>();
            var comparisonDateTwoWeekBackItems = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db,
                prodCats, comparisonDateTwoWeeksBackStart, comparisonDateTwoWeeksBackEnd, out comparisonDateTwoWeekBackCompsAndVoids, out comparisonDateTwoWeekBackProducts);

            //THREE WEEKS BACK
            IList<OrderItem> comparisonDateThreeWeekBackCompsAndVoids = new List<OrderItem>();
            IList<Product> comparisonDateThreWeekBackProducts = new List<Product>();
            var comparisonDateThreeWeekBackItems = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db,
                prodCats, comparisonDateThreeWeeksBackStart, comparisonDateThreeWeeksBackEnd, out comparisonDateThreeWeekBackCompsAndVoids, out comparisonDateThreWeekBackProducts);

            //FOUR WEEKS BACK
            IList<OrderItem> comparisonDateFourWeekBackCompsAndVoids = new List<OrderItem>();
            IList<Product> comparisonDateFourWeekBackProducts = new List<Product>();
            var comparisonDateFourWeekBackItems = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db,
                prodCats, comparisonDateFourWeeksBackStart, comparisonDateFourWeeksBackEnd, out comparisonDateFourWeekBackCompsAndVoids, out comparisonDateFourWeekBackProducts);


            //FIVE WEEKS BACK
            IList<OrderItem> comparisonDateFiveWeekBackCompsAndVoids = new List<OrderItem>();
            IList<Product> comparisonDateFiveWeekBackProducts = new List<Product>();
            var comparisonDateFiveWeekBackItems = OrderItemReportFactory.GetFullOrderItemDataSetWithSplitBillsAndVoidsAndCompsExcluded(db,
                prodCats, comparisonDateFiveWeeksBackStart, comparisonDateFiveWeeksBackEnd, out comparisonDateFiveWeekBackCompsAndVoids, out comparisonDateFiveWeekBackProducts);


            //GOT ITEMS 

            ////////////////
            // CHARTS
            ////////////////
            //HOUR AND SPEND
            var currentHourAndSpend = await OrderItemReportFactory.GetHourlySalesForOrderItems(currentDateStart,
                 currentDateEnd, todaysItems.ToList().ToOrderItemReportingItems());

            var comparisonOneWeekBackHourAndSpend = await OrderItemReportFactory.GetHourlySalesForOrderItems(comparisonDateOneWeekBackStart,
                            comparisonDateOneWeekBackEnd, comparisonDateOneWeekBackItems.ToList().ToOrderItemReportingItems());

            var comparisonTwoWeeksBackHourAndSpend = await OrderItemReportFactory.GetHourlySalesForOrderItems(comparisonDateTwoWeeksBackStart,
                            comparisonDateTwoWeeksBackEnd, comparisonDateTwoWeekBackItems.ToList().ToOrderItemReportingItems());

            var comparisonTHreeWeeksBackHourAndSpend = await OrderItemReportFactory.GetHourlySalesForOrderItems(comparisonDateThreeWeeksBackStart,
                            comparisonDateThreeWeeksBackEnd, comparisonDateThreeWeekBackItems.ToList().ToOrderItemReportingItems());

            var comparisonFourWeeksBackHourAndSpend = await OrderItemReportFactory.GetHourlySalesForOrderItems(comparisonDateFourWeeksBackStart,
                            comparisonDateFourWeeksBackEnd, comparisonDateFourWeekBackItems.ToList().ToOrderItemReportingItems());

            var comparisonFiveWeeksBackHourAndSpend = await OrderItemReportFactory.GetHourlySalesForOrderItems(comparisonDateFiveWeeksBackStart,
                            comparisonDateFiveWeeksBackEnd, comparisonDateFiveWeekBackItems.ToList().ToOrderItemReportingItems());


            ViewBag.CurrentHourAndSpend =
                currentHourAndSpend.Select(
                    x =>
                        new ChartData(x.Hour.ToString(), x.Value, 1, currentDateStart)).ToList();

            ViewBag.ComparisonHourAndSpend =
                comparisonOneWeekBackHourAndSpend.Select(
                    x =>
                        new ChartData(x.Hour.ToString(), x.Value, 1, comparisonDateOneWeekBackStart)).ToList();

            ViewBag.ComparisonTwoWeeksBackHourAndSpend =
               comparisonTwoWeeksBackHourAndSpend.Select(
                   x =>
                       new ChartData(x.Hour.ToString(), x.Value, 1, comparisonDateTwoWeeksBackStart)).ToList();

            ViewBag.comparisonTHreeWeeksBackHourAndSpend =
             comparisonTHreeWeeksBackHourAndSpend.Select(
                 x =>
                     new ChartData(x.Hour.ToString(), x.Value, 1, comparisonDateThreeWeeksBackStart)).ToList();


            ViewBag.comparisonFourWeeksBackHourAndSpend =
             comparisonFourWeeksBackHourAndSpend.Select(
                 x =>
                     new ChartData(x.Hour.ToString(), x.Value, 1, comparisonDateFourWeeksBackStart)).ToList();


            ViewBag.comparisonFiveWeeksBackHourAndSpend =
             comparisonFiveWeeksBackHourAndSpend.Select(
                 x =>
                     new ChartData(x.Hour.ToString(), x.Value, 1, comparisonDateFiveWeeksBackStart)).ToList();

            //END HOUR AND SPEND
            /////////////////////

            //SALES VS LAST WEEK
            var salesTodayAsItemBreakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(todaysItems, est,
                todaysProducts, prodClasses);

            var allData = new List<ChartData>();

            allData.AddRange(new List<ChartData>
            {
                new ChartData("Hot Drinks", salesTodayAsItemBreakdown.HotDrinkItems.Sum(x => x.pure_sales), 6,
                    currentDateStart),
                new ChartData("Food", salesTodayAsItemBreakdown.FoodItems.Sum(x => x.pure_sales), 6,
                    currentDateStart),
                new ChartData("Soft Drinks", salesTodayAsItemBreakdown.SoftDrinkItems.Sum(x => x.pure_sales), 6,
                    currentDateStart),
                new ChartData("Alcohol", salesTodayAsItemBreakdown.AlcoholItems.Sum(x => x.pure_sales), 6,
                    currentDateStart),
                new ChartData("Others", salesTodayAsItemBreakdown.OtherItems.Sum(x => x.pure_sales), 6,
                    currentDateStart)
            });

            //1 week back
            var salesLastWeekAsItemBreakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(comparisonDateOneWeekBackItems, est,
               comparisonDateOneWeekBackProducts, prodClasses);
            allData.AddRange(new List<ChartData>
            {
                new ChartData("Hot Drinks", salesLastWeekAsItemBreakdown.HotDrinkItems.Sum(x => x.pure_sales), 5,
                    comparisonDateOneWeekBackStart),
                new ChartData("Food", salesLastWeekAsItemBreakdown.FoodItems.Sum(x => x.pure_sales), 5,
                    comparisonDateOneWeekBackStart),
                new ChartData("Soft Drinks", salesLastWeekAsItemBreakdown.SoftDrinkItems.Sum(x => x.pure_sales), 5,
                    comparisonDateOneWeekBackStart),
                new ChartData("Alcohol", salesLastWeekAsItemBreakdown.AlcoholItems.Sum(x => x.pure_sales), 5,
                    comparisonDateOneWeekBackStart),
                new ChartData("Others", salesLastWeekAsItemBreakdown.OtherItems.Sum(x => x.pure_sales), 5,
                    comparisonDateOneWeekBackStart)
            });

            //2 weeks back
            var salesTwoWeeksBackAsItemBreakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(comparisonDateTwoWeekBackItems, est,
               comparisonDateTwoWeekBackProducts, prodClasses);
            allData.AddRange(new List<ChartData>
            {
                new ChartData("Hot Drinks", salesTwoWeeksBackAsItemBreakdown.HotDrinkItems.Sum(x => x.pure_sales), 4,
                    comparisonDateTwoWeeksBackStart),
                new ChartData("Food", salesTwoWeeksBackAsItemBreakdown.FoodItems.Sum(x => x.pure_sales), 4,
                    comparisonDateTwoWeeksBackStart),
                new ChartData("Soft Drinks", salesTwoWeeksBackAsItemBreakdown.SoftDrinkItems.Sum(x => x.pure_sales), 4,
                    comparisonDateTwoWeeksBackStart),
                new ChartData("Alcohol", salesTwoWeeksBackAsItemBreakdown.AlcoholItems.Sum(x => x.pure_sales), 4,
                    comparisonDateTwoWeeksBackStart),
                new ChartData("Others", salesTwoWeeksBackAsItemBreakdown.OtherItems.Sum(x => x.pure_sales), 4,
                    comparisonDateTwoWeeksBackStart)
            });

            //3 weeks back
            var salesThreeWeeksBackAsItemBreakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(comparisonDateThreeWeekBackItems, est,
               comparisonDateThreWeekBackProducts, prodClasses);
            allData.AddRange(new List<ChartData>
            {
                new ChartData("Hot Drinks", salesThreeWeeksBackAsItemBreakdown.HotDrinkItems.Sum(x => x.pure_sales), 3,
                    comparisonDateThreeWeeksBackStart),
                new ChartData("Food", salesThreeWeeksBackAsItemBreakdown.FoodItems.Sum(x => x.pure_sales), 3,
                    comparisonDateThreeWeeksBackStart),
                new ChartData("Soft Drinks", salesThreeWeeksBackAsItemBreakdown.SoftDrinkItems.Sum(x => x.pure_sales), 3,
                    comparisonDateThreeWeeksBackStart),
                new ChartData("Alcohol", salesThreeWeeksBackAsItemBreakdown.AlcoholItems.Sum(x => x.pure_sales), 3,
                    comparisonDateThreeWeeksBackStart),
                new ChartData("Others", salesThreeWeeksBackAsItemBreakdown.OtherItems.Sum(x => x.pure_sales), 3,
                    comparisonDateThreeWeeksBackStart)
            });

            //4 weeks back
            var salesFourWeeksBackAsItemBreakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(comparisonDateFourWeekBackItems, est,
               comparisonDateFourWeekBackProducts, prodClasses);
            allData.AddRange(new List<ChartData>
            {
                new ChartData("Hot Drinks", salesFourWeeksBackAsItemBreakdown.HotDrinkItems.Sum(x => x.pure_sales), 2,
                    comparisonDateFourWeeksBackStart),
                new ChartData("Food", salesFourWeeksBackAsItemBreakdown.FoodItems.Sum(x => x.pure_sales), 2,
                    comparisonDateFourWeeksBackStart),
                new ChartData("Soft Drinks", salesFourWeeksBackAsItemBreakdown.SoftDrinkItems.Sum(x => x.pure_sales), 2,
                    comparisonDateFourWeeksBackStart),
                new ChartData("Alcohol", salesFourWeeksBackAsItemBreakdown.AlcoholItems.Sum(x => x.pure_sales), 2,
                    comparisonDateFourWeeksBackStart),
                new ChartData("Others", salesFourWeeksBackAsItemBreakdown.OtherItems.Sum(x => x.pure_sales), 2,
                    comparisonDateFourWeeksBackStart)
            });

            //5 weeks back
            var salesFiveWeeksBackAsItemBreakdown = OrderItemReportFactory.GetOrderItemTypeCategoryBreakdowns(comparisonDateFiveWeekBackItems, est,
               comparisonDateFiveWeekBackProducts, prodClasses);
            allData.AddRange(new List<ChartData>
            {
                new ChartData("Hot Drinks", salesFiveWeeksBackAsItemBreakdown.HotDrinkItems.Sum(x => x.pure_sales), 1,
                    comparisonDateFiveWeeksBackStart),
                new ChartData("Food", salesFiveWeeksBackAsItemBreakdown.FoodItems.Sum(x => x.pure_sales), 1,
                    comparisonDateFiveWeeksBackStart),
                new ChartData("Soft Drinks", salesFiveWeeksBackAsItemBreakdown.SoftDrinkItems.Sum(x => x.pure_sales), 1,
                    comparisonDateFiveWeeksBackStart),
                new ChartData("Alcohol", salesFiveWeeksBackAsItemBreakdown.AlcoholItems.Sum(x => x.pure_sales), 1,
                    comparisonDateFiveWeeksBackStart),
                new ChartData("Others", salesFiveWeeksBackAsItemBreakdown.OtherItems.Sum(x => x.pure_sales), 1,
                    comparisonDateFiveWeeksBackStart)
            });

            var itemTypes = new List<string>
            {
                "Hot Drinks",
                "Food",
                "Soft Drinks",
                "Alcohol",
                "Others"
            };

            ViewBag.SalesTodayStackedColumnData = new StackedChartItemGrouping
            {
                NoOfWeeks = 6,
                Data = allData.OrderBy(x => x.WeekStart).ToList(),
                Types = itemTypes
            };

            //END SALES VS LAST WEEK

            //AVG SPEND
            var currentTotalSpend = new decimal();
            var currentAvgSpendPerCat =
                OrderItemReportFactory.GetCategoryPercentagesOfTotalOrderItems(todaysItems, est, todaysProducts, prodClasses,
                    out currentTotalSpend);
            //1 week
            var comparisonTotalSpend = new decimal();
            var comparisonAvgSpendPerCat =
                OrderItemReportFactory.GetCategoryPercentagesOfTotalOrderItems(comparisonDateOneWeekBackItems, est, comparisonDateOneWeekBackProducts, prodClasses,
                    out comparisonTotalSpend);

            //2 weeks
            var comparisonTotalSpendWeek2 = new decimal();
            var comparisonAvgSpendPerCatWeek2 =
                OrderItemReportFactory.GetCategoryPercentagesOfTotalOrderItems(comparisonDateTwoWeekBackItems, est, comparisonDateTwoWeekBackProducts, prodClasses,
                    out comparisonTotalSpendWeek2);

            //3 weeks
            var comparisonTotalSpendWeek3 = new decimal();
            var comparisonAvgSpendPerCatWeek3 =
                OrderItemReportFactory.GetCategoryPercentagesOfTotalOrderItems(comparisonDateThreeWeekBackItems, est, comparisonDateThreWeekBackProducts, prodClasses,
                    out comparisonTotalSpendWeek3);

            //4 weeks
            var comparisonTotalSpendWeek4 = new decimal();
            var comparisonAvgSpendPerCatWeek4 =
                OrderItemReportFactory.GetCategoryPercentagesOfTotalOrderItems(comparisonDateFourWeekBackItems, est, comparisonDateFourWeekBackProducts, prodClasses,
                    out comparisonTotalSpendWeek4);

            //5 weeks
            var comparisonTotalSpendWeek5 = new decimal();
            var comparisonAvgSpendPerCatWeek5 =
                OrderItemReportFactory.GetCategoryPercentagesOfTotalOrderItems(comparisonDateFiveWeekBackItems, est, comparisonDateFiveWeekBackProducts, prodClasses,
                    out comparisonTotalSpendWeek5);


            var allAvgSpendData = new List<ChartData>();
            foreach (var spend in currentAvgSpendPerCat)
            {
                allAvgSpendData.Add(new ChartData(spend.Key, spend.Value, 1, currentDate));
            }

            foreach (var spend in comparisonAvgSpendPerCat)
            {
                allAvgSpendData.Add(new ChartData(spend.Key, spend.Value, 2, comparisonDateOneWeekBack));
            }

            foreach (var spend in comparisonAvgSpendPerCatWeek2)
            {
                allAvgSpendData.Add(new ChartData(spend.Key, spend.Value, 3, comparisonDateTwoWeeksBack));
            }

            foreach (var spend in comparisonAvgSpendPerCatWeek3)
            {
                allAvgSpendData.Add(new ChartData(spend.Key, spend.Value, 4, comparisonDateThreeWeeksBack));
            }

            foreach (var spend in comparisonAvgSpendPerCatWeek4)
            {
                allAvgSpendData.Add(new ChartData(spend.Key, spend.Value, 5, comparisonDateFourWeeksBack));
            }

            foreach (var spend in comparisonAvgSpendPerCatWeek5)
            {
                allAvgSpendData.Add(new ChartData(spend.Key, spend.Value, 6, comparisonDateFiveWeeksBack));
            }


            ViewBag.SalesAverageSpendByCategoryChart = new StackedChartItemGrouping
            {
                NoOfWeeks = 6,
                Data = allAvgSpendData.OrderBy(x => x.WeekStart).ToList(),
                Types = itemTypes
            };


            //END SALES VS LAST WEEK
            //END CHARTS


            //TABLES


            //TOP SELLERS
            var topSellers = new List<TopSeller>();

            ViewBag.TopSellers = topSellers.Concat(await OrderItemReportFactory.GetTopSellers(todaysItems.ToList(), users, prodClasses, todaysProducts, est)).ToList();


            //  ViewBag.TopSeller
            //END TOP SELLERS

            //TOP PRODUCTS 

            var factory = new OrderItemReportFactory();

            var currentOrderItemsOutPut = new List<OrderItem>();
            var currentReportData = new List<ProductOrderItemSummary>();
            currentReportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
            {
                StartDate = currentDateStart,
                EndDate = currentDateEnd,
                NoOfDaysInEachReportingPeriod = 1,
                IdOfStore = est.establishment_id
            }, new GrindContext(), out currentOrderItemsOutPut, "");

            var comparisonOrderItemsOutPut = new List<OrderItem>();
            var comparisontReportData = new List<ProductOrderItemSummary>();
            comparisontReportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
            {
                StartDate = comparisonDateOneWeekBackStart,
                EndDate = comparisonDateOneWeekBackEnd,
                NoOfDaysInEachReportingPeriod = 1,
                IdOfStore = est.establishment_id
            }, new GrindContext(), out comparisonOrderItemsOutPut, "");

            //filter to only the products we want from product watch
            var productsForThisEst = db.Products.AsNoTracking()
                .Where(x => x.establishment_id == EstablishmentId).ToList();
            var productIdsForEstablishment = productsForThisEst.Select(x => x.product_id).ToList();
            var prodWatches = db.ProductWatches.AsNoTracking().ToList()
                .Where(x => productIdsForEstablishment.Any(anId => anId == x.Revel_Product_Id))
                .Select(x => x.Revel_Product_Id).ToList();


            ViewBag.TopProducts = new OrderItemProductGroupingComparisonList
            {
                Items = currentReportData.Where(x => prodWatches.Any(anId => anId == x.ProductId)).ToList(),
                ComparisonItems = comparisontReportData.Where(x => prodWatches.Any(anId => anId == x.ProductId)).ToList()
            };


            //VARS FOR UI
            ViewBag.DateShowing = currentDate;
            ViewBag.HasFired = "true";
            ViewBag.EstablishmentsSelect =
              db.Establishments.Where(x => x.establishment_id != 2).ToList().Select(x => new SelectListItem { Value = x.id, Text = x.name }).ToList();

            ViewBag.Establishment = db.Establishments.First(x => x.establishment_id == EstablishmentId).name;
            ViewBag.Message = "Please wait while real-time BI is calculated";
            return View();

        }
    }
}
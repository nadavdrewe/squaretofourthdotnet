using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using GeckoboardLibrary.Classes.WidgetItems;
using GeckoboardLibrary.Classes.Widgets;
using GeckoboardLibrary.Services;
using GeckoboardTestWebApp.Controllers;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.BusinessServices;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes.Reporting.ReportingFactory;
using Revel._808nd.com.Classes.ServiceImplemenations;

namespace GeckoboardTestWebApp.Models
{
    public class WidgetSetFactory
    {


        public EstablishmentBindingTable GetURLBindingSetForWidgetSetA(WidgetSetA widgetSetA)
        {
            try
            {
                EstablishmentBindingTable estbTable = new EstablishmentBindingTable(widgetSetA.RevelEstablishment.establishment_id);
                return estbTable;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        /// <summary>
        /// widgets must exist in the WidgetSetA to be bound to!
        /// </summary>
        /// <param name="aWidgetSetA"></param>
        public bool BindWidgetData(WidgetSetA aWidgetSetA)
        {



            EstablishmentBindingTable establishmentBindingTable = GetURLBindingSetForWidgetSetA(aWidgetSetA);
            aWidgetSetA.WidgetURLBindings = establishmentBindingTable;

            foreach (var widget in aWidgetSetA.theWidgetCollection)
            {




            }


            return true;
            //which data goes into which widget//push

            //bind type - each widget has interface Bindable
            //each widget implements bindable and provides it's own data type

            //the datasets and the typeID/enum

            //just use the object creator in 




            //   widgetID = DataSet

            //widgets are all of the same org so there's only 8 to bind



        }


        public async Task<bool> Push24WeekOrderItemWidget(GrindContext db, WidgetSetA widgetSetA,
            IList<OrderItem> last24weeksOrderItems, IEnumerable<ProductClass> allProductClasses)
        {
            IList<Product> tempErrors = new List<Product>();
            IList<Product> errors = new List<Product>();

            var prodClasses = db.ProductClasses.ToList();
            var url = "";
            widgetSetA.WidgetURLBindings = GetURLBindingSetForWidgetSetA(widgetSetA);

            //create prods
            widgetSetA.pcWrapper = new RevelProductAndCategoryWrapper();
            await widgetSetA.revelFactory.CreateProductsAndCategories(widgetSetA.pcWrapper);

            widgetSetA.alcoholProducts = widgetSetA.pcWrapper.GetProductsThatAreAlcoholByClass(allProductClasses, out tempErrors);
            widgetSetA.foodProducts = widgetSetA.pcWrapper.GetProductsThatAreFoodByClass(allProductClasses, out tempErrors);
            widgetSetA.hotDrinkProducts = widgetSetA.pcWrapper.GetProductsThatAreHotDrinksByClass(allProductClasses, out tempErrors);


            //create the graph from the buckets
            List<Establishment> establishmentForIdentificationService = new List<Establishment>
             {
                 widgetSetA.RevelEstablishment
             };
            var indentificationService = new RevelProductAndCategoryWrapper(establishmentForIdentificationService);
            indentificationService.Initialise(db, allProductClasses);

            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(108, out url);
            var widget = new LineV2Widget("ab876212d31d37960e3154eb5e2bc0a0",
                url, "test",
                GeckoboardChartAndItemType.LineV2, 11);

            var _24WeekWidget = LineChartV2RevelFactory.Initialise24WeekLineV2WidgetData(widget, indentificationService,
                establishmentForIdentificationService, last24weeksOrderItems, prodClasses);


            var push = new GeckoboardPushService();
            var ok = await push.Push(_24WeekWidget);

            return true;
        }

        public async Task<ParentWidgetSet> InitialiseOvernightParentWidgetSet(ParentWidgetSet parentWidgetSet, IEnumerable<OrderItem> last24weeksOrderItems)
        {
            var db = new GrindContext();
            try
            {
                parentWidgetSet.WidgetURLBindings = GetURLBindingSetForWidgetSetA(parentWidgetSet);

                string url;

                List<GeckoboardObject> allTheWidgets = new List<GeckoboardObject>();





                //101

                //dates
                var lastWeekStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).LastWeekStart;
                var lastWeekEnd = parentWidgetSet.AllChildWidgetSets.ElementAt(0).LastWeekEnd;

                var weekbeforeLastStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).WeekBeforeLastStart;
                var weekbeforeLastEnd = parentWidgetSet.AllChildWidgetSets.ElementAt(0).WeekBeforeLastEnd;

                int LastWeekGross = (int)db.Payments
                    .AsNoTracking()
                    .Where(x => x.created_date >= lastWeekStart)
                    .Where(x => x.created_date <= lastWeekEnd)
                    .Select(x => x.amount)
                    .Sum();

                int WeekBeforeLastGross = (int)db.Payments
                    .AsNoTracking()
                    .Where(x => x.created_date >= weekbeforeLastStart)
                    .Where(x => x.created_date <= weekbeforeLastEnd)
                     .Select(x => x.amount)
                    .Sum();

                //widgets
                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(101, out url);



                NumberSecondaryStat LastWeekNetVSweekBeforeNet = parentWidgetSet.factory.CreateNumberSecondaryStat(101, "LastWeekNetVSweekBeforeNet",
                url, "LastCompleteWeekNet £", LastWeekGross,
                "CompleteWeekBeforeNet", WeekBeforeLastGross);
                allTheWidgets.Add(LastWeekNetVSweekBeforeNet);


                //102

                //dates
                var yesterdayStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).yesterday;
                var yesterdayEnd = yesterdayStart.AddDays(1);

                var yesterdayLastWeekStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).yesterDayLastWeek;
                var yesterdayLastWeekEnd = yesterdayLastWeekStart.AddDays(1);

                int yesterSalesGross = 0;

                try
                {
                    yesterSalesGross = (int)db.Payments
                    .Where(x => x.created_date >= yesterdayStart)
                    .Where(x => x.created_date <= yesterdayEnd)
                    .Select(x => x.amount)
                    .Sum();
                }
                catch (Exception)
                {


                }


                int yesterLastweekSalesGross = 0;

                try
                {
                    yesterLastweekSalesGross = (int)db.Payments
                    .Where(x => x.created_date >= yesterdayLastWeekStart)
                    .Where(x => x.created_date <= yesterdayLastWeekEnd)
                      .Select(x => x.amount)
                    .Sum();
                }
                catch (Exception)
                {


                }



                //widgets
                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(102, out url);

                NumberSecondaryStat yesterdayVSYesterdayLastWeek = parentWidgetSet.factory.CreateNumberSecondaryStat(102, "yesterdayVSYesterdayLastWeek",
                   url, "Yesterday £", yesterSalesGross,
                   "Yesterday Last Week", yesterLastweekSalesGross);

                allTheWidgets.Add(yesterdayVSYesterdayLastWeek);



                //103
                //dates
                var lastMonthStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).FirstDayOfLastMonth;
                var lastMonthEnd = parentWidgetSet.AllChildWidgetSets.ElementAt(0).FirstDayOfThisMonth;

                int lastMonthSalesNet = (int)db.Payments
                    .Where(x => x.created_date >= lastMonthStart)
                    .Where(x => x.created_date <= lastMonthEnd)
                      .Select(x => x.amount)
                    .Sum();

                int lastMonthTax = (int)db.Orders
                    .Where(x => x.created_date >= lastMonthStart)
                    .Where(x => x.created_date <= lastMonthEnd)
                     .Select(x => x.tax)
                    .Sum();



                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(103, out url);

                //get budget for correct month/venue
                int currentMonth = DateTime.Now.Month;

                //get summed budgets
                var budget = 0;
                try
                {
                    budget = parentWidgetSet.AllChildWidgetSets.Sum(a => a.RevelEstablishment.AnnualBudget[currentMonth]);
                }
                catch (Exception)
                {

                }


                NumberSecondaryStat lastMonthVSBudget = parentWidgetSet.factory.CreateNumberSecondaryStat(103, "LastWeekNetVSweekBeforeNet",
                 url, "Last Month £", (int)(lastMonthSalesNet - lastMonthTax),
                 "Budget", budget);

                allTheWidgets.Add(lastMonthVSBudget);


                //104
                var previousMonthStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).FirstDayOfLastMonth;
                var previousMonthEnd = parentWidgetSet.AllChildWidgetSets.ElementAt(0).FirstDayOfThisMonth;

                var sameMonthLastYearStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).FirstDayOfLastMonthLastYear;
                var sameMonthLastYearEnd = parentWidgetSet.AllChildWidgetSets.ElementAt(0).FirstDayOfThisMonthLastYear;

                var currentMonthlyPayment = (int)db.Payments
                    .Where(x => x.created_date >= previousMonthStart)
                    .Where(x => x.created_date <= previousMonthEnd)
                    .Select(x => x.amount)
                    .Sum();

                var currentMonthlyTax = (int)db.Orders
                    .Where(x => x.created_date >= previousMonthStart)
                    .Where(x => x.created_date <= previousMonthEnd)
                     .Select(x => x.tax)
                    .Sum();




                var sameMonthLastYearPayment = 0.00M;
                var sameMonthLastYearTax = 0.00M;

                if (db.Payments
                    .Where(x => x.created_date >= sameMonthLastYearStart)
                    .Where(x => x.created_date <= sameMonthLastYearStart)
                    .Any())
                {
                    sameMonthLastYearPayment = (int)db.Payments
                        .Where(x => x.created_date >= sameMonthLastYearStart)
                        .Where(x => x.created_date <= sameMonthLastYearStart)
                        .Select(x => x.amount)
                    .Sum();

                    sameMonthLastYearTax = (int)db.Orders
                        .Where(x => x.created_date >= sameMonthLastYearStart)
                        .Where(x => x.created_date <= sameMonthLastYearEnd)
                        .Select(x => x.tax)
                    .Sum();

                }

                var currentMonthly = currentMonthlyPayment - currentMonthlyTax;

                var sameMonthLastYear = sameMonthLastYearPayment - sameMonthLastYearTax;

                var monthBudget = 0;

                try
                {
                    monthBudget = parentWidgetSet.AllChildWidgetSets.Sum(a => a.RevelEstablishment.AnnualBudget[currentMonth]);
                }
                catch (Exception)
                {


                }



                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(104, out url);


                BulletItem item = new BulletItem
                {
                    label = "Last Month(NET£)",
                    sublabel = "vs Last Month Last Year(NET£) and Budget (Green)",

                    axis = new BulletAxis
                    {
                        point = new List<string>
                       {
                          "0",(monthBudget * 0.25).ToString(),(monthBudget * 0.50).ToString(),(monthBudget * 0.75).ToString(),(monthBudget * 1.0).ToString(),(monthBudget * 1.25).ToString()
                       }
                    },

                    range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=monthBudget-1},
                               new BulletRange{color="green", start=monthBudget, end=Convert.ToInt32(monthBudget * 1.25)}

                   },

                    measure = new BulletMeasure
                    {
                        current = new BulletMeasureItem { start = "0", end = currentMonthly.ToString() },
                        projected = new BulletMeasureItem { start = "0", end = "0" }

                    },

                    comparative = new BulletComparative { point = sameMonthLastYear.ToString() }
                };

                var BulletMonthlyBudget = parentWidgetSet.factory.CreateBullet(104, "BulletMonthlyBudget", url, "horizontal", item);

                allTheWidgets.Add(BulletMonthlyBudget);

                //105
                var thisYearStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThisYearStart;
                var YTDYesterday = parentWidgetSet.AllChildWidgetSets.ElementAt(0).YTDYesterday;

                var lastYearStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).LastYearStart;
                var lastYearYesterday = parentWidgetSet.AllChildWidgetSets.ElementAt(0).LastYearYesterday;

                var thisYearSumPayments = 0.00M;
                var thisYearSumTax = 0.00M;

                /*  try
                  {
                      thisYearSumPayments = db.Payments
                                  .Where(x => x.created_date >= thisYearStart)
                            .Where(x => x.created_date <= YTDYesterday)
                            .Sum(x => x.amount);

                      thisYearSumTax = db.Orders
                          .Where(x => x.created_date >= thisYearStart)
                          .Where(x => x.created_date <= YTDYesterday)
                          .Sum(x => x.tax);
                  }
                  catch (Exception)
                  {


                  }*/


                Nullable<decimal> lastYearSumPayments = 0.00M;
                Nullable<decimal> lastYearSumTax = 0.00M;

                if (db.Payments
                        .Where(x => x.created_date >= lastYearStart)
                        .Where(x => x.created_date <= lastYearYesterday)
                        .Any())
                {
                    lastYearSumPayments = db.Payments
                       .Where(x => x.created_date >= lastYearStart)
                       .Where(x => x.created_date <= lastYearYesterday)
                        .Select(x => x.amount)
                    .Sum();

                    lastYearSumTax = db.Orders
                        .Where(x => x.created_date >= lastYearStart)
                        .Where(x => x.created_date <= lastYearYesterday)
                         .Select(x => x.tax)
                    .Sum();
                }




                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(105, out url);

                NumberSecondaryStat ThisYearStartToTodayWidget = parentWidgetSet.factory.CreateNumberSecondaryStat(105, "ThisYearStartToToday",
                       url,
                       "YTDYesterday", (int)(thisYearSumPayments - thisYearSumTax), "YTD Yesterday Last Year",
                       (int)(lastYearSumPayments - lastYearSumTax));

                allTheWidgets.Add(ThisYearStartToTodayWidget);




                //106
                //dates
                var last30Start = parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayStart;
                var last30End = parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayEnd;
                var ThirtyPreviousStart = parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysPreviousStart;
                var ThirtyPreviousEnd = parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysPreviousEnd;

                var ThirtyDaysToYesterdayStartLastYear =
                    parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayStartLastYear;
                var ThirtyDaysToYesterdayEndLastYear =
                    parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayEndLastYear; ;


                //databind
                //payments
                var paymentslast30 = db.Payments
                    .Where(x => x.created_date >= last30Start)
                  .Where(x => x.created_date <= last30End)
                  .Select(x => x.amount)
                    .Sum();

                var payments30Previous = db.Payments
                    .Where(x => x.created_date >= ThirtyPreviousStart)
                  .Where(x => x.created_date <= ThirtyPreviousEnd)
                   .Select(x => x.amount)
                    .Sum();




                var paymentsThirtyToYestLastYear = 0.00M;
                if (db.Payments
                    .Where(x => x.created_date >= ThirtyDaysToYesterdayStartLastYear)
                    .Where(x => x.created_date <= ThirtyDaysToYesterdayEndLastYear)
                    .Any())
                {
                    paymentsThirtyToYestLastYear = db.Payments
                        .Where(x => x.created_date >= ThirtyDaysToYesterdayStartLastYear)
                        .Where(x => x.created_date <= ThirtyDaysToYesterdayEndLastYear)
                         .Select(x => x.amount)
                    .Sum();
                }


                //tax
                /*          var taxlast30 = db.Orders
                              .Where(x => x.created_date >= last30Start)
                            .Where(x => x.created_date <= last30End)              
                            .Sum(x => x.tax);

                          var tax30Previous =  db.Orders
                              .Where(x => x.created_date >= ThirtyPreviousStart)
                            .Where(x => x.created_date <= ThirtyPreviousEnd)              
                            .Sum(x => x.tax);

                          var taxThirtyToYestLastYear = db.Orders
                              .Where(x => x.created_date >= ThirtyDaysToYesterdayStartLastYear)
                            .Where(x => x.created_date <= ThirtyDaysToYesterdayEndLastYear)
                            .Sum(x => x.tax);*/


                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(106, out url);

                var baseLineForCharts =
                    Convert.ToInt32(paymentslast30);


                BulletItem last30Item = new BulletItem
                {
                    label = "Last 30 days (GROSS£)",
                    sublabel = "vs previous 30 days (red/green) and same 30 days last year (line)",

                    axis = new BulletAxis
                    {
                        point = new List<string>
                       {
                       "0",(baseLineForCharts * 0.25).ToString(),(baseLineForCharts * 0.50).ToString(),(baseLineForCharts * 0.75).ToString(),(baseLineForCharts * 1.0).ToString(),(baseLineForCharts * 1.25).ToString()
                       }
                    },

                    range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=Convert.ToInt32( payments30Previous)},
                       new BulletRange{color="green", start=Convert.ToInt32( payments30Previous) + 1, end=(int)(baseLineForCharts * 1.25)}

                   },

                    measure = new BulletMeasure
                    {
                        current = new BulletMeasureItem { start = "0", end = paymentslast30.ToString() },
                        projected = new BulletMeasureItem { start = "0", end = "0" }

                    },

                    comparative = new BulletComparative { point = paymentsThirtyToYestLastYear.ToString() }
                };


                var BulletLast30 = parentWidgetSet.factory.CreateBullet(106, "BulletLast30", url, "horizontal", last30Item);

                allTheWidgets.Add(BulletLast30);


                //109
                var start = new DateTime(parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayStart.Year, parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayStart.Month, parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayStart.Day, 03, 00, 00);
                var end = new DateTime(parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayEnd.Year, parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayEnd.Month, parentWidgetSet.AllChildWidgetSets.ElementAt(0).ThirtyDaysToYesterdayEnd.Day, 03, 00, 00);

                var daterange = new List<DateTime>();
                var range = new List<int>();

                var ordersItemsFromLast30 = db.OrderItems
                    .Where(x => x.created_date <= end)
                    .Where(x => x.created_date >= start);

                parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(109, out url);

                var widget109 = new LineV2Widget("ab876212d31d37960e3154eb5e2bc0a0",
                     url, "test",
                     GeckoboardChartAndItemType.LineV2, 109);


                for (DateTime i = start; i < end; i = i.AddDays(1))
                {
                    daterange.Add(i);
                }

                for (int i = 30; i > 0; i--)
                {
                    range.Add(i);
                }

                var xAxis = new LineV2XAsis
                {
                    type = "standard",
                    /*  labels = range.Select(x => x.Date.ToString("dd-MM-yyyy")).ToList()*/
                    labels = range.Select(x => x.ToString()).ToList()
                };

                widget109.data.x_axis = xAxis;
                widget109.data.y_axis = new LineV2YAxis
                {
                    format = "decimal",
                    unit = ""
                };

                var series = new LineV2Series { name = "Avg service speed" };
                series.data = new List<decimal>();


                foreach (var date in daterange)
                {
                    try
                    {
                        var startLoop = date;
                        var endLoop = date.AddDays(1);

                        var items = ordersItemsFromLast30.Where(x => x.created_date <= endLoop)
                            .Where(x => x.created_date >= startLoop)
                         .ToList();

                        var avgServiceTime = 0.00;
                        try
                        {
                            avgServiceTime = GenericRevelMethods.GetAverageTimeOfServiceInSeconds(items);
                        }
                        catch (Exception)
                        {


                        }
                        //get avg service time


                        try
                        {
                            var asDecimal = Convert.ToDecimal(avgServiceTime);
                            series.data.Add(Decimal.Round(asDecimal, 3));
                        }
                        catch (Exception)
                        {

                            series.data.Add(0.00M);
                        }

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }


                widget109.data.series = new List<LineV2Series>() { series };

                allTheWidgets.Add(widget109);


                parentWidgetSet.theWidgetCollection = allTheWidgets;

                return parentWidgetSet;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// All datasets already populated in children, just needs push to correct URLs and new methods to extract correct data
        /// </summary>
        /// <param name="parentWidgetSet"></param>
        /// <returns></returns>
        public ParentWidgetSet InitialiseDailyParentWidgetSet(ParentWidgetSet parentWidgetSet, IQueryable<Payment> paymentRollingPast6DaysToToday, IQueryable<Payment> paymentRollingPast6DaysLastWeek)
        {
            var db = new GrindContext();

            parentWidgetSet.WidgetURLBindings = GetURLBindingSetForWidgetSetA(parentWidgetSet);
            List<GeckoboardObject> alltheWidgets = new List<GeckoboardObject>();

            string url; //contains URL to push to


            var softDrinks = Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => x.valueOfSoftDrinkSales));

            var softLastWeek = Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => x.sameDayLastWeekvalueOfSoftDrinkSales));

            decimal test =
                parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Sum(x => x.amount);

            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(1, out url);



            var widget1 = parentWidgetSet.factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
            url,
            "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Sum(x => x.amount), "Same Day Last Week",
            (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Sum(x => x.amount));

            alltheWidgets.Add(widget1);


            //2
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(2, out url); //set url param here for pushURL

            var AllMoniesToday =
                parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Sum(x => x.amount);
            var todaysOrders = parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Count();

            var allMoniesYest = parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Sum(x => x.amount);
            var YestOrders = parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Count();


            var finalResultToday = 0;
            var finalResultlastweek = 0;
            if (todaysOrders == 0 || YestOrders == 0)
            {

            }
            else
            {
                finalResultToday = (int)((AllMoniesToday / todaysOrders) * 100);
                finalResultlastweek = (int)((allMoniesYest / YestOrders) * 100);
            }


            var widget2 = parentWidgetSet.factory.CreateNumberSecondaryStat(2, "NoOfOrdersToday",
              url,
              "Today",
              finalResultToday,
              "Same Day Last Week",
              finalResultlastweek);

            alltheWidgets.Add(widget2);

            //3
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(3, out url); //set url param here for pushURL

            var widget3 = parentWidgetSet.factory.CreateNumberSecondaryStat(3, "NoOfOrdersToday",
            url,
            "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Count(), "Same Day Last Week",
            (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Count());

            alltheWidgets.Add(widget3);

            //4
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(4, out url); //set url param here for pushURL
            var widget4 =
            parentWidgetSet.factory.CreateNumberSecondaryStat(4, "AlcoholSalesToday",
            url,
            "Today", Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => Math.Round(x.valueOfAlcoholSales, 2))), "Same Day Last Week",
            Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => Math.Round(x.sameDayLastWeekvalueOfAlcoholSales, 2))));


            alltheWidgets.Add(widget4);
            //5

            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(5, out url); //set url param here for pushURL
            var NoOfHotDrinksWidget = parentWidgetSet.factory.CreateNumberSecondaryStat(5, "NoOfHotDrinks",
             url,
            "Today", parentWidgetSet.AllChildWidgetSets.Sum(x => x.NoOfHotDrinks), "Same Day Last Week",
            parentWidgetSet.AllChildWidgetSets.Sum(x => x.sameDayLastWeekNoOfHotDrinks));

            alltheWidgets.Add(NoOfHotDrinksWidget);

            //6
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(6, out url); //set url param here for pushURL
            var widget6 = parentWidgetSet.factory.CreateNumberSecondaryStat(6, "FoodSalesToday",
            url,
            "Today", Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => Math.Round(x.valueOfFoodSales, 2))), "Same Day Last Week",
            Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => Math.Round(x.sameDayLastWeekvalueOfFoodSales, 2))));
            alltheWidgets.Add(widget6);

            //7
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(7, out url); //set url param here for pushURL
            var widget7 = parentWidgetSet.factory.CreateText(7, "TodaysOrders", url, new List<Item_Text>
                {
                    new Item_Text(DateTime.Now.ToString(),1)

                });


            alltheWidgets.Add(widget7);

            //8
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(8, out url); //set url param here for pushURL
            var widget8 = parentWidgetSet.factory.CreateNumberSecondaryStat(8, "NoOfSoftDrinks",
            url,
            "Today", Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => Math.Round(x.valueOfSoftDrinkSales, 2))), "Same Day Last Week",
            Convert.ToInt32(parentWidgetSet.AllChildWidgetSets.Sum(x => Math.Round(x.sameDayLastWeekvalueOfSoftDrinkSales, 2))));

            alltheWidgets.Add(widget8);


            //9 hour and spend
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(9, out url); //set url param here for pushURL
            //ITEM MAPPING DATA
            Dictionary<int, decimal> HourAndSpend = new Dictionary<int, decimal>();

            //do the hours from 6 am - 24.00
            for (int i = 6; i < 24; i++)
            {
                var tax = 0.00M;
                var currentAccumulatedHourlySpend = 0.00M;
                int currentHour = i;

                //if the first two digits of the order match our range

                foreach (var childSet in parentWidgetSet.AllChildWidgetSets)
                {
                    foreach (var payment in childSet.PaymentTodaySoFar)
                    {
                        int hourOfOrder = Convert.ToInt16(((DateTime)payment.created_date).ToString("HH"));

                        if (hourOfOrder.Equals(currentHour))
                        {
                            currentAccumulatedHourlySpend += payment.amount;
                        }

                    }

                    //do tax
                    if (childSet.TodaysOrdersSoFar.Orders.Where(
                       x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour).Any())
                    {
                        tax = childSet.TodaysOrdersSoFar.Orders.Where(
                               x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour)
                               .Sum(x => x.tax);
                    }

                }


                //we've done the hour, now add to the dictionary
                HourAndSpend.Add(currentHour, currentAccumulatedHourlySpend /*- tax*/);
                tax = 0.00M; //reset tax
            }

            //do the hours from 24.00 - 03.00
            for (int i = 0; i <= 3; i++)
            {
                var tax = 0.00M;
                var currentAccumulatedHourlySpend = 0.00M;
                int currentHour = i;

                foreach (var childSet in parentWidgetSet.AllChildWidgetSets)
                {
                    //if the first two digits of the order match our range
                    foreach (var order in childSet.TodaysOrdersSoFar.Orders)
                    {
                        int hourOfOrder = Convert.ToInt16(order.created_date.ToString("HH"));

                        if (hourOfOrder.Equals(currentHour))
                        {
                            currentAccumulatedHourlySpend += order.final_total;
                        }

                    }

                    //do tax
                    if (childSet.TodaysOrdersSoFar.Orders.Where(
                       x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour).Any())
                    {
                        tax = childSet.TodaysOrdersSoFar.Orders.Where(
                               x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour)
                               .Sum(x => x.tax);
                    }

                }

                //we've done the hour, now add to the dictionary
                HourAndSpend.Add(currentHour, currentAccumulatedHourlySpend /*- tax*/);
                tax = 0.00M; //reset tax
            }

            //create widget items
            List<string> axisX = new List<string>(); //cash
            List<decimal> axisY = new List<decimal>(); //time


            //get max spend from hour and spend - that's our top level spend. 
            //y axis is now this. 
            var maxSpend = HourAndSpend.Values.Max();

            for (decimal y = 0.0M; y <= 1.0M; y = y + 0.2M)
            {
                axisY.Add((Decimal.Round(y * maxSpend, 2)));
            }

            foreach (var item in HourAndSpend.Keys)
            {
                axisX.Add(item.ToString());
            }


            LineSettings settings = new LineSettings
            {
                axisy = axisY,
                axisx = axisX,
                colour = null

            };

            //create items
            List<decimal> items = new List<decimal>();
            foreach (var item in HourAndSpend.Values)
            {
                items.Add(item);
            }

            //create widget
            var widget9 = parentWidgetSet.factory.CreateLine(9, "HourAndSpend", url, items, settings);
            alltheWidgets.Add(widget9);



            //10
            /*  var discountToday =
               parentWidgetSet.AllChildWidgetSets.Select(
                   x => x.TodaySameDayLastWeekWrapper.OrderItems.Where(y => y.discount_amount > 0.00M).Sum(y => y.discount_amount - (y.pure_sales * 0.2M) )).FirstOrDefault(); ;
            
              var discountLastWeek =
                  parentWidgetSet.AllChildWidgetSets.Select(
                      x => x.TodaySameDayLastWeekWrapper.OrderItems.Where(y => y.discount_amount > 0.00M).Sum(y => y.discount_amount - (y.pure_sales * 0.2M) )).FirstOrDefault(); ;*/


            var totalDiscountToday = 0.00M;

            foreach (var widgetsSet in parentWidgetSet.AllChildWidgetSets)
            {

                foreach (var item in widgetsSet.TodaysOrdersSoFar.OrderItems)
                {
                    if (item.discount_amount > 0.00M)
                    {
                        totalDiscountToday += item.pure_sales;
                        if (item.discount_taxed.Equals(true))
                        {
                            totalDiscountToday += item.tax_amount;
                        }
                    }

                }

            }


            var totalDiscountTodayLastWeek = 0.00M;

            foreach (var widgetsSet in parentWidgetSet.AllChildWidgetSets)
            {
                foreach (var item in widgetsSet.TodaySameDayLastWeekWrapper.OrderItems)
                {
                    if (item.discount_amount > 0.00M)
                    {
                        totalDiscountTodayLastWeek += item.pure_sales;
                        if (item.discount_taxed.Equals(true))
                        {
                            totalDiscountTodayLastWeek += item.tax_amount;
                        }
                    }
                }

            }



            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(10, out url); //set url param here for pushURL
            var widget10 = parentWidgetSet.factory.CreateNumberSecondaryStat(10, "Discount",
             url,
            "Today",
            Convert.ToInt32(Decimal.Round(totalDiscountToday, 2))
            , "Same Day Last Week",
             Convert.ToInt32(Decimal.Round(totalDiscountTodayLastWeek, 2))
            //last week discounts -???
            );

            alltheWidgets.Add(widget10);


            //COFFEE AVG SERVICE TIME
            //11

            //today
            int Nnancount = 0;
            double totalSecond = 0.00;
            foreach (var set in parentWidgetSet.AllChildWidgetSets)
            {
                if (!Double.IsNaN(set.coffeeServiceTimeAvgToday) && set.coffeeServiceTimeAvgToday > 0.00)
                {
                    Nnancount++;
                    totalSecond += set.coffeeServiceTimeAvgToday;
                }

            }


            int NnancountYest = 0;
            double totalSecondYest = 0.00;
            foreach (var set in parentWidgetSet.AllChildWidgetSets)
            {
                if (!Double.IsNaN(set.coffeeServiceTimeAvgYest) && set.coffeeServiceTimeAvgToday > 0.00)
                {
                    NnancountYest++;
                    totalSecondYest += set.coffeeServiceTimeAvgYest;
                }

            }



            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(11, out url);


            if (totalSecond != 0 && totalSecondYest != 0)
            {

                double avgSecondsToday = totalSecond / Nnancount;
                double avgSecondsYest = totalSecondYest / NnancountYest;

                NumberSecondaryStat AvgCoffeeServiceTime = parentWidgetSet.factory.CreateNumberSecondaryStat(11,
                    "AvgCoffeeServiceTime",
                    url,
                    "AvgCoffeeServiceTimeToday",
                    Convert.ToInt32(
                        avgSecondsToday),
                    "AvgCoffeeServiceTimeYesterday",
                    Convert.ToInt32(
                        avgSecondsYest));

                alltheWidgets.Add(AvgCoffeeServiceTime);
            }


            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(12, out url);

            List<Order> unpaidToday =
                parentWidgetSet.AllChildWidgetSets.SelectMany(
                    x => x.TodaysOrdersSoFar.Orders.Where(y => y.is_unpaid == "True" && y.closed == false)).ToList();




            NumberSecondaryStat openAndUnpaidOrders = parentWidgetSet.factory.CreateNumberSecondaryStat(12,
                "OpenAndUnpaidOrders",
                url,
                "OpenAndUnpaidOrdersToday",
                Convert.ToInt32(unpaidToday.Sum(x => x.final_total)),
                "OpenAndUnpaidOrdersYesterday",
                Convert.ToInt32(0));

            alltheWidgets.Add(openAndUnpaidOrders);


            //added by ND at David's request to parent grind
            //shoreditch daily
            var widget11 = parentWidgetSet.factory.CreateNumberSecondaryStat(14, "TodayShoreditch£",
            "https://push.geckoboard.com/v1/send/95518-7bd8ee27-ff5e-4f3d-bf44-9b327fe08b39",
            "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(1)).Sum(x => x.amount), "Same Day Last Week",
            (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(1)).Sum(x => x.amount));

            alltheWidgets.Add(widget11);


            var widget12 = parentWidgetSet.factory.CreateNumberSecondaryStat(15, "TodaySoho£",
            "https://push.geckoboard.com/v1/send/95518-b9fd5a5e-5672-47b0-8f57-067ae2bbad90",
            "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(3)).Sum(x => x.amount), "Same Day Last Week",
            (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(3)).Sum(x => x.amount));

            alltheWidgets.Add(widget12);


            var widget13 = parentWidgetSet.factory.CreateNumberSecondaryStat(16, "TodayLondon£",
             "https://push.geckoboard.com/v1/send/95518-1c95724a-1848-4bf4-9d18-1b1fda1f1f09",
             "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(4)).Sum(x => x.amount), "Same Day Last Week",
             (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(4)).Sum(x => x.amount));

            alltheWidgets.Add(widget13);

            var widget14 = parentWidgetSet.factory.CreateNumberSecondaryStat(20, "TodayHolborn£",
             "https://push.geckoboard.com/v1/send/95518-fb481bbf-57f4-4497-b645-b7118af5b280",
             "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(5)).Sum(x => x.amount), "Same Day Last Week",
             (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(5)).Sum(x => x.amount));

            alltheWidgets.Add(widget14);

            var widgetStrat = parentWidgetSet.factory.CreateNumberSecondaryStat(18, "TodayStrat£",
           "https://push.geckoboard.com/v1/send/95518-b9f911dd-b400-4f5e-922a-ad42dc91e70e",
           "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(7)).Sum(x => x.amount), "Same Day Last Week",
           (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(7)).Sum(x => x.amount));

            alltheWidgets.Add(widgetStrat);

            var widgetRadio = parentWidgetSet.factory.CreateNumberSecondaryStat(19, "TodayRadio£",
        "https://push.geckoboard.com/v1/send/95518-66c6d7b5-4ffd-45bd-b102-076e1c01172e",
        "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(8)).Sum(x => x.amount), "Same Day Last Week",
        (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(8)).Sum(x => x.amount));

            alltheWidgets.Add(widgetRadio);


            var widgetRoyal = parentWidgetSet.factory.CreateNumberSecondaryStat(19, "TodayRoyal£",
    "https://push.geckoboard.com/v1/send/95518-e6b2279e-c3eb-46a7-8d03-26e27e5ad7e6",
    "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(6)).Sum(x => x.amount), "Same Day Last Week",
    (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(6)).Sum(x => x.amount));
            alltheWidgets.Add(widgetRoyal);


            var widgetExmouth = parentWidgetSet.factory.CreateNumberSecondaryStat(19, "TodayExmouth£",
 "https://push.geckoboard.com/v1/send/51912-21d2bec0-c895-0134-a2d4-22000b048960",
 "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(10)).Sum(x => x.amount), "Same Day Last Week",
 (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(10)).Sum(x => x.amount));
            alltheWidgets.Add(widgetExmouth);

            var widgetWhite = parentWidgetSet.factory.CreateNumberSecondaryStat(19, "TodayWC",
 "https://push.geckoboard.com/v1/send/51912-c8776700-f388-0134-dfc0-22000b048960",
 "Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(9)).Sum(x => x.amount), "Same Day Last Week",
 (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(9)).Sum(x => x.amount));
            alltheWidgets.Add(widgetWhite);

            var widgetGW = parentWidgetSet.factory.CreateNumberSecondaryStat(20, "TodayGW",
"https://push.geckoboard.com/v1/send/51912-b0058dc0-d5f7-0136-5b02-0e2e82f783c6",
"Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(13)).Sum(x => x.amount), "Same Day Last Week",
(int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(13)).Sum(x => x.amount));
            alltheWidgets.Add(widgetGW);

            var widgetLIVER = parentWidgetSet.factory.CreateNumberSecondaryStat(20, "Today Liverpool St",
"https://push.geckoboard.com/v1/send/51912-cb7e7630-3aa7-0137-ff6a-0292b2f0be54",
"Today", (int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentTodaySoFar).Where(x => x.establishment_id.Equals(14)).Sum(x => x.amount), "Same Day Last Week",
(int)parentWidgetSet.AllChildWidgetSets.SelectMany(x => x.PaymentsTodaySameDayLastWeek).Where(x => x.establishment_id.Equals(14)).Sum(x => x.amount));
            alltheWidgets.Add(widgetLIVER);

            //addded 11/17/2014 ND

            //107 rolling past 6 days plus today PIT

            //dates
            /*     var RollingPast6DaysToTodayStart =
                     parentWidgetSet.AllChildWidgetSets.ElementAt(0).RollingPast6DaysToTodayStart;
                 var RollingPast6DaysToTodayEnd =
                     parentWidgetSet.AllChildWidgetSets.ElementAt(0).RollingPast6DaysToTodayEnd;

                 var RollingPast6DaysLastWeekStart =
                     parentWidgetSet.AllChildWidgetSets.ElementAt(0).RollingPast6DaysLastWeekStart;
                 var RollingPast6DaysLastWeekEnd =
                     parentWidgetSet.AllChildWidgetSets.ElementAt(0).RollingPast6DaysLastWeekEnd;


                 int last6daysRollingWeekPayment = (int)context.Payments
                     .Where(x => x.created_date >= RollingPast6DaysToTodayStart)
                     .Where(x => x.created_date <= RollingPast6DaysToTodayEnd)
                     .Sum(x => x.amount);

                 int last6daysRollingWeekBeforePayment = (int)context.Payments
                     .Where(x => x.created_date >= RollingPast6DaysLastWeekStart)
                     .Where(x => x.created_date <= RollingPast6DaysLastWeekEnd)
                     .Sum(x => x.amount);
     */
            var amountToday = paymentRollingPast6DaysToToday.Sum(x => x.amount).ToString();
            var amountLastweek = paymentRollingPast6DaysLastWeek.Sum(x => x.amount).ToString();

            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(107, out url);

            var test1 = paymentRollingPast6DaysToToday.Sum(x => x.amount);


            NumberSecondaryStat RollingPast6DaysToTodayWidget = parentWidgetSet.factory.CreateNumberSecondaryStat(107,
                "RollingPast6DaysToTodayWidget",
                url,
                "RollingPast6DaysToToday",
                (int)(paymentRollingPast6DaysToToday
                    .Select(x => x.amount)
                    .Sum()),
                "RollingPast6DaysLastWeek",
                (int)(paymentRollingPast6DaysLastWeek
                    .Select(x => x.amount)
                    .Sum()));

            alltheWidgets.Add(RollingPast6DaysToTodayWidget);



            //Added Bullet chart for bucket week
            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(13, out url);
            DateTime ThisWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday);
            DateTime LastWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays(-7), DayOfWeek.Monday);

            var ordersSinceMonday = db.Payments
                .Where(x => x.created_date >= ThisWeekStart)
                .Select(x => x.amount)
                .ToList()
                .Sum();

            var taxSinceMonday = db.Orders
                .Where(x => x.created_date >= ThisWeekStart)
                .Select(x => x.tax)
                .ToList()
                .Sum();

            var finalPaymentsThisWeek = ordersSinceMonday - taxSinceMonday;


            var calendar = _445Calendar.GetCurrentWeek(db);
            var budget = calendar.Projections.Sum(x => x.ProjectionFigure);

            var lastWeekSales = db.Payments
                .Where(x => x.created_date >= LastWeekStart && x.created_date <= ThisWeekStart)
                .Select(x => x.amount)
                .ToList()
                .Sum();


            var lastWeekTax = db.Orders
                .Where(x => x.created_date >= LastWeekStart && x.created_date <= ThisWeekStart)
                .Select(x => x.tax)
                .ToList()
                .Sum();

            var finalPaymenetLastweek = lastWeekSales - lastWeekTax;

            //create bullet
            BulletItem ChartItem = new BulletItem
            {
                label = "This Week (NET£)",
                sublabel = "vs Budget (Green)",

                axis = new BulletAxis
                {
                    point = new List<string>
                       {
                          "0",(Convert.ToInt32(budget * 0.25M)).ToString(),(Convert.ToInt32(budget * 0.50M)).ToString(),Convert.ToInt32(budget * 0.75M).ToString(),(Convert.ToInt32(budget * 1.0M)).ToString(),(Convert.ToInt32(budget * 1.25M)).ToString()
                       }
                },

                range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=Convert.ToInt32(budget-1M)},
                               new BulletRange{color="green", start=Convert.ToInt32(budget), end=Convert.ToInt32(budget * 1.25M)}

                   },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = Convert.ToInt32(finalPaymentsThisWeek).ToString() },
                    projected = new BulletMeasureItem { start = "0", end = "0" }

                },

                comparative = new BulletComparative { point = finalPaymenetLastweek.ToString() }
            };

            var bulletWeeklyBudget = parentWidgetSet.factory.CreateBullet(13, "BulletWeeklyBudget", url, "horizontal", ChartItem);

            alltheWidgets.Add(bulletWeeklyBudget);


            parentWidgetSet.WidgetURLBindings.widgetBindMappings.TryGetValue(17, out url); //set url param here for pushURL
            var widget17 = parentWidgetSet.factory.CreateNumberSecondaryStat(17, "ServiceCharge",
            url,
            "Today", Convert.ToInt32(db.Orders
            .Where(x => x.created_date >= parentWidgetSet.LastWeekStart && x.created_date <= parentWidgetSet.LastWeekEnd)
            .Select(x => x.gratuity)
            .Sum()), "Service Charge",
            Convert.ToInt32(db.Orders
            .Where(x => x.created_date >= parentWidgetSet.WeekBeforeLastStart && x.created_date <= parentWidgetSet.WeekBeforeLastEnd)
            .Select(x => x.gratuity)
            .Sum()));

            alltheWidgets.Add(widget17);


            //web orders
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 00, 00);
            var tomz = today.AddDays(1);

            var lastweeKToday = today.AddDays(-7);
            var lastweeKtomz = tomz.AddDays(-7);


            var weBOrders = parentWidgetSet.factory.CreateNumberSecondaryStat(200, "WebOrders",
            "https://push.geckoboard.com/v1/send/95518-f3151e18-00b3-4043-82c1-f4f805ad9d98",
            "WebOrders",
            Convert.ToInt32(db.Orders.AsNoTracking().Count(x => x.created_date >= today && x.created_date <= tomz && x.web_order)), "Service Charge",
            Convert.ToInt32(db.Orders.AsNoTracking().Where(x => x.created_date >= lastweeKToday && x.created_date <= lastweeKtomz && x.web_order).Count()));

            alltheWidgets.Add(weBOrders);

            //end individual widgets
            parentWidgetSet.theWidgetCollection = alltheWidgets;

            return parentWidgetSet;

        }



        public async Task<WidgetSetA> InitialiseWidgetSetAOvernightWidgets(WidgetSetA widgetSetA, IEnumerable<ProductClass> productClasses)
        {
            var db = new GrindContext();
            db.Database.CommandTimeout = 600;

            IList<Product> tempErrors = new List<Product>();
            IList<Product> errors = new List<Product>();

            widgetSetA.WidgetURLBindings = GetURLBindingSetForWidgetSetA(widgetSetA);

            //create prods
            widgetSetA.pcWrapper = new RevelProductAndCategoryWrapper();
            widgetSetA.pcWrapper.ProductCategories =
                     db.ProductCategories.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();
            widgetSetA.pcWrapper.Products =
               db.Products.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();

            widgetSetA.pcWrapper.ProductCategoriesComparisonDictionary = new Dictionary<int, string>();
            widgetSetA.pcWrapper.CreateProductCategoriesDictionary();

            widgetSetA.alcoholProducts = widgetSetA.pcWrapper.GetProductsThatAreAlcoholByClass(productClasses, out tempErrors);
            widgetSetA.foodProducts = widgetSetA.pcWrapper.GetProductsThatAreFoodByClass(productClasses, out tempErrors);
            widgetSetA.hotDrinkProducts = widgetSetA.pcWrapper.GetProductsThatAreHotDrinksByClass(productClasses, out tempErrors);

            //DATES
            widgetSetA.ThirtyDaysToYesterdayEnd = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-1));
            widgetSetA.ThirtyDaysToYesterdayStart = RevelHelper.WrapAllRevelStartingDatesInThisMethod(widgetSetA.ThirtyDaysToYesterdayEnd.AddDays(-30));



            //30 days before that
            widgetSetA.ThirtyDaysPreviousEnd = RevelHelper.WrapAllRevelStartingDatesInThisMethod(widgetSetA.ThirtyDaysToYesterdayStart.AddDays(-1));
            widgetSetA.ThirtyDaysPreviousStart = widgetSetA.ThirtyDaysPreviousEnd.AddDays(-30);



            //30 days previous, last year            
            widgetSetA.ThirtyDaysToYesterdayEndLastYear = RevelHelper.WrapAllRevelStartingDatesInThisMethod(widgetSetA.ThirtyDaysToYesterdayEnd.AddYears(-1));
            widgetSetA.ThirtyDaysToYesterdayStartLastYear = widgetSetA.ThirtyDaysToYesterdayEndLastYear.AddDays(-30);

            //last week
            widgetSetA.LastWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays((-7)), DayOfWeek.Monday);
            widgetSetA.LastWeekEnd = widgetSetA.LastWeekStart.AddDays(7);

            //week before last
            widgetSetA.WeekBeforeLastStart = widgetSetA.LastWeekStart.AddDays(-7);
            widgetSetA.WeekBeforeLastEnd = widgetSetA.WeekBeforeLastStart.AddDays(7);



            var lastMonthFullDate = DateTime.Now.AddMonths(-1);

            widgetSetA.FirstDayOfLastMonth = new DateTime(lastMonthFullDate.Year, lastMonthFullDate.Month, 01, 02, 00, 00);
            widgetSetA.FirstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01, 02, 00, 00);


            int lastMonthLastYearInt = DateTime.Now.AddMonths(-1).AddYears(-1).Month;

            widgetSetA.FirstDayOfLastMonthLastYear = new DateTime(DateTime.Now.AddYears(-1).Year, lastMonthLastYearInt, 01, 02, 00, 00);
            widgetSetA.FirstDayOfThisMonthLastYear = new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 01, 02, 00, 00);

            widgetSetA.ThisYearStart = new DateTime(DateTime.Now.Year, 01, 01, 02, 00, 00);

            var yesterday = DateTime.Now.AddDays(1);
            widgetSetA.YTDYesterday = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 02, 00, 00);

            widgetSetA.LastYearStart = widgetSetA.ThisYearStart.AddYears(-1);
            widgetSetA.LastYearYesterday = widgetSetA.YTDYesterday.AddYears(-1);

            widgetSetA.yesterday = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-1));
            widgetSetA.yesterday = new DateTime(widgetSetA.yesterday.Year, widgetSetA.yesterday.Month, widgetSetA.yesterday.Day, 02, 00, 00);

            widgetSetA.yesterDayLastWeek = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-8));
            widgetSetA.yesterDayLastWeek = new DateTime(widgetSetA.yesterDayLastWeek.Year, widgetSetA.yesterDayLastWeek.Month, widgetSetA.yesterDayLastWeek.Day, 02, 00, 00);

            //init order and item wrappers

            widgetSetA.last30 = new RevelOrderandOrderItemWrapper(widgetSetA.ThirtyDaysToYesterdayStart, widgetSetA.ThirtyDaysToYesterdayEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);
            widgetSetA.last30Previous = new RevelOrderandOrderItemWrapper(widgetSetA.ThirtyDaysPreviousStart, widgetSetA.ThirtyDaysPreviousEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);
            widgetSetA.last30LastYear = new RevelOrderandOrderItemWrapper(widgetSetA.ThirtyDaysToYesterdayStartLastYear, widgetSetA.ThirtyDaysToYesterdayEndLastYear, RevelOrderandOrderItemWrapper.WrapperType.Order);

            widgetSetA.lastWeek = new RevelOrderandOrderItemWrapper(widgetSetA.LastWeekStart, widgetSetA.LastWeekEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);
            widgetSetA.weekBeforeLast = new RevelOrderandOrderItemWrapper(widgetSetA.WeekBeforeLastStart, widgetSetA.WeekBeforeLastEnd, RevelOrderandOrderItemWrapper.WrapperType.Order);

            widgetSetA.yesterdaysOrders = new RevelOrderandOrderItemWrapper(widgetSetA.yesterday, widgetSetA.yesterday.AddDays(1), RevelOrderandOrderItemWrapper.WrapperType.Full);
            widgetSetA.yesterdaysLastWeekOrders = new RevelOrderandOrderItemWrapper(widgetSetA.yesterDayLastWeek, widgetSetA.yesterDayLastWeek.AddDays(1), RevelOrderandOrderItemWrapper.WrapperType.Full);

            widgetSetA.lastMonth = new RevelOrderandOrderItemWrapper(widgetSetA.FirstDayOfLastMonth,
               widgetSetA.FirstDayOfThisMonth, RevelOrderandOrderItemWrapper.WrapperType.Order);

            widgetSetA.lastMonthLastYear = new RevelOrderandOrderItemWrapper(widgetSetA.FirstDayOfLastMonthLastYear,
               widgetSetA.FirstDayOfThisMonthLastYear, RevelOrderandOrderItemWrapper.WrapperType.Order);

            widgetSetA.ThisYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(widgetSetA.ThisYearStart, widgetSetA.YTDYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);
            widgetSetA.LastYearStartToTodayWrapper = new RevelOrderandOrderItemWrapper(widgetSetA.LastYearStart, widgetSetA.LastYearYesterday, RevelOrderandOrderItemWrapper.WrapperType.Order);

            TestController tc = new TestController();

            //payments
            var paymentslast30 = (await tc.GetPaymentsFromDB(widgetSetA.ThirtyDaysToYesterdayStart,
                widgetSetA.ThirtyDaysToYesterdayEnd)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                .Select(x => x.amount)
                .ToList();
            var paymentslast30Previous = (await tc.GetPaymentsFromDB(widgetSetA.ThirtyDaysPreviousStart,
                widgetSetA.ThirtyDaysPreviousEnd)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                .Select(x => x.amount)
                .ToList();
            var paymentslast30LastYear = (await tc.GetPaymentsFromDB(widgetSetA.ThirtyDaysToYesterdayStartLastYear,
                widgetSetA.ThirtyDaysToYesterdayEndLastYear)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                .Select(x => x.amount)
                .ToList();

            var paymentslastWeek = (await tc.GetPaymentsFromDB(widgetSetA.LastWeekStart, widgetSetA.LastWeekEnd)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Select(x => x.amount)
                .ToList();
            var paymentsweekBeforeLast = (await tc.GetPaymentsFromDB(widgetSetA.WeekBeforeLastStart,
                widgetSetA.WeekBeforeLastEnd)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Select(x => x.amount)
                 .ToList();

            var paymentsYesterdays = (await tc.GetPaymentsFromDB(widgetSetA.yesterday, widgetSetA.yesterday.AddDays(1))).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Select(x => x.amount)
                 .ToList();
            var paymentsyesterdaysLastWeek = (await tc.GetPaymentsFromDB(widgetSetA.yesterDayLastWeek, widgetSetA.yesterDayLastWeek.AddDays(1))).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Select(x => x.amount)
                 .ToList();

            var paymentslastMonth = (await tc.GetPaymentsFromDB(widgetSetA.FirstDayOfLastMonth,
                widgetSetA.FirstDayOfThisMonth)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Select(x => x.amount)
                 .ToList();
            var paymentslastMonthLastYear = (await tc.GetPaymentsFromDB(widgetSetA.FirstDayOfLastMonthLastYear,
                widgetSetA.FirstDayOfThisMonthLastYear)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Select(x => x.amount)
                 .ToList();

            var paymentsThisYearStartToTodayWrapper = (await tc.GetPaymentsFromDB(widgetSetA.ThisYearStart, widgetSetA.YTDYesterday)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Select(x => x.amount).ToList();
            var paymentsLastYearStartToTodayWrapper = (await tc.GetPaymentsFromDB(widgetSetA.LastYearStart, widgetSetA.LastYearYesterday)).Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Select(x => x.amount).ToList();


            /*   await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.last30);
               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.last30Previous);
               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.last30LastYear);

               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.lastWeek);
               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.weekBeforeLast);

               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.yesterdaysOrders);
               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.yesterdaysLastWeekOrders);

               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.lastMonth);

               await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.lastMonthLastYear);*/

            /* await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.ThisYearStartToTodayWrapper);

             await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.LastYearStartToTodayWrapper);*/



            //create and populate the widgets:
            string url;

            List<GeckoboardObject> allTheWidgets = new List<GeckoboardObject>();


            //WIDGET 1



            //101
            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(101, out url);

            int LastWeekGross = (int)(paymentslastWeek.Sum(x => x) /*- widgetSetA.lastWeek.GetOrderTotalPoundsTax()*/);
            int weekBeforeGross = (int)(paymentsweekBeforeLast.Sum(x => x) /*- widgetSetA.weekBeforeLast.GetOrderTotalPoundsTax()*/);

            NumberSecondaryStat LastWeekNetVSweekBeforeNet = widgetSetA.factory.CreateNumberSecondaryStat(101, "LastWeekNetVSweekBeforeNet",
            url, "LastCompleteWeekNet £", LastWeekGross,
            "CompleteWeekBeforeNet", weekBeforeGross);

            allTheWidgets.Add(LastWeekNetVSweekBeforeNet);


            //102
            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(102, out url);

            var yesterSalesGross =
                (int)(paymentsYesterdays.Sum(x => x) /*- widgetSetA.yesterdaysOrders.GetOrderItemPoundsTax()*/);
            var yesterLastweekSalesGross =
                (int)
                    (paymentsyesterdaysLastWeek.Sum(x => x) /*-
                      widgetSetA.yesterdaysLastWeekOrders.GetOrderTotalPoundsTax()*/);

            NumberSecondaryStat yesterdayVSYesterdayLastWeek = widgetSetA.factory.CreateNumberSecondaryStat(102, "yesterdayVSYesterdayLastWeek",
               url, "Yesterday £", yesterSalesGross,
               "Yesterday Last Week", yesterLastweekSalesGross);

            allTheWidgets.Add(yesterdayVSYesterdayLastWeek);



            //103
            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(103, out url);

            //get budget for correct month/venue
            int currentMonth = DateTime.Now.Month;

            var budget = 0;
            try
            {
                budget = widgetSetA.RevelEstablishment.AnnualBudget[currentMonth];
            }
            catch (Exception)
            {


            }


            NumberSecondaryStat lastMonthVSBudget = widgetSetA.factory.CreateNumberSecondaryStat(103, "LastWeekNetVSweekBeforeNet",
             url, "Last Month £", (int)(paymentslastMonth.Sum(x => x) - widgetSetA.lastMonth.GetOrderTotalPoundsTax()),
             "Budget", budget);

            allTheWidgets.Add(lastMonthVSBudget);


            //last month vs budget
            //104
            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(104, out url);


            var currentMonthly = paymentslastMonth.Sum(x => x) - widgetSetA.lastMonth.GetOrderTotalPoundsTax();
            var sameMonthLastYear = paymentslastMonthLastYear.Sum(x => x) - widgetSetA.lastMonthLastYear.GetOrderTotalPoundsTax();

            var monthBudget = budget;



            BulletItem item = new BulletItem
            {
                label = "Last Month(NET£)",
                sublabel = "vs Last Month Last Year(NET£) and Budget (Green)",

                axis = new BulletAxis
                {
                    point = new List<string>
                       {
                          "0",(monthBudget * 0.25).ToString(),(monthBudget * 0.50).ToString(),(monthBudget * 0.75).ToString(),(monthBudget * 1.0).ToString(),(monthBudget * 1.25).ToString()
                       }
                },

                range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=monthBudget-1},
                               new BulletRange{color="green", start=monthBudget, end=(int)(monthBudget * 1.25)}

                   },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = currentMonthly.ToString() },
                    projected = new BulletMeasureItem { start = "0", end = "0" }

                },

                comparative = new BulletComparative { point = sameMonthLastYear.ToString() }
            };

            var BulletMonthlyBudget = widgetSetA.factory.CreateBullet(104, "BulletMonthlyBudget", url, "horizontal", item);

            allTheWidgets.Add(BulletMonthlyBudget);


            /*
                        //105
                        //calcs
                        Nullable<decimal> thisYearSumPayments = 0.00M;
                        Nullable<decimal> thisYearSumTax = 0.00M;

                        try
                        {
                            thisYearSumPayments = db.Payments
                                .AsNoTracking()
                                   .Where(x => x.created_date >= widgetSetA.ThisYearStart)
                                   .Where(x => x.created_date <= widgetSetA.YTDYesterday)
                                     .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                                   .Sum(x => x.amount);
                            thisYearSumTax = db.Orders
                                .AsNoTracking()
                                .Where(x => x.created_date >= widgetSetA.ThisYearStart)
                                                    .Where(x => x.created_date <= widgetSetA.YTDYesterday)
                                                      .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                                                    .Sum(x => x.tax);
                        }
                        catch (Exception)
                        {


                        }


                        Nullable<decimal> lastYearSumPayments = 0.00M;
                        Nullable<decimal> lastYearSumTax = 0.00M;

                        if (db.Payments
                            .AsNoTracking()
                                .Where(x => x.created_date >= widgetSetA.LastYearStart)
                                .Where(x => x.created_date <= widgetSetA.LastYearYesterday)
                                .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                                .Any())
                        {
                            lastYearSumPayments = db.Payments
                                .AsNoTracking()
                               .Where(x => x.created_date >= widgetSetA.LastYearStart)
                               .Where(x => x.created_date <= widgetSetA.LastYearYesterday)
                               .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                               .Sum(x => x.amount);

                            lastYearSumTax = db.Payments
                                .AsNoTracking()
                                .Where(x => x.created_date >= widgetSetA.LastYearStart)
                                                                .Where(x => x.created_date <= widgetSetA.LastYearYesterday)
                                                                .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                                                                .Sum(x => x.amount);
                        }



                        //widget
                        widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(105, out url);

                        NumberSecondaryStat ThisYearStartToTodayWidget = widgetSetA.factory.CreateNumberSecondaryStat(105, "ThisYearStartToToday",
                               url,
                               "YTDYesterday",
                               (int)(thisYearSumPayments - thisYearSumTax),
                               "YTD Yesterday Last Year",
                               (int)(lastYearSumPayments - lastYearSumTax));

                        allTheWidgets.Add(ThisYearStartToTodayWidget);*/



            //last 30 days
            //106
            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(106, out url);

            var BaselineForChart = Convert.ToInt32(paymentslast30.Sum(x => x));

            BulletItem last30Item = new BulletItem
            {
                label = "Last 30 days (GROSS£)",
                sublabel = "vs previous 30 days (red/green) and same 30 days last year (line)",

                axis = new BulletAxis
                {
                    point = new List<string>
                       {
                          "0",(BaselineForChart*0.25).ToString(),(BaselineForChart*0.50).ToString(),(BaselineForChart*0.75).ToString(),(BaselineForChart*1.00).ToString(),(BaselineForChart*1.25).ToString(),
                       }
                },

                range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=Convert.ToInt32(paymentslast30Previous.Sum(x=>x))},
                       new BulletRange{color="green", start=Convert.ToInt32(paymentslast30Previous.Sum(x=>x)) + 1, end=(int)(BaselineForChart*1.25)}

                   },

                measure = new BulletMeasure
                {
                    current = new BulletMeasureItem { start = "0", end = paymentslast30.Sum(x => x).ToString() },
                    projected = new BulletMeasureItem { start = "0", end = "0" }

                },

                comparative = new BulletComparative { point = paymentslast30LastYear.Sum(x => x).ToString() }
            };


            var BulletLast30 = widgetSetA.factory.CreateBullet(106, "BulletLast30", url, "horizontal", last30Item);

            allTheWidgets.Add(BulletLast30);


            //last 30 days avg service time
            //109
            var start = new DateTime(widgetSetA.ThirtyDaysToYesterdayStart.Year, widgetSetA.ThirtyDaysToYesterdayStart.Month, widgetSetA.ThirtyDaysToYesterdayStart.Day, 03, 00, 00);
            var end = new DateTime(widgetSetA.ThirtyDaysToYesterdayEnd.Year, widgetSetA.ThirtyDaysToYesterdayEnd.Month, widgetSetA.ThirtyDaysToYesterdayEnd.Day, 03, 00, 00);

            var daterange = new List<DateTime>();
            var range = new List<int>();
            var ordersItemsFromLast30 = db.OrderItems
                .AsNoTracking()
                .Where(x => x.created_date <= end)
                .Where(x => x.created_date >= start)
                .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                .ToList();

            widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(109, out url);

            var widget109 = new LineV2Widget("ab876212d31d37960e3154eb5e2bc0a0",
                 url, "test",
                 GeckoboardChartAndItemType.LineV2, 109);


            for (DateTime i = start; i < end; i = i.AddDays(1))
            {
                daterange.Add(i);
            }

            for (int i = 30; i > 0; i--)
            {
                range.Add(i);
            }

            var xAxis = new LineV2XAsis
            {
                type = "standard",
                /*  labels = range.Select(x => x.Date.ToString("dd-MM-yyyy")).ToList()*/
                labels = range.Select(x => x.ToString()).ToList()
            };

            widget109.data.x_axis = xAxis;
            widget109.data.y_axis = new LineV2YAxis
            {
                format = "decimal",
                unit = ""
            };

            var series = new LineV2Series { name = "Avg service speed" };
            series.data = new List<decimal>();


            foreach (var date in daterange)
            {
                try
                {
                    var startLoop = date;
                    var endLoop = date.AddDays(1);

                    var items = ordersItemsFromLast30.Where(x => x.created_date <= endLoop)
                        .Where(x => x.created_date >= startLoop)
                     .ToList();

                    var avgServiceTime = 0.00;
                    try
                    {
                        avgServiceTime = GenericRevelMethods.GetAverageTimeOfServiceInSeconds(items);
                    }
                    catch (Exception)
                    {


                    }
                    //get avg service time


                    try
                    {
                        var asDecimal = Convert.ToDecimal(avgServiceTime);
                        series.data.Add(Decimal.Round(asDecimal, 3));
                    }
                    catch (Exception)
                    {

                        series.data.Add(0.00M);
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }


            widget109.data.series = new List<LineV2Series>() { series };

            allTheWidgets.Add(widget109);
            //end 109


            widgetSetA.theWidgetCollection = allTheWidgets;

            db.Dispose();

            return widgetSetA;
        }



        public async Task<WidgetSetA> BindDailyPaymentsOrdersAndItems(WidgetSetA widgetSetA, List<Order> orders, List<OrderItem> items, List<Payment> payments)
        {
            //bind the orders



            //bind the items


            //bind the payments

            return widgetSetA;
        }



        public async Task<WidgetSetA> BindOvernightPaymentsOrdersAndItems(WidgetSetA widgetSetA)
        {


            return widgetSetA;
        }



        public async Task<WidgetSetA>
            InitialiseWidgetSetADailyWidgets(WidgetSetA widgetSetA, IQueryable<Payment> paymentsRollingPast6DaysToToday, IQueryable<Payment> paymentRollingPast6DaysLastWeek)
        {
            var db = new GrindContext();

            if (widgetSetA.RevelEstablishment.name == "Whitechapel")
            {

                var stop = "";
            }

            try
            {
                //set up URL bindings - init all widgets and assign correct URLs                
                widgetSetA.WidgetURLBindings = GetURLBindingSetForWidgetSetA(widgetSetA);//dont actually need these

                //create prods
                widgetSetA.pcWrapper = new RevelProductAndCategoryWrapper();
                var prodClasses = db.ProductClasses.ToList();

                //new product code
                widgetSetA.pcWrapper.ProductCategories =
                    db.ProductCategories.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();
                widgetSetA.pcWrapper.Products =
                   db.Products.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();

                widgetSetA.pcWrapper.ProductCategoriesComparisonDictionary = new Dictionary<int, string>();
                widgetSetA.pcWrapper.CreateProductCategoriesDictionary();

                //assign correct dates for all widgets
                widgetSetA.today = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now);

                widgetSetA.TodaySameDayLastWeekMinusOne = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-7));
                widgetSetA.TodaySameDayLastWeek = DateTime.Now.AddDays(-7);

                //init order wrappers

                //today and yesterday
                var todayStamp = DateTime.Now;

                var todayStart = RevelHelper.WrapAllRevelStartingDatesInThisMethod(new DateTime(todayStamp.Year, todayStamp.Month, todayStamp.Day, 03, 00, 00));
                var todayEnd = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now);

                var tomorrowStart = RevelHelper.WrapAllRevelStartingDatesInThisMethod(todayStart.AddDays(1));
                var tomorrowEnd = todayEnd.AddDays(1);

                var yestedayStart = todayStart.AddDays(-1);
                var yesterdayEnd = todayEnd.AddDays(-1);

                var todayStartLastWeek = todayStart.AddDays(-7);
                var todayEndLastWeek = todayEnd.AddDays(-7);
                // var tomorrowLastWeek = tomorrowStart.AddDays(-7);



                //INIT ORDER AND ITEM WRAPPERS
                widgetSetA.TodaysOrdersSoFar = new RevelOrderandOrderItemWrapper();
                widgetSetA.TodaySameDayLastWeekWrapper = new RevelOrderandOrderItemWrapper();
                widgetSetA.YesterdaysOrders = new RevelOrderandOrderItemWrapper();


                widgetSetA.TodaysOrdersSoFar.Orders = db.Orders
                    .Where(x => x.created_date >= todayStart && x.created_date <= todayEnd)
                    .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                    .Where(x => x.bill_parent == null)
                    .ToList();

                var todayORdersIdsToExlucde =
                    db.Orders
                    .Where(x => x.created_date >= todayStart && x.created_date <= todayEnd)
                    .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                  .Where(x => x.bill_parent != null)
                    .Select(x => x.order_id)
                    .ToList();

                widgetSetA.TodaysOrdersSoFar.OrderItems = db.OrderItems
                     .Where(x => x.created_date >= todayStart && x.created_date <= todayEnd)
              .FilterCompsAndVoids()
                     .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id).
               ToList();

                //////////////////////
                //same day last week
                widgetSetA.TodaySameDayLastWeekWrapper.Orders = db.Orders
                    .Where(x => x.created_date >= todayStartLastWeek && x.created_date <= todayEndLastWeek)
                    .Where(x => x.bill_parent == null)
                    .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                    .ToList();

                var sameDayLastWeeORdersIdsToExlucde =
                    db.Orders
                .Where(x => x.created_date >= todayStartLastWeek && x.created_date <= todayEndLastWeek)
                    .Where(x => x.bill_parent != null)
                    .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                    .Select(x => x.order_id)
                    .ToList();

                widgetSetA.TodaySameDayLastWeekWrapper.OrderItems = db.OrderItems
                   .Where(x => x.created_date >= todayStartLastWeek && x.created_date <= todayEndLastWeek)
                   .FilterCompsAndVoids()
                  .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id).
             ToList();

                /////////////////////////
                //yesterday
                widgetSetA.YesterdaysOrders.Orders = db.Orders
                 .Where(x => x.created_date >= yestedayStart && x.created_date <= yesterdayEnd)
                 .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                 .Where(x => x.bill_parent == null)
                 .ToList();

                var yesterdayORdersIdsToExlucde =
                   db.Orders
               .Where(x => x.created_date >= yestedayStart && x.created_date <= yesterdayEnd)
                   .Where(x => x.bill_parent != null)
                   .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                   .Select(x => x.order_id)
                   .ToList();

                widgetSetA.YesterdaysOrders.OrderItems = db.OrderItems
                   .Where(x => x.created_date >= yestedayStart && x.created_date <= yesterdayEnd)
                   .FilterCompsAndVoids()
                  .Where(y => y.establishment_id == widgetSetA.RevelEstablishment.establishment_id).
             ToList();


                /* THIS CODE CAN BE REMOVED - THESE ARE INITALISED BY OTHER MEANS (ABOVE)
              widgetSetA.TodaysOrdersSoFar = new RevelOrderandOrderItemWrapper(widgetSetA.today, widgetSetA.today.AddDays(1), RevelOrderandOrderItemWrapper.WrapperType.Full);
              widgetSetA.TodaysOrdersSoFar = await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.TodaysOrdersSoFar);



              widgetSetA.YesterdaysOrders = new RevelOrderandOrderItemWrapper(widgetSetA.today.AddDays(-1), widgetSetA.today, RevelOrderandOrderItemWrapper.WrapperType.Full);

                                                          widgetSetA.YesterdaysOrders = await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.YesterdaysOrders);

                              //same day last week
             widgetSetA.TodaySameDayLastWeekWrapper = new RevelOrderandOrderItemWrapper(widgetSetA.TodaySameDayLastWeekMinusOne, widgetSetA.TodaySameDayLastWeek, RevelOrderandOrderItemWrapper.WrapperType.Full);

              widgetSetA.TodaySameDayLastWeekWrapper = await widgetSetA.revelFactory.PopulateOrderAndItemWrapper(widgetSetA.TodaySameDayLastWeekWrapper);
              */


                //payments
                var controller = new TestController();

                widgetSetA.PaymentTodaySoFar =
                    await controller.GetPaymentsFromDB(widgetSetA.today, widgetSetA.today.AddDays(1));
                widgetSetA.PaymentsYesterday =
                    await controller.GetPaymentsFromDB(widgetSetA.today.AddDays(-1), widgetSetA.today);
                widgetSetA.PaymentsTodaySameDayLastWeek =
                    await
                        controller.GetPaymentsFromDB(widgetSetA.TodaySameDayLastWeekMinusOne,
                            widgetSetA.TodaySameDayLastWeek);


                //added for rolling 6

                //filter to correct establishment

                widgetSetA.PaymentTodaySoFar =
                    widgetSetA.PaymentTodaySoFar.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();

                widgetSetA.PaymentsYesterday = widgetSetA.PaymentsYesterday.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();

                widgetSetA.PaymentsTodaySameDayLastWeek = widgetSetA.PaymentsTodaySameDayLastWeek.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).ToList();
                //assign product breakdown
                widgetSetA.TodaysBreakdown = widgetSetA.pcWrapper.GetProductCategoryBreakdown(widgetSetA.TodaysOrdersSoFar.OrderItems);
                widgetSetA.sameDayLastWeekBreakdown = widgetSetA.pcWrapper.GetProductCategoryBreakdown(widgetSetA.TodaySameDayLastWeekWrapper.OrderItems);

                var prodClassService = new ProductClassService("408d6c05f2864ece90c037333d64f333:9ae943831e7f443b9edf3a6203e66598290fc7d2f3244ca9b69dd67404aa39f2", "https://shoreditchgrind.revelup.com/", db);
                var orderItemIdentificationService = new OrderItemClassIdentificationService(prodClassService, widgetSetA.pcWrapper.Products, prodClasses);

                var errorItemsToday = new List<OrderItem>();
                var errorItemsYesterday = new List<OrderItem>();

                //init breakdown for today
                var TodayOk = widgetSetA.AssignAllItemsToCalculateSpend(widgetSetA.TodaysOrdersSoFar, orderItemIdentificationService, widgetSetA.pcWrapper,
                    out widgetSetA.valueOfAlcoholSales,
                    out widgetSetA.valueOfFoodSales,
                    out widgetSetA.FoodItems,
                    out widgetSetA.valueOfSoftDrinkSales,
                    out widgetSetA.NoOfHotDrinks,
                    out widgetSetA.anythingElse,
                    out errorItemsToday);

                //init breakdown for sameday last week
                var YesterdayOk = widgetSetA.AssignAllItemsToCalculateSpend(widgetSetA.TodaySameDayLastWeekWrapper, orderItemIdentificationService, widgetSetA.pcWrapper,
                    out widgetSetA.sameDayLastWeekvalueOfAlcoholSales,
                    out widgetSetA.sameDayLastWeekvalueOfFoodSales,
                      out widgetSetA.FoodItemsSameDayLastWeek,
                    out widgetSetA.sameDayLastWeekvalueOfSoftDrinkSales,
                    out widgetSetA.sameDayLastWeekNoOfHotDrinks,
                    out widgetSetA.sameDayLastWeekanythingElse,
                    out errorItemsYesterday);


                //get avg time for coffees
                IList<Product> errorProducts = new List<Product>();
                var TodaysProduct = new List<OrderItem>();

                List<Product> hotDrink = widgetSetA.pcWrapper.GetProductsThatAreHotDrinksByClass(prodClasses, out errorProducts);
                foreach (var item in widgetSetA.TodaysOrdersSoFar.OrderItems)
                {
                    if (widgetSetA.pcWrapper.isItemHotDrink(item, hotDrink, out errorProducts))
                        TodaysProduct.Add(item);
                }
                widgetSetA.coffeeServiceTimeAvgToday = GenericRevelMethods.GetAverageTimeOfServiceInSeconds(TodaysProduct);


                var yesterdaysProduct = new List<OrderItem>();
                foreach (var item in widgetSetA.YesterdaysOrders.OrderItems)
                {
                    if (widgetSetA.pcWrapper.isItemHotDrink(item, hotDrink, out errorProducts))
                        yesterdaysProduct.Add(item);
                }
                widgetSetA.coffeeServiceTimeAvgYest =
                 GenericRevelMethods.GetAverageTimeOfServiceInSeconds(yesterdaysProduct.ToList());


                //use factory to push the correct data to widget
                List<GeckoboardObject> alltheWidgets = new List<GeckoboardObject>();

                //add each widget to the list then push them to gecko -- datasets
                //where are binding the URLs??
                //needs to be a mapping table e.g. 1/2/3/4/5/6/7/8 corresponding to widgetOrder or ID
                //for each grind
                //1
                string url;


                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(1, out url);

                var widget1 = widgetSetA.factory.CreateNumberSecondaryStat(1, "TodaySameDayLastWeek",
                    url,
                    "Today", (int)Decimal.Round(widgetSetA.PaymentTodaySoFar.Sum(x => x.amount)), "Same Day Last Week",
                (int)Decimal.Round(widgetSetA.PaymentsTodaySameDayLastWeek.Sum(x => x.amount)));

                alltheWidgets.Add(widget1);



                //TO DO!
                //2
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(2, out url); //set url param here for pushURL

                decimal avgSpend = 0.00M;
                if (widgetSetA.PaymentTodaySoFar.Count > 0 /*&& widgetSetA.TodaysOrdersSoFar.Orders.Count >0*/)
                {
                    try
                    {
                        var sumPaymentAmount = (widgetSetA.PaymentTodaySoFar.Sum(x => x.amount));
                        var sumTaxAmount = widgetSetA.TodaysOrdersSoFar.Orders.Sum(x => x.tax);
                        var orderCount = widgetSetA.TodaysOrdersSoFar.Orders.Count();

                        avgSpend =
                            Decimal.Round(
                                ((sumPaymentAmount - sumTaxAmount) / orderCount),
                                2);
                    }
                    catch (Exception)
                    {


                    }
                }


                var avgSpendYesterday = 0.00M;
                if (widgetSetA.PaymentsTodaySameDayLastWeek.Count > 0 && widgetSetA.PaymentTodaySoFar.Count > 0)
                {

                    try
                    {
                        var sumPaymentAmount = (widgetSetA.PaymentsTodaySameDayLastWeek.Sum(x => x.amount));
                        var sumTaxAmount = widgetSetA.TodaySameDayLastWeekWrapper.Orders.Sum(x => x.tax);
                        var orderCount = widgetSetA.TodaySameDayLastWeekWrapper.Orders.Count();

                        avgSpendYesterday =
                       Decimal.Round(
                                 ((sumPaymentAmount - sumTaxAmount) / orderCount),
                                 2);
                    }
                    catch (Exception)
                    {


                    }
                }
                var widget2 = widgetSetA.factory.CreateNumberSecondaryStat(2, "AvgSpend",
                url,
               "Today", (int)(avgSpend * 100), "Same Day Last Week",
               (int)(avgSpendYesterday * 100));




                alltheWidgets.Add(widget2);
                //3
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(3, out url); //set url param here for pushURL


                var widget3 = widgetSetA.factory.CreateNumberSecondaryStat(3, "NoOfOrdersToday",
                url,
                "Today", (int)widgetSetA.PaymentTodaySoFar.Count(), "Same Day Last Week",
                (int)widgetSetA.PaymentsTodaySameDayLastWeek.Count());

                alltheWidgets.Add(widget3);

                //4
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(4, out url); //set url param here for pushURL
                var widget4 =
                widgetSetA.factory.CreateNumberSecondaryStat(4, "AlcoholSalesToday",
                url,
                "Today", Convert.ToInt32(Math.Round(widgetSetA.valueOfAlcoholSales, 2)), "Same Day Last Week",
                Convert.ToInt32(Math.Round(widgetSetA.sameDayLastWeekvalueOfAlcoholSales, 2)));


                alltheWidgets.Add(widget4);
                //5

                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(5, out url); //set url param here for pushURL
                var NoOfHotDrinksWidget = widgetSetA.factory.CreateNumberSecondaryStat(5, "NoOfHotDrinks",
                 url,
                "Today", widgetSetA.NoOfHotDrinks, "Same Day Last Week",
                widgetSetA.sameDayLastWeekNoOfHotDrinks);

                alltheWidgets.Add(NoOfHotDrinksWidget);

                //6
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(6, out url); //set url param here for pushURL
                var widget6 = widgetSetA.factory.CreateNumberSecondaryStat(6, "FoodSalesToday",
                url,
                "Today", Convert.ToInt32(Math.Round(widgetSetA.valueOfFoodSales, 2)), "Same Day Last Week",
                Convert.ToInt32(Math.Round(widgetSetA.sameDayLastWeekvalueOfFoodSales, 2)));
                alltheWidgets.Add(widget6);

                //7
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(7, out url); //set url param here for pushURL
                var widget7 = widgetSetA.factory.CreateText(7, "TodaysOrders", url, new List<Item_Text>
                {
                    new Item_Text(DateTime.Now.ToString(),1)

                });
                alltheWidgets.Add(widget7);


                //8
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(8, out url); //set url param here for pushURL
                var widget8 = widgetSetA.factory.CreateNumberSecondaryStat(8, "NoValueOfSoftDrinks",
                url,
                "Today", Convert.ToInt32(Math.Round(widgetSetA.valueOfSoftDrinkSales, 2)), "Same Day Last Week",
                Convert.ToInt32(Math.Round(widgetSetA.sameDayLastWeekvalueOfSoftDrinkSales, 2)));

                alltheWidgets.Add(widget8);

                //9
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(9, out url); //set url param here for pushURL

                //ITEM MAPPING DATA
                Dictionary<int, decimal> HourAndSpend = new Dictionary<int, decimal>();

                //do the hours from 6 am - 24.00
                for (int i = 6; i < 24; i++)
                {
                    var currentAccumulatedHourlySpend = 0.00M;
                    int currentHour = i;

                    //if the first two digits of the order match our range
                    foreach (var payment in widgetSetA.PaymentTodaySoFar)
                    {
                        int hourOfOrder = Convert.ToInt16(((DateTime)payment.created_date).ToString("HH"));


                        if (hourOfOrder.Equals(currentHour))
                        {

                            currentAccumulatedHourlySpend += payment.amount;
                        }

                    }

                    /*var testHour = currentHour;
                        var testtest = */

                    //subtract tax and discount for that hour
                    var tax = 0.00M;

                    if (widgetSetA.TodaysOrdersSoFar.Orders.Where(
                            x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour).Any())
                    {
                        tax = widgetSetA.TodaysOrdersSoFar.Orders.Where(
                               x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour)
                               .Sum(x => x.tax);
                    }

                    //we've done the hour, now add to the dictionary
                    HourAndSpend.Add(currentHour, currentAccumulatedHourlySpend /*- tax*/);
                }

                //do the hours from 24.00 - 03.00
                for (int i = 0; i <= 3; i++)
                {
                    var currentAccumulatedHourlySpend = 0.00M;
                    int currentHour = i;

                    //if the first two digits of the order match our range
                    foreach (var payment in widgetSetA.PaymentTodaySoFar)
                    {
                        int hourOfOrder = Convert.ToInt16(((DateTime)payment.created_date).ToString("HH"));

                        if (hourOfOrder.Equals(currentHour))
                        {

                            currentAccumulatedHourlySpend += payment.amount;
                        }

                    }

                    //subtract tax and discount for that hour
                    var tax = 0.00M;

                    if (widgetSetA.TodaysOrdersSoFar.Orders.Where(
                            x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour).Any())
                    {
                        tax = widgetSetA.TodaysOrdersSoFar.Orders.Where(
                               x => Convert.ToInt16(x.created_date.ToString("HH")) == currentHour)
                               .Sum(x => x.tax);
                    }

                    //we've done the hour, now add to the dictionary
                    HourAndSpend.Add(currentHour, currentAccumulatedHourlySpend /*- tax*/);
                }



                var newHourAndSpend = new Dictionary<int, decimal>();



                //create widget items
                List<string> axisX = new List<string>(); //time
                List<decimal> axisY = new List<decimal>(); //cash

                //get max spend from hour and spend - that's our top level spend. 
                //y axis is now this. 
                var maxSpend = HourAndSpend.Values.Max();

                for (decimal y = 0.0M; y <= 1.0M; y = y + 0.2M)
                {
                    axisY.Add(Decimal.Round(y * maxSpend, 2));
                }

                foreach (var item in HourAndSpend.Keys)
                {
                    axisX.Add(item.ToString());
                }


                LineSettings settings = new LineSettings
                {
                    axisy = axisY,
                    axisx = axisX,
                    colour = null

                };

                //create items
                List<decimal> items = new List<decimal>();
                foreach (var item in HourAndSpend.Values)
                {
                    items.Add(item);
                }

                //create widget
                var widget9 = widgetSetA.factory.CreateLine(9, "HourAndSpend", url, items, settings);



                alltheWidgets.Add(widget9);



                var totalDiscountToday = 0.00M;

                foreach (var item in widgetSetA.TodaysOrdersSoFar.OrderItems)
                {
                    if (item.discount_amount > 0.00M)
                    {
                        totalDiscountToday += item.pure_sales;
                        if (item.discount_taxed.Equals(true))
                        {
                            totalDiscountToday += item.tax_amount;
                        }
                    }

                }


                var totalDiscountTodayLastWeek = 0.00M;

                foreach (var item in widgetSetA.TodaySameDayLastWeekWrapper.OrderItems)
                {
                    if (item.discount_amount > 0.00M)
                    {
                        totalDiscountTodayLastWeek += item.pure_sales;
                        if (item.discount_taxed.Equals(true))
                        {
                            totalDiscountTodayLastWeek += item.tax_amount;
                        }
                    }

                }


                //10
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(10, out url); //set url param here for pushURL
                var widget10 = widgetSetA.factory.CreateNumberSecondaryStat(5, "Discount",
                 url,
                "Today", Convert.ToInt32(Math.Round(totalDiscountToday, 2)), "Same Day Last Week",

                Convert.ToInt32(Math.Round(totalDiscountTodayLastWeek, 2))

                //last week discounts -???
                );

                alltheWidgets.Add(widget10);

                //11
                //moved from Overnight to run on dailies 
                //rolling 6 days
                var now6DaysAgo = DateTime.Now.AddHours(-168).Date;
                /*      widgetSetA.RollingPast6DaysToTodayStart = new DateTime(now6DaysAgo.Year, now6DaysAgo.Month, now6DaysAgo.Day, now6DaysAgo.Hour, 00, 00);
                      widgetSetA.RollingPast6DaysToTodayEnd = DateTime.Now;

                      var now6DaysAgoLastWeek = widgetSetA.RollingPast6DaysToTodayStart.AddDays(-7);
                      widgetSetA.RollingPast6DaysLastWeekStart = new DateTime(now6DaysAgoLastWeek.Year, now6DaysAgoLastWeek.Month, now6DaysAgoLastWeek.Day, now6DaysAgoLastWeek.Hour, 00, 00);
                      widgetSetA.RollingPast6DaysLastWeekEnd = widgetSetA.RollingPast6DaysToTodayEnd.AddDays(-7);*/
                //end


                var testst =
                //107 rolling past 6 days plus today PIT
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(107, out url);



                var RollingPast6DaysToToday = 0;
                var RollingPast6DaysLastWeek = 0;
                try
                {
                    RollingPast6DaysToToday = (int)(paymentsRollingPast6DaysToToday.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Sum(x => x.amount));
                    RollingPast6DaysLastWeek = (int)(paymentRollingPast6DaysLastWeek.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Sum(x => x.amount));
                }
                catch (Exception)
                {


                }


                var rollingAmount = 0.00M;
                try
                {
                    rollingAmount = paymentRollingPast6DaysLastWeek.Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Sum(x => x.amount);
                }
                catch (Exception)
                {


                }
                NumberSecondaryStat RollingPast6DaysToTodayWidget = widgetSetA.factory.CreateNumberSecondaryStat(107, "RollingPast6DaysToTodayWidget",
                       url,
                       "RollingPast6DaysToToday", RollingPast6DaysToToday
                       ,
                       "RollingPast6DaysLastWeek",
                       (int)rollingAmount);

                alltheWidgets.Add(RollingPast6DaysToTodayWidget);


                try
                {
                    //11
                    if (Double.IsNaN(widgetSetA.coffeeServiceTimeAvgToday))
                    {
                        widgetSetA.coffeeServiceTimeAvgToday = 0.00;
                        widgetSetA.coffeeServiceTimeAvgYest = 0.00;
                    }


                    widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(11, out url);

                    NumberSecondaryStat AvgCoffeeServiceTime = widgetSetA.factory.CreateNumberSecondaryStat(11,
                        "AvgCoffeeServiceTime",
                        url,
                        "AvgCoffeeServiceTimeToday",
                        Convert.ToInt32(widgetSetA.coffeeServiceTimeAvgToday),
                        "AvgCoffeeServiceTimeYesterday",
                        Convert.ToInt32(widgetSetA.coffeeServiceTimeAvgYest));

                    alltheWidgets.Add(AvgCoffeeServiceTime);
                }
                catch (Exception)
                {


                }



                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(12, out url);

                var unpaidToday =
                    widgetSetA.TodaysOrdersSoFar.Orders.Where(x => x.is_unpaid == "True" && x.closed == false)
                        .Sum(x => x.final_total);



                NumberSecondaryStat openAndUnpaidOrders = widgetSetA.factory.CreateNumberSecondaryStat(12,
                    "OpenAndUnpaidOrders",
                    url,
                    "OpenAndUnpaidOrdersToday",
                    Convert.ToInt32(unpaidToday),
                    "OpenAndUnpaidOrdersYesterday",
                    Convert.ToInt32(0));

                alltheWidgets.Add(openAndUnpaidOrders);


                //Added Bullet chart for bucket week
                DateTime ThisWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday);
                DateTime LastWeekStart = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays(-7), DayOfWeek.Monday);

                var ordersSinceMonday = db.Payments
                    .Where(x => x.created_date >= ThisWeekStart)
                    .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                    .AsNoTracking()
                    .Select(x => x.amount)
                    .ToList()
                    .Sum();

                var taxSinceMonday = db.Orders
                    .Where(x => x.created_date >= ThisWeekStart)
                    .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                    .AsNoTracking()
                    .Select(x => x.tax)
                    .ToList()
                    .Sum();

                var finalPaymentsThisWeek = ordersSinceMonday - taxSinceMonday;

                //RUN OUT OF CALENDAR - BUDGET = 0
                //var calendar = _445Calendar.GetCurrentWeek(db);
                //var budget = calendar.Projections.FirstOrDefault(x => x.Establishment.DBKEY_establishment_id == widgetSetA.RevelEstablishment.DBKEY_establishment_id);
                Projection budget = null;

                var lastWeekSales = db.Payments
                   .Where(x => x.created_date >= LastWeekStart && x.created_date <= ThisWeekStart)
                   .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                   .AsNoTracking()
                   .Select(x => x.amount)
                   .ToList()
                   .Sum();


                var lastWeekTax = db.Orders
                     .Where(x => x.created_date >= LastWeekStart && x.created_date <= ThisWeekStart)
                      .Where(x => x.establishment_id == widgetSetA.RevelEstablishment.establishment_id)
                      .AsNoTracking()
                        .Select(x => x.tax)
                    .ToList()
                    .Sum();

                var finalPaymenetLastweek = lastWeekSales - lastWeekTax;

                //create bullet
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(13, out url); //set url param here for pushURL
                if (budget == null)
                {
                    budget = new Projection
                    {
                        Establishment = new Establishment(),
                        ProjectionFigure = 0,



                    };
                }
                BulletItem ChartItem = new BulletItem
                {
                    label = "This Week (NET£)",
                    sublabel = "vs Budget (Green)",

                    axis = new BulletAxis
                    {
                        point = new List<string>
                       {
                          "0",(Convert.ToInt32(budget.ProjectionFigure * 0.25M)).ToString(),(Convert.ToInt32(budget.ProjectionFigure * 0.50M)).ToString(),Convert.ToInt32(budget.ProjectionFigure * 0.75M).ToString(),(Convert.ToInt32(budget.ProjectionFigure * 1.0M)).ToString(),(Convert.ToInt32(budget.ProjectionFigure * 1.25M)).ToString()
                       }
                    },

                    range = new List<BulletRange>
                   {
                       new BulletRange{color="red", start=0, end=Convert.ToInt32(budget.ProjectionFigure-1M)},
                               new BulletRange{color="green", start=Convert.ToInt32(budget.ProjectionFigure), end=Convert.ToInt32(budget.ProjectionFigure * 1.25M)}

                   },

                    measure = new BulletMeasure
                    {
                        current = new BulletMeasureItem { start = "0", end = Convert.ToInt32(finalPaymentsThisWeek).ToString() },
                        projected = new BulletMeasureItem { start = "0", end = "0" }

                    },

                    comparative = new BulletComparative { point = finalPaymenetLastweek.ToString() }
                };

                var bulletWeeklyBudget = widgetSetA.factory.CreateBullet(13, "BulletWeeklyBudget", url, "horizontal", ChartItem);

                alltheWidgets.Add(bulletWeeklyBudget);

                //14

                ///DO CALCS FOR FOOD

                GrindItemSalesPeriod identifierService = new GrindItemSalesPeriod();

                var breakfastTotal = 0.00M;
                var lunchTotal = 0.00M;
                var dinnerTotal = 0.00M;

                var yesterdayBreakfastTotal = 0.00M;
                var yesterdayLunchTotal = 0.00M;
                var yesterdaydinnerTotal = 0.00M;



                //get today's totals
                var aggreagator = new Order_OrderItemItemPeriodAggregator();

                aggreagator.GetBreakfastLunchDinnerTotals(widgetSetA.FoodItems, widgetSetA.TodaysOrdersSoFar.Orders, DateTime.Now, out breakfastTotal, out lunchTotal, out dinnerTotal
                  );

                aggreagator.GetBreakfastLunchDinnerTotals(widgetSetA.FoodItemsSameDayLastWeek, widgetSetA.TodaySameDayLastWeekWrapper.Orders, DateTime.Now.AddDays(-7), out yesterdayBreakfastTotal, out yesterdayLunchTotal, out yesterdaydinnerTotal
                 );

                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(14, out url); //set url param here for pushURL

                var widget14 = widgetSetA.factory.CreateNumberSecondaryStat(14, "BreakfastFoodSalesToday",
                url,
                "Today", Convert.ToInt32(breakfastTotal), "Same Day Last Week",
                Convert.ToInt32(yesterdayBreakfastTotal)
                );

                alltheWidgets.Add(widget14);

                //15
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(15, out url); //set url param here for pushURL
                var widget15 = widgetSetA.factory.CreateNumberSecondaryStat(15, "LunchFoodSalesToday",
               url,
                "Today", Convert.ToInt32(lunchTotal), "Same Day Last Week",
                Convert.ToInt32(yesterdayLunchTotal));

                alltheWidgets.Add(widget15);

                //16
                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(16, out url); //set url param here for pushURL
                var widget16 = widgetSetA.factory.CreateNumberSecondaryStat(16, "DinnerFoodSalesToday",
                url,
                "Today", Convert.ToInt32(dinnerTotal), "Same Day Last Week",
                Convert.ToInt32(yesterdaydinnerTotal));

                alltheWidgets.Add(widget16);

                //17
                //get last fixed monday
                //last week
                var LastWeekStartBase = DateTimeExtensions.StartOfWeek(DateTime.Now.AddDays((-7)), DayOfWeek.Monday);

                widgetSetA.LastWeekStart = new DateTime(LastWeekStartBase.Year, LastWeekStartBase.Month,
                    LastWeekStartBase.Day, 03, 00, 00);
                widgetSetA.LastWeekEnd = widgetSetA.LastWeekStart.AddDays(7);

                //week before last
                widgetSetA.WeekBeforeLastStart = widgetSetA.LastWeekStart.AddDays(-7);
                widgetSetA.WeekBeforeLastEnd = widgetSetA.WeekBeforeLastStart.AddDays(7);


                widgetSetA.WidgetURLBindings.widgetBindMappings.TryGetValue(17, out url); //set url param here for pushURL
                var widget17 = widgetSetA.factory.CreateNumberSecondaryStat(17, "ServiceCharge",
                url,
                "Today", Convert.ToInt32(db.Orders.Where(x => x.created_date >= widgetSetA.LastWeekStart && x.created_date <= widgetSetA.LastWeekEnd && x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Sum(x => x.gratuity)), "Service Charge",
                Convert.ToInt32(db.Orders.Where(x => x.created_date >= widgetSetA.WeekBeforeLastStart && x.created_date <= widgetSetA.WeekBeforeLastEnd && x.establishment_id == widgetSetA.RevelEstablishment.establishment_id).Sum(x => x.gratuity)));

                alltheWidgets.Add(widget17);



                //end widgets
                widgetSetA.theWidgetCollection = alltheWidgets;

                return widgetSetA;
            }
            catch (Exception ex)
            {

                throw new Exception("Couldn't initalise widget set - establishment:" + widgetSetA.RevelEstablishment.name, ex);
            }


        }

        public async Task<bool> PushWidgetsToGeckoboard(WidgetSetA widgetSetA)
        {

            foreach (var widget in widgetSetA.theWidgetCollection)
            {
                await widgetSetA.pushService.Push(widget);
            }

            return true;
        }
    }
}
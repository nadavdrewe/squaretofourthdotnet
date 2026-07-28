using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using GeckoboardLibrary.Services;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes.ServiceImplemenations;

namespace GeckoboardTestWebApp.Models
{

    public class WidgetSetA
    {
        //Unique class properties
        public List<GeckoboardObject> theWidgetCollection = new List<GeckoboardObject>();
        public EstablishmentBindingTable WidgetURLBindings { get; set; }
        //REVEL
        public Establishment RevelEstablishment { get; set; }
        public RevelProductAndCategoryWrapper pcWrapper { get; set; }
        public IRevelFactoryAsync revelFactory { get; set; }
        //GECKO
        public GeckoboardOrganisation shoreditchGrindOrganisation { get; set; }
        public IGeckoboardObjectCreatorFactory factory { get; set; }
        public GeckoboardPushService pushService { get; set; }

        //PRODUCTS
        public List<Product> alcoholProducts { get; set; }
        public List<Product> foodProducts { get; set; }
        public List<Product> hotDrinkProducts { get; set; }


        //DATES

        public DateTime today { get; set; }



        //30 days previous
        public DateTime ThirtyDaysToYesterdayEnd { get; set; }
        public DateTime ThirtyDaysToYesterdayStart { get; set; }

        //30 days before that
        public DateTime ThirtyDaysPreviousEnd { get; set; }
        public DateTime ThirtyDaysPreviousStart { get; set; }

        //30 days previous, last year
        public DateTime ThirtyDaysToYesterdayEndLastYear { get; set; }
        public DateTime ThirtyDaysToYesterdayStartLastYear { get; set; }


        public DateTime yesterday { get; set; }

        public DateTime yesterDayLastWeek { get; set; }

        public DateTime LastWeekStart { get; set; }
        public DateTime LastWeekEnd { get; set; }

        public DateTime WeekBeforeLastStart { get; set; }
        public DateTime WeekBeforeLastEnd { get; set; }


        public DateTime RollingPast6DaysToTodayStart { get; set; }
        public DateTime RollingPast6DaysToTodayEnd { get; set; }

        public DateTime RollingPast6DaysLastWeekStart { get; set; }
        public DateTime RollingPast6DaysLastWeekEnd { get; set; }


        //3
        public int LastMonth { get; set; }
        public DateTime FirstDayOfLastMonth { get; set; }
        public DateTime FirstDayOfThisMonth { get; set; }

        public int lastMonthLastYearInt = DateTime.Now.AddMonths(-1).AddYears(-1).Month;

        public DateTime FirstDayOfLastMonthLastYear { get; set; }
        public DateTime FirstDayOfThisMonthLastYear { get; set; }



        //dates
        public DateTime ThisYearStart { get; set; }
        public DateTime YTDYesterday { get; set; }

        public DateTime LastYearStart { get; set; }
        public DateTime LastYearYesterday { get; set; }


        public Dictionary<string, int> TodaysBreakdown { get; set; }

        public int NoOfHotDrinks = 0;
        public int NoOfSoftDrinks = 0;
        public decimal valueOfFoodSales = 0.00M;
        public decimal valueOfAlcoholSales = 0.00M;
        public decimal valueOfSoftDrinkSales = 0.00M;
        public decimal valueOfFoodInitialPrice = 0.00M;
        public decimal valueOfAlcoholInitialPrice = 0.00M;
        public decimal valueOfAlcoholPlusTax = 0.00M;
        public decimal valueOfFoodPlusTax = 0.00M;

        //////////////////
        public double coffeeServiceTimeAvgToday { get; set; }
        public double coffeeServiceTimeAvgYest { get; set; }

        public Dictionary<string, int> sameDayLastWeekBreakdown { get; set; }
        public int sameDayLastWeekNoOfHotDrinks = 0;
        public int sameDayLastWeekNoOfSoftDrinks = 0;

        public decimal sameDayLastWeekvalueOfFoodSales = 0.00M;
        public decimal sameDayLastWeekvalueOfAlcoholSales = 0.00M;
        public decimal sameDayLastWeekvalueOfFoodInitialPrice = 0.00M;
        public decimal sameDayLastWeekvalueOfAlcoholInitialPrice = 0.00M;
        public decimal sameDayLastWeekvalueOfSoftDrinkSales = 0.00M;

        public List<OrderItem> FoodItems = new List<OrderItem>();
        public List<OrderItem> FoodItemsSameDayLastWeek = new List<OrderItem>();

        public decimal sameDayLastWeekvalueOfAlcoholPlusTax = 0.00M;
        public decimal sameDayLastWeekvalueOfFoodPlusTax = 0.00M;

        ////////////////
        //PAYMENTS
        //

        //Daily
        public List<Payment> PaymentTodaySoFar { get; set; }
        public List<Payment> PaymentsYesterday { get; set; }
        public List<Payment> PaymentsTodaySameDayLastWeek { get; set; }

        //overnight

        /////////////////////////////
        //ORDER ITEM WRAPPERS
        public RevelOrderandOrderItemWrapper TodaysOrdersSoFar { get; set; }
        public RevelOrderandOrderItemWrapper YesterdaysOrders { get; set; }
        public List<OrderItem> anythingElse = new List<OrderItem>();
        public List<OrderItem> sameDayLastWeekanythingElse = new List<OrderItem>();
        public RevelOrderandOrderItemWrapper TodaySameDayLastWeekWrapper { get; set; }

        //OVERNIGHT ORDER ITEM WRAPPERS
        //////////
        public RevelOrderandOrderItemWrapper last30 { get; set; }
        public RevelOrderandOrderItemWrapper last30Previous { get; set; }
        public RevelOrderandOrderItemWrapper last30LastYear { get; set; }

        public RevelOrderandOrderItemWrapper lastWeek { get; set; }
        public RevelOrderandOrderItemWrapper weekBeforeLast { get; set; }

        public RevelOrderandOrderItemWrapper yesterdaysOrders { get; set; }
        public RevelOrderandOrderItemWrapper yesterdaysLastWeekOrders { get; set; }

        public RevelOrderandOrderItemWrapper lastMonth { get; set; }
        public RevelOrderandOrderItemWrapper lastMonthLastYear { get; set; }

        public RevelOrderandOrderItemWrapper ThisYearStartToTodayWrapper { get; set; }
        public RevelOrderandOrderItemWrapper LastYearStartToTodayWrapper { get; set; }


        //WIDGET ONE
        public DateTime TodaySameDayLastWeekMinusOne = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-7));
        public DateTime TodaySameDayLastWeek = RevelHelper.WrapAllRevelStartingDatesInThisMethod(DateTime.Now.AddDays(-7));

        /// <summary>
        /// WIDGETS
        /// </summary>
        //ONE
        NumberSecondaryStat ThisYearStartToTodayWidget { get; set; }
        //TWO
        Text AvgSpendToday { get; set; }
        //THREE
        NumberSecondaryStat NoOfOrdersToday { get; set; }
        //FOUR
        NumberSecondaryStat AlcoholSalesToday { get; set; }
        //FIVE
        NumberSecondaryStat NoOfHotDrinksWidget { get; set; }
        //SIX
        NumberSecondaryStat FoodSalesToday { get; set; }

        //SEVEN
        Text LastUpdated { get; set; }

        //EIGHT
        NumberSecondaryStat NoOfSoftDrinksWidget { get; set; }

        //NINE        
        Line hourAndSpendLine { get; set; }







        public WidgetSetA()
        {
            //GECKOSETUP
            this.pushService = new GeckoboardPushService();

            this.theWidgetCollection = new List<GeckoboardObject>();

            //end const


            //DAILY
            //One
            NumberSecondaryStat TodaySameDayLastWeek;
            //TWO
            Text AvgSpendToday;
            //THREE
            NumberSecondaryStat NoOfOrdersToday;
            //FOUR
            NumberSecondaryStat AlcoholSalesToday;
            //FIVE
            NumberSecondaryStat NoOfHotDrinksWidget;
            //SIX
            NumberSecondaryStat FoodSalesToday;
            //SEVEN
            Text LastUpdated;
            //EIGHT
            NumberSecondaryStat NoOfSoftDrinksWidget;
            //Nine
            Line HourlySpend;



            //OVERNIGHT
            //101
            NumberSecondaryStat LastWeekWeekBefore;
            //102
            NumberSecondaryStat yesterdayVSYesterdayLastWeek;
            //103
            NumberSecondaryStat lastMonthVSBudget;
            //104
            Bullet LASTMONTHVSLASTYEARBUDGETBULLET;
            //105
            NumberSecondaryStat ThisYearStartToDateVsLastYear;
            //106
            Bullet Last30Days;
            theWidgetCollection.Add(TodaySameDayLastWeek = new NumberSecondaryStat());
            theWidgetCollection.Add(AvgSpendToday = new Text());
            theWidgetCollection.Add(NoOfOrdersToday = new NumberSecondaryStat());
            theWidgetCollection.Add(AlcoholSalesToday = new NumberSecondaryStat());
            theWidgetCollection.Add(NoOfHotDrinksWidget = new NumberSecondaryStat());
            theWidgetCollection.Add(FoodSalesToday = new NumberSecondaryStat());
            theWidgetCollection.Add(LastUpdated = new Text());
            theWidgetCollection.Add(NoOfSoftDrinksWidget = new NumberSecondaryStat());
            theWidgetCollection.Add(HourlySpend = new Line());

        }


        //DONE ON PURE SALES, BAR HOT DRINKS NUMBER
        public bool AssignAllItemsToCalculateSpend(RevelOrderandOrderItemWrapper wrapper, OrderItemClassIdentificationService orderItemIdentificationService,
            RevelProductAndCategoryWrapper prodCatWrapper,
            out decimal alcoholVar,
            out decimal foodVar,
            out List<OrderItem> FoodItems,
            out decimal softDrinksVar,
            out int noOfHotDrinksVar,
            out List<OrderItem> otherProducts,
            out List<OrderItem> errorItems
            )
        {

            var COUNT_FoodItems = 0;
            var COUNT_BoozeItems = 0;
            var COUNT_SoftDrinkItems = 0;

            alcoholVar = 0.00M;
            foodVar = 0.00M;
            softDrinksVar = 0.00M;
            noOfHotDrinksVar = 0;
            otherProducts = new List<OrderItem>();
            FoodItems = new List<OrderItem>();
            errorItems = new List<OrderItem>();

            foreach (var item in wrapper.OrderItems)
            {
                try
                {
                    if (orderItemIdentificationService.GetItemClassType(item, this.RevelEstablishment.establishment_id).ToLower() == "bar")
                    {
                        alcoholVar += (item.pure_sales);
                        COUNT_BoozeItems += item.quantity;
                    }
                    else if (orderItemIdentificationService.GetItemClassType(item, this.RevelEstablishment.establishment_id).ToLower() == "food")
                    {
                        FoodItems.Add(item);
                        foodVar += (item.pure_sales);
                        COUNT_FoodItems += item.quantity;
                    }
                    else if (orderItemIdentificationService.GetItemClassType(item, this.RevelEstablishment.establishment_id).ToLower() == "coffee/hot drinks")
                    {
                        noOfHotDrinksVar += item.quantity;
                    }
                    else if (
                        orderItemIdentificationService.GetItemClassType(item, this.RevelEstablishment.establishment_id).ToLower() == ("soft drinks") ||
                        orderItemIdentificationService.GetItemClassType(item, this.RevelEstablishment.establishment_id).ToLower() == ("juice")
                        )
                    {
                        softDrinksVar += (item.pure_sales);
                        COUNT_SoftDrinkItems += item.quantity;
                    }
                    else
                    {
                        otherProducts.Add(item);
                    }
                }
                catch (Exception)
                {
                    errorItems.Add(item);
                }

            }

            return true;

        }

    }


}

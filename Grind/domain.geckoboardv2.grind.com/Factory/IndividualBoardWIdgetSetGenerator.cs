using domain.geckoboardv2.grind.com.Models.BoardData;
using domain.geckoboardv2.grind.com.Models.BoardTypeWidgetUrls;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using GeckoboardLibrary.Classes.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.geckoboardv2.grind.com.Factory
{
    public class IndividualBoardWidgetSetGenerator
    {
        IGeckoboardObjectCreatorFactory geckoboardObjectCreatorFactory;

        public IndividualBoardWidgetSetGenerator(string geckoBoardAPIKEY)
        {
            var geckoORg = new GeckoboardOrganisation(geckoBoardAPIKEY, "Railgunit");
            geckoboardObjectCreatorFactory = new GeckoboardObjectCreatorFactory(geckoORg);
        }

        //take in data and just maps to widgets
        public List<GeckoboardObject> GenerateWidgets(IndividualBoardDataset data, IndividualStoreBase widgetUrls)
        {
            List<GeckoboardObject> allWidgets = new List<GeckoboardObject>();

            //first widget - SalesTodayVsLastWeek
            var salesTodayVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  widgetUrls.SalesTodayVsLastWeek,
                  "Today", (int)Decimal.Round(data.SalesTodayVsLastWeek_Today), "Last Week",
              (int)Decimal.Round(data.SalesTodayVsLastWeek_LastWeek));
            allWidgets.Add(salesTodayVsLastWeek);

            //SalesTodayVsBudget
            var salesTodayVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  widgetUrls.SalesTodayVsBudget,
                  "Today", (int)Decimal.Round(data.SalesTodayVsBudget_Today), "Budget",
              (int)Decimal.Round(data.SalesTodayVsBudget_Budget));
            allWidgets.Add(salesTodayVsBudget);

            //disoucntTodayVsBudget
            var disoucntTodayVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  widgetUrls.DiscountTodayVsBudget,
                  "Today", (int)Decimal.Round(data.DiscountTodayVsBudget_Today), "Budget",
              (int)Decimal.Round(data.DiscountTodayVsBudget_Budget));
            allWidgets.Add(disoucntTodayVsBudget);

            //WTDVsLastWeek
            var wTDVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  widgetUrls.WTDVsLastWeek,
                  "Today", (int)Decimal.Round(data.WTDVsLastWeek_Today), "Last Week",
              (int)Decimal.Round(data.WTDVsLastWeek_LastWeek));
            allWidgets.Add(wTDVsLastWeek);

            //WTDVsBudget
            var wTDVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  widgetUrls.WTDVsBudget,
                  "Today", (int)Decimal.Round(data.WTDVsBudget_Today), "Last Week",
              (int)Decimal.Round(data.WTDVsBudget_Budget));
            allWidgets.Add(wTDVsBudget);

            //WTDDiscountVsBudget
            var WTDDiscountVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 widgetUrls.WTDDiscountVsBudget,
                 "Today", (int)Decimal.Round(data.WTDDiscountVsBudget_Today), "Budget",
             (int)Decimal.Round(data.WTDDiscountVsBudget_Budget));
            allWidgets.Add(WTDDiscountVsBudget);

            //CumulativeHourlySales_Today
            //THIS IS GRAPH!!

            //CATS NOW
            //coffeeWTDSalesVsLastWeek            
            var coffeeWTDSalesVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 widgetUrls.CoffeeWTDSalesVsLastWeek,
                 "Today", (int)Decimal.Round(data.CoffeeWTDSalesVsLastWeek_Today), "Last Week",
             (int)Decimal.Round(data.CoffeeWTDSalesVsLastWeek_LastWeek));
            allWidgets.Add(coffeeWTDSalesVsLastWeek);

            //foodWTDSalesVsLastWeek            
            var foodWTDSalesVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 widgetUrls.FoodWTDSalesVsLastWeek,
                 "Today", (int)Decimal.Round(data.FoodWTDSalesVsLastWeek_Today), "Last Week",
             (int)Decimal.Round(data.FoodWTDSalesVsLastWeek_LastWeek));
            allWidgets.Add(foodWTDSalesVsLastWeek);

            //bar
            var barWTDSalesVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                widgetUrls.BarWTDSalesVsLastWeek,
                "Today", (int)Decimal.Round(data.BarWTDSalesVsLastWeek_Today), "Last Week",
            (int)Decimal.Round(data.BarWTDSalesVsLastWeek_LastWeek));
            allWidgets.Add(barWTDSalesVsLastWeek);

            //retail
            var retailWTDSalesVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                widgetUrls.RetailTodayVsLastWeek,
                "Today", (int)Decimal.Round(data.RetailTodayVsLastWeek_Today), "Last Week",
            (int)Decimal.Round(data.RetailTodayVsLastWeek_LastWeek));
            allWidgets.Add(retailWTDSalesVsLastWeek);

            //coffee volumes
            var CoffeeVolumeTodayVsLastWeek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
               widgetUrls.CoffeeVolumeTodayVsLastWeek,
               "Today", (int)Decimal.Round(data.CoffeeVolumeTodayVsLastWeek_Today), "Last Week",
           (int)Decimal.Round(data.CoffeeVolumeTodayVsLastWeek_LastWeek));
            allWidgets.Add(CoffeeVolumeTodayVsLastWeek);

            //covers WTD
            var coversWTD = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
             widgetUrls.CoversWTDVsLastWeek,
             "Today", (int)Decimal.Round(data.CoversWTDVsLastWeek_Today), "Last Week",
            (int)Decimal.Round(data.CoversWTDVsLastWeek_LastWeek));
            allWidgets.Add(coversWTD);

            //Average cover value
            var avgCoverValue = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
             widgetUrls.AverageCoverValue,
             "Today", (int)Decimal.Round(data.AverageCoverValueWTD_Today), "Last Week",
            (int)Decimal.Round(data.AverageCoverValueWTD_LastWeek));
            allWidgets.Add(avgCoverValue);

            ////LINE CHART
            ////setup

            //hour 
            var cumulativeHourAndSpendWidget = new LineV2Widget("ab876212d31d37960e3154eb5e2bc0a0", widgetUrls.CumulativeHourlySales, "", GeckoboardChartAndItemType.LineV2);
            var xAxisHours = data.CumulativeHourAndSpendsToday.Select(x => Convert.ToDateTime(x.Hour).ToString("HH")).ToList();
            var xAxis = new LineV2XAsis
            {
                type = "standard",
                labels = xAxisHours
            };
            cumulativeHourAndSpendWidget.data.x_axis = xAxis;
            cumulativeHourAndSpendWidget.data.y_axis = new LineV2YAxis
            {
                format = "currency",
                unit = "GBP"
            };

            var todaySeries = new LineV2Series { name = "Today", data = data.CumulativeHourAndSpendsToday.Select(X => X.Value).ToList() };
            var budgetSeries = new LineV2Series { name = "Budget", data = data.CumulativeHourAndSpendsBudget.Select(X => X.Value).ToList() };
            var yesterdaySeries = new LineV2Series { name = "Last Week", data = data.CumulativeHourAndSpendsSameDayLastWeeek.Select(X => X.Value).ToList() };

            cumulativeHourAndSpendWidget.data.series = new List<LineV2Series>
            {
                todaySeries,
                budgetSeries,
                yesterdaySeries

            };
            allWidgets.Add(cumulativeHourAndSpendWidget);

            //return widgets
            return allWidgets;
        }
    }
}

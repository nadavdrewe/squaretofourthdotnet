using domain.geckoboardv2.grind.com.Models.BoardData;
using domain.geckoboardv2.grind.com.Models.BoardTypeWidgetUrls;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.geckoboardv2.grind.com.Factory
{
    public class GrindTodayBoardWidgetSetGenerator
    {
        IGeckoboardObjectCreatorFactory geckoboardObjectCreatorFactory;

        public GrindTodayBoardWidgetSetGenerator(string geckoBoardAPIKEY)
        {
            var geckoORg = new GeckoboardOrganisation(geckoBoardAPIKEY, "Railgunit");
            geckoboardObjectCreatorFactory = new GeckoboardObjectCreatorFactory(geckoORg);
        }

        /// <summary>
        /// This does WTD widgets
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<GeckoboardObject> GenerateWTDWidgets(GrindTodayBoardDataset data)
        {
            List<GeckoboardObject> allWidgets = new List<GeckoboardObject>();

            //first widget - todayVsBudgets
            var todayVsBudgets = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  GrindWTD.WidgetEndpoints.WTDVsBudgets,
                  "WTD", (int)Decimal.Round(data.TodayVsBudgets_Today), "WTDBudget",
              (int)Decimal.Round(data.TodayVsBudgets_Budget));
            allWidgets.Add(todayVsBudgets);

            var todayVsLastweek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
          GrindWTD.WidgetEndpoints.WTDVsSameDayLastWeek,
          "WTD", (int)Decimal.Round(data.TodayVsSameDayLastWeek_Today), "Budget",
      (int)Decimal.Round(data.TodayVsSameDayLastWeek_LastWeek));
            allWidgets.Add(todayVsLastweek);

            //shore
            var shore = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindWTD.WidgetEndpoints.Shoreditch,
           "WTD", (int)Decimal.Round(data.Shoreditch_Today), "Budget",
       (int)Decimal.Round(data.Shoreditch_Budget));
            allWidgets.Add(shore);



            var soho = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindWTD.WidgetEndpoints.Soho,
           "WTD", (int)Decimal.Round(data.Soho_Today), "Budget",
       (int)Decimal.Round(data.Soho_Budget));
            allWidgets.Add(soho);

            var london = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindWTD.WidgetEndpoints.London,
           "WTD", (int)Decimal.Round(data.London_Today), "Last Week",
       (int)Decimal.Round(data.London_Budget));
            allWidgets.Add(london);


            var hatton = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindWTD.WidgetEndpoints.Hatton_Garden,
           "WTD", (int)Decimal.Round(data.Hatton_Garden_Today), "Budget",
       (int)Decimal.Round(data.Hatton_Garden_Budget));
            allWidgets.Add(hatton);

            var royal = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindWTD.WidgetEndpoints.Royal_Exchange,
           "WTD", (int)Decimal.Round(data.Royal_Exchange_Today), "Budget",
       (int)Decimal.Round(data.Royal_Exchange_Budget));
            allWidgets.Add(royal);

            var covent = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
          GrindWTD.WidgetEndpoints.Covent_Garden,
          "WTD", (int)Decimal.Round(data.Covent_Garden_Today), "Budget",
          (int)Decimal.Round(data.Covent_Garden_Budget));
            allWidgets.Add(covent);

            var clerk = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindWTD.WidgetEndpoints.Clerkenwell,
            "WTD", (int)Decimal.Round(data.Clerkenwell_Today), "Budget",
            (int)Decimal.Round(data.Clerkenwell_Budget));
            allWidgets.Add(clerk);


            var white = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                GrindWTD.WidgetEndpoints.Whitechapel,
                "WTD", (int)Decimal.Round(data.Whitechapel_Today), "Budget",
                (int)Decimal.Round(data.Whitechapel_Budget));
            allWidgets.Add(white);

            var exmouth = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindWTD.WidgetEndpoints.Exmouth_Market,
            "WTD", (int)Decimal.Round(data.Exmouth_Market_Today), "Budget",
            (int)Decimal.Round(data.Exmouth_Market_Budget));
            allWidgets.Add(exmouth);


            var face = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindWTD.WidgetEndpoints.Facebook,
            "WTD", (int)Decimal.Round(data.Facebook_Today), "Budget",
            (int)Decimal.Round(data.Facebook_Budget));
            allWidgets.Add(face);


            var greenwich = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindWTD.WidgetEndpoints.Greenwich,
            "WTD", (int)Decimal.Round(data.Greenwich_Today), "Budget",
            (int)Decimal.Round(data.Greenwich_Budget));
            allWidgets.Add(greenwich);

            var liverpool = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindWTD.WidgetEndpoints.Liverpool_Street,
           "WTD", (int)Decimal.Round(data.Liverpool_Street_Today), "Budget",
           (int)Decimal.Round(data.Liverpool_Street_Budget));
            allWidgets.Add(liverpool);

            return allWidgets;
        }


        public List<GeckoboardObject> GenerateWidgets(GrindTodayBoardDataset data)
        {
            List<GeckoboardObject> allWidgets = new List<GeckoboardObject>();

            //first widget - todayVsBudgets
            var todayVsBudgets = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  GrindToday.WidgetEndpoints.TodayVsBudgets,
                  "Today", (int)Decimal.Round(data.TodayVsBudgets_Today), "Budget",
              (int)Decimal.Round(data.TodayVsBudgets_Budget));
            allWidgets.Add(todayVsBudgets);

            var todayVsLastweek = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
              GrindToday.WidgetEndpoints.TodayVsSameDayLastWeek,
              "Today", (int)Decimal.Round(data.TodayVsSameDayLastWeek_Today), "Budget",
          (int)Decimal.Round(data.TodayVsSameDayLastWeek_LastWeek));
            allWidgets.Add(todayVsLastweek);

            //shore
            var shore = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindToday.WidgetEndpoints.Shoreditch,
           "Today", (int)Decimal.Round(data.Shoreditch_Today), "Budget",
       (int)Decimal.Round(data.Shoreditch_Budget));
            allWidgets.Add(shore);

            var soho = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindToday.WidgetEndpoints.Soho,
           "Today", (int)Decimal.Round(data.Soho_Today), "Budget",
       (int)Decimal.Round(data.Soho_Budget));
            allWidgets.Add(soho);

            var london = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindToday.WidgetEndpoints.London,
           "Today", (int)Decimal.Round(data.London_Today), "Budget",
       (int)Decimal.Round(data.London_Budget));
            allWidgets.Add(london);

            var hatton = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindToday.WidgetEndpoints.Hatton_Garden,
           "Today", (int)Decimal.Round(data.Hatton_Garden_Today), "Budget",
       (int)Decimal.Round(data.Hatton_Garden_Budget));
            allWidgets.Add(hatton);

            var royal = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindToday.WidgetEndpoints.Royal_Exchange,
           "Today", (int)Decimal.Round(data.Royal_Exchange_Today), "Budget",
       (int)Decimal.Round(data.Royal_Exchange_Budget));
            allWidgets.Add(royal);

            var covent = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindToday.WidgetEndpoints.Covent_Garden,
            "Today", (int)Decimal.Round(data.Covent_Garden_Today), "Budget",
            (int)Decimal.Round(data.Covent_Garden_Budget));
            allWidgets.Add(covent);

            var clerk = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindToday.WidgetEndpoints.Clerkenwell,
            "Today", (int)Decimal.Round(data.Clerkenwell_Today), "Budget",
            (int)Decimal.Round(data.Clerkenwell_Budget));
            allWidgets.Add(clerk);


            var white = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                GrindToday.WidgetEndpoints.Whitechapel,
                "Today", (int)Decimal.Round(data.Whitechapel_Today), "Budget",
                (int)Decimal.Round(data.Whitechapel_Budget));
            allWidgets.Add(white);

            var exmouth = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindToday.WidgetEndpoints.Exmouth_Market,
            "Today", (int)Decimal.Round(data.Exmouth_Market_Today), "Budget",
            (int)Decimal.Round(data.Exmouth_Market_Budget));
            allWidgets.Add(exmouth);


            var face = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindToday.WidgetEndpoints.Facebook,
            "Today", (int)Decimal.Round(data.Facebook_Today), "Budget",
            (int)Decimal.Round(data.Facebook_Budget));
            allWidgets.Add(face);


            var greenwich = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
            GrindToday.WidgetEndpoints.Greenwich,
            "Today", (int)Decimal.Round(data.Greenwich_Today), "Budget",
            (int)Decimal.Round(data.Greenwich_Budget));
            allWidgets.Add(greenwich);

            var liverpool = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
           GrindToday.WidgetEndpoints.Liverpool_Street,
           "Today", (int)Decimal.Round(data.Liverpool_Street_Today), "Budget",
           (int)Decimal.Round(data.Liverpool_Street_Budget));
            allWidgets.Add(liverpool);

            return allWidgets;
        }


    }
}

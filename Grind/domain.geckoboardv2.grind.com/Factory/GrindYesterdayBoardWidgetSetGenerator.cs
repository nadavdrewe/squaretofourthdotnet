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
    public class GrindYesterdayBoardWidgetSetGenerator
    {
        IGeckoboardObjectCreatorFactory geckoboardObjectCreatorFactory;

        public GrindYesterdayBoardWidgetSetGenerator(string geckoBoardAPIKEY)
        {
            var geckoORg = new GeckoboardOrganisation(geckoBoardAPIKEY, "Railgunit");
            geckoboardObjectCreatorFactory = new GeckoboardObjectCreatorFactory(geckoORg);
        }

        //take in data and just maps to widgets
        public List<GeckoboardObject> GenerateWidgets(GrindYesterdayBoardDataset data)
        {
            List<GeckoboardObject> allWidgets = new List<GeckoboardObject>();

            //first widget - SalesTodayVsLastWeek
            var yestVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                  GrindYesterday.WidgetEndpoints.YesterdayVsBudget,
                  "Today", (int)Decimal.Round(data.YesterdayVsBudget_Yesterday), "Last Week",
              (int)Decimal.Round(data.YesterdayVsBudget_Budget));
            allWidgets.Add(yestVsBudget);

            var shoreVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Shoreditch,
                 "Today", (int)Decimal.Round(data.Shoreditch_Yesterday), "Last Week",
             (int)Decimal.Round(data.Shoreditch_Budget));
            allWidgets.Add(shoreVsBudget);

            var sohoVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Soho,
                 "Today", (int)Decimal.Round(data.Soho_Yesterday), "Last Week",
             (int)Decimal.Round(data.Soho_Budget));
            allWidgets.Add(sohoVsBudget);

            //london
            var londonVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.London,
                 "Today", (int)Decimal.Round(data.London_Yesterday), "Last Week",
             (int)Decimal.Round(data.London_Budget));
            allWidgets.Add(londonVsBudget);

            //hatton
            var hattonVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Hatton_Garden,
                 "Today", (int)Decimal.Round(data.Hatton_Garden_Yesterday), "Last Week",
             (int)Decimal.Round(data.Hatton_Garden_Budget));
            allWidgets.Add(hattonVsBudget);

            //coventVsBudget
            var coventVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Covent_Garden,
                 "Today", (int)Decimal.Round(data.Covent_Garden_Yesterday), "Last Week",
             (int)Decimal.Round(data.Covent_Garden_Budget));
            allWidgets.Add(coventVsBudget);

            //coventVsBudget
            var royal = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Royal_Exchange,
                 "Today", (int)Decimal.Round(data.Royal_Exchange_Yesterday), "Last Week",
             (int)Decimal.Round(data.Royal_Exchange_Budget));
            allWidgets.Add(royal);

            //clerkenwellVsBudget
            var clerkenwellVsBudget = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Clerkenwell,
                 "Today", (int)Decimal.Round(data.Clerkenwell_Yesterday), "Last Week",
             (int)Decimal.Round(data.Clerkenwell_Budget));
            allWidgets.Add(clerkenwellVsBudget);

            //whitechamp
            var whitechap = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Whitechapel,
                 "Today", (int)Decimal.Round(data.Whitechapel_Yesterday), "Last Week",
             (int)Decimal.Round(data.Whitechapel_Budget));
            allWidgets.Add(whitechap);

            //exmouth
            var exmouth = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                 GrindYesterday.WidgetEndpoints.Exmouth_Market,
                 "Today", (int)Decimal.Round(data.Exmouth_Market_Yesterday), "Last Week",
             (int)Decimal.Round(data.Exmouth_Market_Budget));
            allWidgets.Add(exmouth);

            //facebook
            var facebook = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                GrindYesterday.WidgetEndpoints.Facebook,
                "Today", (int)Decimal.Round(data.Facebook_Yesterday), "Last Week",
            (int)Decimal.Round(data.Facebook_Budget));
            allWidgets.Add(facebook);

            //greenwich
            var greenwich = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                GrindYesterday.WidgetEndpoints.Greenwich,
                "Today", (int)Decimal.Round(data.Greenwich_Yesterday), "Last Week",
            (int)Decimal.Round(data.Greenwich_Budget));
            allWidgets.Add(greenwich);

            //liverpool st
            var liverpoolSt = geckoboardObjectCreatorFactory.CreateNumberSecondaryStat(1, "",
                GrindYesterday.WidgetEndpoints.Liverpool_Street,
                "Today", (int)Decimal.Round(data.Liverpool_Street_Yesterday), "Last Week",
            (int)Decimal.Round(data.Liverpool_Street_Budget));
            allWidgets.Add(liverpoolSt);

            return allWidgets;
        }
    }
}

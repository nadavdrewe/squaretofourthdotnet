//using System;
//using System.Collections.Generic;
//using System.Linq;
//using GeckoboardLibrary.Classes.Widgets;

//namespace Revel._808nd.com.Classes
//{
//    public class LineGraphOrderItemBreakdown
//    {
//        public LineGraphOrderItemBreakdown()
//        {
//            OrderItems = new List<OrderItem>();
//        }


//        public decimal MoneyInPeriod { get; set; }
//        public DatePeriod Period { get; set; }
//        public ItemType ItemType { get; set; }
//        public List<OrderItem> OrderItems { get; set; }


//    //    public static LineV2Series ReturnLineV2SeriesPureSales(List<LineGraphOrderItemBreakdown> itemBreakdowns)
//    //    {
//    //        var coffeeSeries = new LineV2Series
//    //        {
//    //            name = itemBreakdowns.First().ItemType.ToString(),
//    //            data = new List<decimal>()
//    //        };

//    //        foreach (var coffeeWeek in itemBreakdowns)
//    //        {
//    //            coffeeSeries.data.Add(coffeeWeek.OrderItems.Sum(x => x.pure_sales));
//    //        }
//    //        return coffeeSeries;
//    //    }

//    //    /*  public static LineV2Series ReturnLineV2SeriesAvgSalesSpeed(DateTime Date, List<OrderItem> itemBreakdowns)
//    //      {
//    //          var series = new LineV2Series
//    //          {
//    //              name = Date.ToString(),
//    //              data = new List<decimal>()
//    //          };

//    //          foreach (var coffeeWeek in itemBreakdowns)
//    //          {
//    //              coffeeSeries.data.Add(coffeeWeek.OrderItems.Sum(x => x.pure_sales));
//    //          }
//    //          return coffeeSeries;
//    //      }*/

//    //}





//    public class DatePeriod
//    {
//        public DateTime PeriodStart { get; set; }
//        public DateTime PeriodEnd { get; set; }

//    }
//}

//namespace Revel._808nd.com.Classes
//{
//    public enum ItemType
//    {
//        Food, Beverage, Coffee
//    }
//}
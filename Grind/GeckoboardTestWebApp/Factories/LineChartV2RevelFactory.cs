using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes.Widgets;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Extensions;


namespace GeckoboardTestWebApp.Models
{
    public static class LineChartV2RevelFactory
    {
        public static LineV2Widget Initialise24WeekLineV2WidgetData(LineV2Widget widget,
            RevelProductAndCategoryWrapper identificationsService, IEnumerable<Establishment> establishments,
            IEnumerable<OrderItem> last24weeksOrderItems, IEnumerable<ProductClass> productClasses
            )
        {
            var mostRecentMonday = DateTimeExtensions.StartOfWeek(DateTime.Now, DayOfWeek.Monday);

            var _24weeksAgo = mostRecentMonday.AddDays(-168);


            var startOfEachWeekForPast24 = new List<DatePeriod>();
            var currentLoopDate = _24weeksAgo;
            var coffee = new List<LineGraphOrderItemBreakdown>();
            var food = new List<LineGraphOrderItemBreakdown>();
            var beverage = new List<LineGraphOrderItemBreakdown>();

            do
            {
                var endLoopDate = currentLoopDate.AddDays(7);
                startOfEachWeekForPast24.Add(new DatePeriod
                {
                    PeriodStart = new DateTime(currentLoopDate.Year, currentLoopDate.Month, currentLoopDate.Day, 02, 00, 00),
                    PeriodEnd = new DateTime(endLoopDate.Year, endLoopDate.Month, endLoopDate.Day, 23, 59, 59)

                });

                currentLoopDate = currentLoopDate.AddDays(7);
            } while (currentLoopDate < mostRecentMonday);

            //create one breakdown for each cat, for all 24 weeeks
            foreach (ItemType itemEnum in Enum.GetValues(typeof(ItemType)))
            {
                foreach (var week in startOfEachWeekForPast24)
                {
                    if (itemEnum.Equals(ItemType.Beverage))
                    {
                        beverage.Add(new LineGraphOrderItemBreakdown
                        {
                            Period = week,
                            ItemType = ItemType.Beverage


                        });
                    }
                    else if (itemEnum.Equals(ItemType.Coffee))
                    {
                        coffee.Add(new LineGraphOrderItemBreakdown
                        {
                            Period = week,
                            ItemType = ItemType.Coffee,
                        });

                    }
                    else if (itemEnum.Equals(ItemType.Food))
                    {
                        food.Add(new LineGraphOrderItemBreakdown
                        {
                            Period = week,
                            ItemType = ItemType.Food,
                        });
                    }
                }
            }


            //coffee

            var itemsWithoutACategory = new List<OrderItem>();
            foreach (var orderItem in last24weeksOrderItems)
            {


                var foodbucket = food.Where(
                               x =>
                                   x.Period.PeriodStart <= orderItem.created_date &&
                                   x.Period.PeriodEnd >= orderItem.created_date).FirstOrDefault();

                var beverageBucket = beverage.Where(
                            x =>
                                x.Period.PeriodStart <= orderItem.created_date &&
                                x.Period.PeriodEnd >= orderItem.created_date).FirstOrDefault();
                var coffeeBucket = coffee.Where(
                          x =>
                              x.Period.PeriodStart <= orderItem.created_date &&
                              x.Period.PeriodEnd >= orderItem.created_date).FirstOrDefault();

                try
                {
                    RevelProductAndCategoryWrapper pcWrapper = new RevelProductAndCategoryWrapper();
                    IList<Product> errors = new List<Product>();
                    List<Product> foodProducts = pcWrapper.GetProductsThatAreFoodByClass(productClasses, out errors);
                    List<Product> hotDrinksProducts = pcWrapper.GetProductsThatAreHotDrinksByClass(productClasses, out errors);
                    List<Product> alcoholProducts = pcWrapper.GetProductsThatAreAlcoholByClass(productClasses, out errors);
                    List<Product> softDrinks = pcWrapper.GetProductsThatAreSoftDrinksByClass(productClasses, out errors);



                    foreach (var est in establishments)
                    {

                        if (identificationsService.isItemFood(orderItem, foodProducts, out errors))
                        {
                            //get the date range of the food item and add it to that week range

                            foodbucket.OrderItems.Add(orderItem);
                        }
                        else if (identificationsService.isItemAlcohol(orderItem, alcoholProducts, out errors))
                        {
                            //get the date range of the food item and add it to that week range
                            beverageBucket.OrderItems.Add(orderItem);

                        }
                        //soft drinks
                        else if (identificationsService.isItemSoftDrink(orderItem, softDrinks, out errors))
                        {
                            //get the date range of the food item and add it to that week range
                            beverageBucket.OrderItems.Add(orderItem);
                        }
                        else if (identificationsService.isItemHotDrink(orderItem, hotDrinksProducts, out errors))
                        {
                            //get the date range of the food item and add it to that week range                      
                            coffeeBucket.OrderItems.Add(orderItem);

                        }
                        else
                        {
                            itemsWithoutACategory.Add(orderItem);
                        }
                    }

                }
                catch (Exception ex)
                {

                    /* throw new Exception("There's been an error in the Factory when sorting the line chart items", ex);*/
                }
            }

            //setup
            var xAxis = new LineV2XAsis
            {
                type = "",
                labels = coffee.Select(x => x.Period.PeriodStart.Month.ToString()).ToList()
            };


            widget.data.x_axis = xAxis;
            widget.data.y_axis = new LineV2YAxis
            {
                format = "currency",
                unit = "GBP"
            };

            var series = new List<LineV2Series>();
            var allGroupedItems = new List<LineGraphOrderItemBreakdown>();


            var coffeeSeries = LineGraphOrderItemBreakdown.ReturnLineV2SeriesPureSales(coffee);
            var foodSeries = LineGraphOrderItemBreakdown.ReturnLineV2SeriesPureSales(food);
            var beverageSeries = LineGraphOrderItemBreakdown.ReturnLineV2SeriesPureSales(beverage);
            //end coffee



            widget.data.series = new List<LineV2Series>
            {
                coffeeSeries,
                foodSeries,
                beverageSeries

            };

            return widget;
        }
    }
}

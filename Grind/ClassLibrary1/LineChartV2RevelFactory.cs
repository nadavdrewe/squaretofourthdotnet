using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes.Widgets;
using Revel._808nd.com.Classes;


namespace domain.geckoboard.grind
{
    public static class LineChartV2RevelFactory
    {
        public static LineV2Widget Initialise24WeekLineV2WidgetData(LineV2Widget widget,
            RevelProductAndCategoryWrapper identificationsService, Establishment establishment,
            IEnumerable<OrderItem> last24weeksOrderItems)
        {
            var yesterday = DateTime.Now.AddDays(-1);
            var _24weeksAgo = yesterday.AddDays(-168);
            

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
                    PeriodStart = currentLoopDate,
                    PeriodEnd = endLoopDate

                });

                currentLoopDate = currentLoopDate.AddDays(7);
            } while (currentLoopDate < yesterday);

            //create one breakdown for each cat, for all 24 weeeks
            foreach (ItemType itemEnum in Enum.GetValues(typeof (ItemType)))
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
            foreach (var orderItem in last24weeksOrderItems)
            {
                try
                {
                    if (identificationsService.isItemFood(orderItem, establishment.establishment_id))
                    {
                        //get the date range of the food item and add it to that week range
                        var graphBucket = food.Where(
                            x =>
                                x.Period.PeriodStart <= orderItem.created_date &&
                                x.Period.PeriodEnd >= orderItem.created_date).FirstOrDefault();

                        graphBucket.OrderItems.Add(orderItem);
                    }
                    else if (identificationsService.isItemAlcohol(orderItem, establishment.establishment_id) ||
                             identificationsService.isItemSoftDrink(orderItem, establishment.establishment_id))
                    {
                        //get the date range of the food item and add it to that week range
                        var graphBucket = beverage.Where(
                            x =>
                                x.Period.PeriodStart <= orderItem.created_date &&
                                x.Period.PeriodEnd >= orderItem.created_date).FirstOrDefault();

                        if (graphBucket == null)
                        {

                        }

                        graphBucket.OrderItems.Add(orderItem);

                    }
                    else if (identificationsService.isItemHotDrink(orderItem, establishment.establishment_id))
                    {
                        //get the date range of the food item and add it to that week range
                        var graphBucket = coffee.Where(
                            x =>
                                x.Period.PeriodStart <= orderItem.created_date &&
                                x.Period.PeriodEnd >= orderItem.created_date).FirstOrDefault();

                        if (graphBucket == null)
                        {

                        }

                        graphBucket.OrderItems.Add(orderItem);

                    }
                }
                catch (Exception ex)
                {


                    /* throw new Exception("", ex);*/
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


            var coffeeSeries = LineGraphOrderItemBreakdown.ReturnLineV2Series(coffee);
            var foodSeries = LineGraphOrderItemBreakdown.ReturnLineV2Series(food);
            var beverageSeries = LineGraphOrderItemBreakdown.ReturnLineV2Series(beverage);
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

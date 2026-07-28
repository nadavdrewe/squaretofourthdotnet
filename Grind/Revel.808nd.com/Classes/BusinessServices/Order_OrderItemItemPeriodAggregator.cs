using System;
using System.Collections.Generic;
using System.Linq;

namespace Revel._808nd.com.Classes.BusinessServices
{
    public  class Order_OrderItemItemPeriodAggregator
    {

        public  void GetBreakfastLunchDinnerTotals(List<OrderItem> items, List<Order> parentItems, DateTime startDate,
            out decimal breakfastTotal,
            out decimal lunchTotal, out decimal dinnerTotal)
          
        {

            breakfastTotal = 0.00M;
            lunchTotal = 0.00M;
            dinnerTotal = 0.00M;

            GrindItemSalesPeriod identifierService = new GrindItemSalesPeriod();
            GrindItemSalesPeriod.ItemSalesPeriodTime grindItemSalesPeriod;

            foreach (var item in items)
            {
                var order =
                    parentItems.FirstOrDefault(x => x.order_id == item.parent_order_id);


                if (order != null)
                {
                    grindItemSalesPeriod = identifierService.GetItemSalesPeriod(order, order.created_date);
                }
                else
                {
                    grindItemSalesPeriod = identifierService.GetItemSalesPeriod(item, (DateTime)item.created_date);
                }



                switch (grindItemSalesPeriod)
                {
                    case GrindItemSalesPeriod.ItemSalesPeriodTime.Breakfast:
                        breakfastTotal += item.Price;
                        break;
                    case GrindItemSalesPeriod.ItemSalesPeriodTime.Lunch:
                        lunchTotal += item.Price;
                        break;
                    case GrindItemSalesPeriod.ItemSalesPeriodTime.Dinner:
                        dinnerTotal += item.Price;
                        break;
                    default:
                        break;

                }
            }
        }

    }
}

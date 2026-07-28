using System;
using System.Collections.Generic;

namespace Revel._808nd.com.Classes.BusinessServices
{
    public class GrindItemSalesPeriod /* : IItemSalesPeriod*/
    {

        public List<IItemSalesPeriod> Items { get; set; }
        public ItemSalesPeriodTime SalesPeriod;



        public ItemSalesPeriodTime GetItemSalesPeriod(ICreationDated item, DateTime startDate)
        {
            var now = startDate;
            var tomorrow = now.AddDays(1);

            if (item.CreationDate > new DateTime(now.Year, now.Month, now.Day, 05, 00, 00) &&
                item.CreationDate <= new DateTime(now.Year, now.Month, now.Day, 11, 59, 59))
            {
                return ItemSalesPeriodTime.Breakfast;
            }
            else if
                (item.CreationDate > new DateTime(now.Year, now.Month, now.Day, 12, 00, 00) &&
                 item.CreationDate <= new DateTime(now.Year, now.Month, now.Day, 16, 59, 59))
            {
                return ItemSalesPeriodTime.Lunch;
            }
            else if
                (item.CreationDate > new DateTime(now.Year, now.Month, now.Day, 17, 00, 00) &&
                 item.CreationDate <= new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 04, 59, 59))
            {
                return ItemSalesPeriodTime.Dinner;

            }
            return ItemSalesPeriodTime.None;
        }


        public void GetBreakfastLunchDinnerTotals<T>(List<T> items, out decimal breakfastTotal, DateTime startDate, 
            out decimal lunchTotal, out decimal dinnerTotal) where T : IPricable, ICreationDated
        {
            breakfastTotal = 0.00M;
            lunchTotal = 0.00M;
            dinnerTotal = 0.00M;

            foreach (var item in items)
            {
                var grindItemSalesPeriod = GetItemSalesPeriod(item, item.CreationDate);

                switch (grindItemSalesPeriod)
                {
                    case ItemSalesPeriodTime.Breakfast:
                        breakfastTotal += item.Price;
                        break;
                    case ItemSalesPeriodTime.Lunch:
                        lunchTotal += item.Price;
                        break;
                    case ItemSalesPeriodTime.Dinner:
                        dinnerTotal += item.Price;
                        break;
                    default:
                        break;
                }



            }
        }

        public enum ItemSalesPeriodTime
        {
            Breakfast,
            Lunch,
            Dinner,
            None
        }

        public interface IItemSalesPeriod
        {
            ItemSalesPeriodTime GetItemSalesPeriod(ICreationDated item);
        }

        public interface ICreationDated
        {
            DateTime CreationDate { get; }
        }

        public interface IPricable
        {
            decimal Price { get; }
        }

        public interface IParentable
        {
            int LinkingIdToParent { get; }
        }
    }
}



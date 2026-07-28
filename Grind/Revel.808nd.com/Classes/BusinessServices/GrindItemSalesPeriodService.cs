using System;
using System.Collections.Generic;
using System.Linq;

namespace Revel._808nd.com.Classes.BusinessServices
{
    public class GrindItemSalesPeriodService
    {

        private GrindItemSalesPeriod.IItemSalesPeriod ItemSalesPeriod { get; }
        public GrindItemSalesPeriodService()
        {
            
        }
       

        public List<T> GetListOfTimePeriodItems<T>(GrindItemSalesPeriod.ItemSalesPeriodTime timePeriod, List<T> items) where T : GrindItemSalesPeriod.ICreationDated
        {
            if (timePeriod.Equals(GrindItemSalesPeriod.ItemSalesPeriodTime.Breakfast))
            {

                return
                    items.Where(
                        item =>
                            item.CreationDate >
                            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 05, 00, 00) &&
                            item.CreationDate <=
                            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 59, 59)).ToList();
            }
            else if (timePeriod.Equals(GrindItemSalesPeriod.ItemSalesPeriodTime.Lunch))
            {
                return
                   items.Where(
                       item =>
                           item.CreationDate >
                           new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 00, 00) &&
                           item.CreationDate <=
                           new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 59, 59)).ToList();

            }
            else if (timePeriod.Equals(GrindItemSalesPeriod.ItemSalesPeriodTime.Dinner))
            {
                return
                   items.Where(
                       item =>
                           item.CreationDate >
                           new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 00, 00) &&
                           item.CreationDate <=
                           new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 02, 59, 59)).ToList();

            }
            else return new List<T>();
        }


    }
}

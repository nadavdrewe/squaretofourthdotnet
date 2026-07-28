using System;
using System.Collections.Generic;
using System.Linq;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class GenericRevelMethods
    {


     


        public static double GetAverageTimeOfServiceInSeconds<T>(List<T> listofType) where T : IRevelTimeable
        {
            List<T> prodsIncluded = new List<T>();
            try
            {
                Nullable<TimeSpan> avgTime = new TimeSpan();

                foreach (var item in listofType)
                {
                    if (item.start_time != null && item.kitchen_completed != null)
                    {
                        var remainder = item.kitchen_completed - item.start_time;
                        var castRemainder = (TimeSpan)remainder;

                        /*if (castRemainder.TotalSeconds < 60)
                        {*/
                        avgTime += remainder;
                        /*}*/

                        prodsIncluded.Add(item);

                    }

                }

                var finalTimespan = (TimeSpan)avgTime;

                var howManyItemsWereKDS = listofType.Count(x => x.start_time != null);
                var secondsToReturn = finalTimespan.TotalSeconds / (int)howManyItemsWereKDS;

                return secondsToReturn;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

    }
}

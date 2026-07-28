using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.geckoboardv2.grind.com.Models.BoardTypeWidgetUrls
{
    //this holds the endpoints
    public class IndividualStoreBase
    {
        public int EstablishmentId { get; set; }

        //Sales
        public string SalesTodayVsLastWeek { get; set; }
        public string SalesTodayVsBudget { get; set; }
        public string DiscountTodayVsBudget { get; set; }
        public string WTDVsLastWeek { get; set; }
        public string WTDVsBudget { get; set; }
        public string WTDDiscountVsBudget { get; set; }
        public string CumulativeHourlySales { get; set; }

        public string CoffeeWTDSalesVsLastWeek { get; set; }
        public string FoodWTDSalesVsLastWeek { get; set; }
        public string BarWTDSalesVsLastWeek { get; set; }
        public string RetailTodayVsLastWeek { get; set; }
        public string CoffeeVolumeTodayVsLastWeek { get; set; }

        public string CoversWTDVsLastWeek { get; set; }
        public string AverageCoverValue { get; set; }

    }
}

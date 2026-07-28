using Revel._808nd.com.ReportingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.geckoboardv2.grind.com.Models.BoardData
{
    public class IndividualBoardDataset
    {
        public int EstablishmentId { get; set; }

        //Sales
        public decimal SalesTodayVsLastWeek_Today { get; set; }
        public decimal SalesTodayVsLastWeek_LastWeek { get; set; }

        public decimal SalesTodayVsBudget_Today { get; set; }
        public decimal SalesTodayVsBudget_Budget { get; set; }

        public decimal DiscountTodayVsBudget_Today { get; set; }
        public decimal DiscountTodayVsBudget_Budget { get; set; }


        public decimal WTDVsLastWeek_Today { get; set; }
        public decimal WTDVsLastWeek_LastWeek { get; set; }

        public decimal WTDVsBudget_Today { get; set; }
        public decimal WTDVsBudget_Budget { get; set; }

        public decimal WTDDiscountVsBudget_Today { get; set; }
        public decimal WTDDiscountVsBudget_Budget { get; set; }
                
        public decimal CoffeeWTDSalesVsLastWeek_Today { get; set; }
        public decimal CoffeeWTDSalesVsLastWeek_LastWeek { get; set; }

        public decimal FoodWTDSalesVsLastWeek_Today { get; set; }
        public decimal FoodWTDSalesVsLastWeek_LastWeek { get; set; }

        public decimal BarWTDSalesVsLastWeek_Today { get; set; }
        public decimal BarWTDSalesVsLastWeek_LastWeek { get; set; }

        public decimal RetailTodayVsLastWeek_Today { get; set; }
        public decimal RetailTodayVsLastWeek_LastWeek { get; set; }

        public decimal CoffeeVolumeTodayVsLastWeek_Today { get; set; }
        public decimal CoffeeVolumeTodayVsLastWeek_LastWeek { get; set; }

        public decimal CoversWTDVsLastWeek_Today { get; set; }
        public decimal CoversWTDVsLastWeek_LastWeek { get; set; }

        public decimal AverageCoverValueWTD_Today { get; set; }
        public decimal AverageCoverValueWTD_LastWeek { get; set; }

        public List<HourAndSpend> CumulativeHourAndSpendsToday { get; set; }
        public List<HourAndSpend> CumulativeHourAndSpendsBudget { get; set; }
        public List<HourAndSpend> CumulativeHourAndSpendsSameDayLastWeeek { get; set; }
    }
}

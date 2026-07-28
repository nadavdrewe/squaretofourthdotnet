using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.Reporting.ReportingFactory
{
    public class RewardCardLogsReportFactory
    {

        public IEnumerable<RewardCardLogPointsTotal> GetLogPointsVsOrdersForDateRange(ReportContext context)
        {
            List<RewardCardLogPointsTotal> pointSummaries = new List<RewardCardLogPointsTotal>();
            List<DateTime> thePeriodForReporting = new List<DateTime>();
            GrindContext db = new GrindContext();


            var startDate = context.StartDate;
            var endDate = context.EndDate;


            /*Get date range*/
            var currentDate = startDate;
            while (currentDate < endDate)
            {
                thePeriodForReporting.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }


            foreach (var date in thePeriodForReporting)
            {
                var nextDate = date.AddDays(1);


                /*Exclude split bills and comps*/
                var ordersIdsForThatDay = db.Orders
              .Where(x => x.created_date >= date)
              .Where(x => x.created_date <= nextDate)
              .Where(x => x.bill_parent == null)
              .Select(x => x.order_id).Distinct().
              ToList();


                //todo: filter using Contains on the orders
                var logsForThatDay = db.RewardCardLogs
                    .Where(x => x.created_date >= date)
                    .Where(x => x.created_date <= nextDate);


                var addedPoints = logsForThatDay
                     .Where(x => x.type_of_change == "Add Points")
                    .Where(x => x.point >= 0).ToList();

                var redeemedPoints = logsForThatDay
                    .Where(x => x.type_of_change == "Redeem Points").ToList();

               var otherRedeemedPoints =  logsForThatDay
                        .Where(x => x.type_of_change == "Add Points")
                     .Where(x => x.point < 0).ToList();

                redeemedPoints.AddRange(otherRedeemedPoints);

                var reportingItem = new RewardCardLogPointsTotal
                {
                    NumberOfOrders = ordersIdsForThatDay.Count(),
                    DateTime = date,
                    Grind = "All Grinds",
                    LogsAddingPointsAsPercentageOfOrders = ((decimal)addedPoints.Count() / (decimal)ordersIdsForThatDay.Count()) * 100.00M,
                    NumberOfLogsAddingPoints = addedPoints.Count(),
                    TotalNumberOfPointsAdded = addedPoints.Sum(x => x.point),
                    LogsRedeemingPointsAsPercentageOfOrders = ((decimal)redeemedPoints.Count() / (decimal)ordersIdsForThatDay.Count()) * 100.00M,
                    NumberOfLogsRedeemingPoints = redeemedPoints.Count,
                    TotalNumberOfPointsRedeemed = redeemedPoints.Sum(x=>Math.Abs(x.point))
                };

                pointSummaries.Add(reportingItem);


            }


            
            return pointSummaries.OrderBy(x=>x.DateTime).ToList();
        }




    }
}

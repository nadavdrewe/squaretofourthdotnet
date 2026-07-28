using System;
using System.Collections.Generic;
using System.Linq;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public class RewardCardPointsTransactionLogService
    {

        public List<RewardCardPointsTransactionLog> GetTransactionsForADate(DateTime date, GrindContext db)
        {
            var start = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            var end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
         
            return db.RewardCardPointsTransactionLogs.Where(x=>x.WhenCreated >= start && x.WhenCreated <= end).ToList();


        }

        public RewardCardPointsTransactionLog CreateRewardCardPointsTransactionLog(DateTime logDate, RewardsCardNew oldCard, RewardsCardNew newCard)
        {

            return new RewardCardPointsTransactionLog()
            {
                card_number = newCard.number,
                multiplier = 0,
                orginal_points_total =  oldCard.total_points,
                orginal_points_current = oldCard.current_points,
                new_points_total = newCard.total_points,
                new_points_current = newCard.current_points,
                pointsAdded = newCard.total_points - oldCard.total_points,
                WhenCreated = logDate.ToUniversalTime()
            };


        }
        
    }
}

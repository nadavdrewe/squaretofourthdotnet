using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public class RewardCardDailyPointsService
    {

        public async Task<int> CreateRewardCardDailyPointsToday(GrindContext db, DateTime todaysDate)
        {
            var today = todaysDate;
            return await CreateRewardCardDailyPoints(today, db.RewardsCardNew.ToList(), db);
        }

        public async Task<int> CreateRewardCardDailyPoints(DateTime syncDate, List<RewardsCardNew> cards, GrindContext db)
        {
            var syncedToday = await db.RewardsCardDailyPoints.Where(x => DbFunctions.TruncateTime(x.date) == syncDate.Date).ToListAsync();
            var stampsToadd = new List<RewardsCardDailyPoints>();

            foreach (var rewardsCardNew in cards)
            {
                var todaysSync =
                    syncedToday.Where(
                        x => x.RewardsCardNew.DBKEY_rewardscardnew_id == rewardsCardNew.DBKEY_rewardscardnew_id).FirstOrDefault();

                if (todaysSync == null)
                {
                    //save a stamp
                    stampsToadd.Add(new RewardsCardDailyPoints()
                    {
                        card_number = rewardsCardNew.number,
                        date = DateTime.Now.ToUniversalTime(),
                        RewardsCardNew = rewardsCardNew,
                        total_points_on_date = rewardsCardNew.total_points,
                        current_points_on_date = rewardsCardNew.current_points

                    });
                }

            }


            if (stampsToadd.Count > 0)
            {
                db.RewardsCardDailyPoints.AddRange(stampsToadd);
                var saved = db.SaveChanges();

                return saved;
            }

            return 0;
        }



        public async Task<List<RewardsCardDailyPoints>> GetRewardCardDailyPoints(DateTime DateToCheck, List<RewardsCardNew> cards,
            GrindContext db)
        {
            var dailyPointsForDateToCheck =
                db.RewardsCardDailyPoints.Where(x => DbFunctions.TruncateTime(x.date) == DateToCheck.Date).ToList();

            var dailyPointsToReturn = new List<RewardsCardDailyPoints>();

            foreach (var rewardsCardNew in cards)
            {
                var cardNumber = rewardsCardNew.number.ToLower().Trim();

                var test = dailyPointsForDateToCheck.FirstOrDefault(x => x.card_number.ToLower().Trim() == cardNumber);

                if (test != null)
                {
                    dailyPointsToReturn.Add(test);
                }
            }

            return dailyPointsToReturn;
        }





    }
}

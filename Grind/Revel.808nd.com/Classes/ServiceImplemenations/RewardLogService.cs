using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class RewardLogService
    {
        public RewardLogService()
        {

        }


        /// <summary>
        /// PUuls back all card log since a certain date and adds them to the db.
        /// </summary>
      





    




        public async Task<IEnumerable<RewardsCardNew>> GetCardsThatHaveUserTheAppInDateRange(DateTime start, DateTime end, IRevelDBContextable db)
        {

            /*var start = new DateTime(2016, 04, 01);
            var end = new DateTime(2016, 08, 01);*/

            var cardLogOrders = await db.RewardCardLogs
                   .Where(x => x.created_date >= start)
                   .Where(x => x.created_date <= end)
                .Distinct().ToListAsync();
            var carLogORderIds = new List<int>();

            var finalLogsWeWant = new List<RewardCardLog>();
            foreach (var log in cardLogOrders)
            {
                carLogORderIds.Add(RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(log.order));
            }

            var orderinLogs = await db.Orders.
            Where(x => x.created_date >= start)
                        .Where(x => x.created_date <= end)
                        .Where(x => x.created_by == "/enterprise/User/175/")
                        .ToListAsync();


            var matchingOrders = orderinLogs.FindAll(y => carLogORderIds.Any(anId => anId == y.order_id)).ToList();



            foreach (var log in cardLogOrders)
            {
                var orderId = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(log.order);

                matchingOrders.FirstOrDefault(x => x.order_id == orderId);

                if (matchingOrders.FirstOrDefault(x => x.order_id == orderId) != null)
                {

                    finalLogsWeWant.Add(log);
                }
            }

            var listOfRewardCards = finalLogsWeWant.Select(x => x.reward_card_id).ToList();


            var cards = new List<RewardsCardNew>();
            foreach (var cardlog in listOfRewardCards)
            {
                var card = db.RewardsCardNew.First(x => x.Revelid == cardlog);

                if (card != null)
                {
                    cards.Add(card);

                }
            }



            /*            var cardNumbers = cards.Select(x => x.Number).Distinct().ToList();
                        cardNumbers.Dump();*/


            return cards;
        }

        public async Task<List<RewardCardLog>> GetRewardLogPointsFromWebservice(Brand brand, DateTime startdate, DateTime endDate)
        {

            Establishment TopLevelOrg = new Establishment(1, "ARevelOrg",
                brand.key_secret,
                new Uri(brand.revel_base_url));
            var webReader = new RevelWebserviceDataReader(TopLevelOrg);

            var query = "/resources/RewardCardLog/?format=json&updated_date__gt={0}&updated_date__lte={1}&limit=0";
            var startdateString = startdate.ToString("yyyy-MM-ddTHH:mm:ss");
            var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");

            string webURL = String.Format(query,
                startdateString,
                endDateString);

            var rewardLogAstype = new RewardCardLog();
            var logs = await webReader.GetRevelWebserviceData<RewardCardLog>(rewardLogAstype, webURL);

            return logs;
        }

    }
}

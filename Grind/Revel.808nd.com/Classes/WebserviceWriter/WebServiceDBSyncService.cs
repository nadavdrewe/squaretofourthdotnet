using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.WebserviceWriter
{

    public class WebServiceDBSyncService
    {
        private RevelContextBase _db { get; set; }
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        public WebServiceDBSyncService(RevelContextBase db)
        {
            _db = db;
        }

        public async Task<int> ResetRedCards()
        {
            using (var db = new GrindContext())
            {
                var est = new Establishment(1, "Grind",
                      RevelAPIKEY,
                       new Uri(RevelBaseURL));

                var redCards =
                    db.RewardsCardNew.Where(x => x.is_vip_card == true).ToList();

                if (redCards.Any())
                {
                    var cardsToUpdate = new List<RewardsCardNew>(); //write to DB

                    foreach (var rewardsCardNew in redCards)
                    {
                        var service = new WebserviceDataWriter(est, db);
                        rewardsCardNew.current_points = rewardsCardNew.vip_points_refresh;
                        rewardsCardNew.total_points += rewardsCardNew.vip_points_refresh;

                        var ok = await service.UpdateRewardCard(rewardsCardNew);

                        if (ok.Equals(0))
                        {

                            //it worked, add to cards to be saved
                            cardsToUpdate.Add(rewardsCardNew);
                        }
                        else
                        {
                            return -1;
                        }

                    }

                    if (cardsToUpdate.Any())
                    {
                        var writer = new RevelDBWriter(_db);
                        var updated = await writer.UpdateRevelType(cardsToUpdate);
                        if (updated > 0)
                        {
                            return 0;
                        }

                        return -1;
                    }

                }

            }


            return 0;
        }


    }
}

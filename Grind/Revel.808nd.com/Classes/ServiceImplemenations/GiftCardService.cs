using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public class GiftCardService
    {
        private RevelContextBase _db { get; set; }
        private string RevelAPIKEY { get; } = ConfigurationManager.AppSettings["RevelAPIKEY"];
        private string RevelBaseURL { get; } = ConfigurationManager.AppSettings["RevelBaseURL"];

        public GiftCardService(RevelContextBase db)
        {
            _db = db;
        }

        public static IEnumerable<int> GetGiftCardNewIDs(List<GiftCard> existingGiftCard, List<GiftCard> webServiceexistingGiftCard,
     out IEnumerable<int> webServiceGiftCardNewIDs)
        {
            List<int> existingGiftCardIDs = (from card in existingGiftCard
                                             select (int)card.id).ToList();


            List<int> webGiftCardIDs = (from card in webServiceexistingGiftCard
                                        select (int)card.id).ToList();

            var test = webGiftCardIDs.Except(existingGiftCardIDs);

            webServiceGiftCardNewIDs = test;

            return webServiceGiftCardNewIDs;

        }


        public async Task<int> SyncAllGiftCardsAndInsertNew(DateTime start, DateTime end, int lastRewardsCard)
        {

            var revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            //////////////////
            //ADDNEW
            //////////////////

            List<GiftCard> existingGiftCard = await DBReader.GetRevelType<GiftCard>();

            var cardasType = new GiftCard("/resources/GiftCard?format=json&limit=0");
            List<GiftCard> webServiceexistingGiftCard = await webReader.GetRevelWebserviceData(cardasType,
              String.Format(cardasType.theAddress, start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end.ToString("yyyy-MM-ddTHH:mm:ss"))
                );


            IEnumerable<int> GiftCardNewIDsToInsert;

            GetGiftCardNewIDs(existingGiftCard, webServiceexistingGiftCard, out GiftCardNewIDsToInsert);

            //does this work????


            List<GiftCard> GiftCardsToInsert = new List<GiftCard>();

            foreach (var item in GiftCardNewIDsToInsert)
            {
                GiftCard GiftCardToInsert = webServiceexistingGiftCard.Where(c => c.id == item).FirstOrDefault();
                GiftCardsToInsert.Add(GiftCardToInsert);
            }

            var howMany = writer.SaveRevelType(GiftCardsToInsert);



            /////////////////
            //sync points
            /////////////////
            var cardToUpdate = new List<GiftCard>();

            //reget the cards with new ones included
            existingGiftCard = await DBReader.GetRevelType<GiftCard>();

            foreach (var card in webServiceexistingGiftCard)
            {
                //if (card.id == 4185)
                //{
                //    var stop = true;
                //}

                //get the same record from DB
                var dbCard = existingGiftCard.Find(x => x.id == card.id);
                //compare points, total visits, ermmm
                //if it's different update the points
                if (!dbCard.remaining_balance.Equals(card.remaining_balance))
                {
                 var id = dbCard.giftcard_id;

                    dbCard = card;
                    dbCard.remaining_balance = card.remaining_balance;
                    dbCard.giftcard_id = id;

                    cardToUpdate.Add(dbCard);
                }

            }

            //do the update
            try
            {
                if (cardToUpdate.Any())
                {
                    var updated = await writer.UpdateRevelType(cardToUpdate);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            //log these transactions


            return 0;
        }




    }
}

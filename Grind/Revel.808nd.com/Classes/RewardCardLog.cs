using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class RewardCardLog : IRevelAddressable, IRevelCreateable
    {
        [Key]
        public int DB_KEY_id { get; set; }
        public DateTime created_date { get; set; }
        public string establishment { get; set; }
        public int id { get; set; }
        public string order { get; set; }
        public decimal point { get; set; }
        public decimal points_by_purchases { get; set; }
        public decimal points_by_visits { get; set; }
        public decimal purchased { get; set; }
        public string resource_uri { get; set; }
        public string reward_card { get; set; }
        public string type_of_change { get; set; }
        public DateTime updated_date { get; set; }
        public string user { get; set; }
        public bool visit { get; set; }
        public string theAddress { get; set; }

        //added to make like easier

        public int reward_card_id { get; set; }
        public int order_id { get; set; }

        public RewardCardLog()
        {
            theAddress =
            "/resources/RewardCardLog?format=json&updated_date__gt={0}&updated_date__lte={1}&limit=0";
        }
        public int Create(dynamic jsonSingleRewardLogObject)
        {
            created_date = Convert.ToDateTime(jsonSingleRewardLogObject["created_date"]);
            establishment = jsonSingleRewardLogObject["establishment"];
            id = (int)jsonSingleRewardLogObject["id"];
            order = jsonSingleRewardLogObject["order"];
            point = (decimal)jsonSingleRewardLogObject["point"];
            points_by_purchases = (decimal)jsonSingleRewardLogObject["points_by_purchases"];
            points_by_visits = (decimal)jsonSingleRewardLogObject["points_by_visits"];
            purchased = (decimal)jsonSingleRewardLogObject["purchased"];
            resource_uri = jsonSingleRewardLogObject["resource_uri"];
            reward_card = jsonSingleRewardLogObject["reward_card"];
            type_of_change = jsonSingleRewardLogObject["type_of_change"];
            updated_date = Convert.ToDateTime(jsonSingleRewardLogObject["updated_date"]);

            try
            {
                order_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(order);

            }
            catch (Exception)
            {
               
            }


            try
            {
                reward_card_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(reward_card);
            }
            catch (Exception)
            {
                
                throw;
            }
            return 0;
        }
    }
}

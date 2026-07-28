using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class GiftCard : IRevelAddressable, IRevelCreateable,IPrimaryKeyable
    {
        public string address { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public string customer { get; set; }
        public string establishment { get; set; }       
        public int id { get; set; }
        public int initial_value { get; set; }
        public string number { get; set; }
        public int payment_type { get; set; }
        public decimal remaining_balance { get; set; }
        public string resource_uri { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }

        [JsonIgnore]
        [Key]
        public int giftcard_id { get; set; }

        [JsonIgnore]
        public string theAddress { get; set; }

        [JsonIgnore]
        public virtual Customer theCustomer { get; set; }

        [JsonIgnore]
        public virtual RewardsCardNew RewardsCardNew { get; set; }

        [JsonIgnore]
        public virtual int LinkingRevelCustomerID { get; set; }

        [JsonIgnore]
        public virtual int LinkingRevelRewardsCardNewID { get; set; }






        public GiftCard()
        {
            theAddress = @"/resources/GiftCard?format=json&id__gt={0}";
        }

        public GiftCard(string url)
        {
            theAddress = url;
        }

        public int Create(dynamic jsonSingleGiftCard)
        {
            try
            {
                //this.address = (string)jsonSingleGiftCard["address"];
                created_by = (string)jsonSingleGiftCard["created_by"];
                created_date = Convert.ToDateTime(jsonSingleGiftCard["created_date"].ToString("yyyy-MM-dd HH:mm:ss"));
                customer = (string)jsonSingleGiftCard["customer"];
                establishment = (string)jsonSingleGiftCard["establishment"];
                id = (int)jsonSingleGiftCard["id"];
                initial_value = (int)jsonSingleGiftCard["initial_value"];
                number = (string)jsonSingleGiftCard["number"];
                payment_type = (int)jsonSingleGiftCard["payment_type"];
                remaining_balance = (decimal)jsonSingleGiftCard["remaining_balance"];
                resource_uri = (string)jsonSingleGiftCard["resource_uri"];
                updated_by = (string)jsonSingleGiftCard["updated_by"];
                updated_date = Convert.ToDateTime(jsonSingleGiftCard["updated_date"].ToString("yyyy-MM-dd HH:mm:ss"));

            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't create Gift Card ctor", ex);

            }

            return 0;
        }


        public int PrimaryKey { get { return giftcard_id; } }
    }
}

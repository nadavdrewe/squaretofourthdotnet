using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Revel._808nd.com.Classes
{
    public class Address
    {
        [JsonProperty("active")]
        public bool active { get; set; }

        [JsonProperty("city")]
        public string city { get; set; }

        [JsonProperty("country")]
        public string country { get; set; }

        [JsonProperty("created_date")]
        public DateTime created_date { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("phone_number")]
        public string phone_number { get; set; }

        [JsonProperty("primary_billing")]
        public bool primary_billing { get; set; }

        [JsonProperty("primary_shipping")]
        public bool primary_shipping { get; set; }

        [JsonProperty("resource_uri")]
        public string resource_uri { get; set; }

        [JsonProperty("state")]
        public string state { get; set; }

        [JsonProperty("street_1")]
        public string street_1 { get; set; }

        [JsonProperty("street_2")]
        public string street_2 { get; set; }

        [JsonProperty("updated_date")]
        public DateTime updated_date { get; set; }

        [JsonProperty("uuid")]
        public string uuid { get; set; }

        [JsonProperty("zipcode")]
        public string zipcode { get; set; }


        //added by ND
        [Key]
        [JsonIgnore]
        public int DBKEY_address_id { get; set; }
        [JsonIgnore]
        public int customer_id { get; set; }
     
    }
}

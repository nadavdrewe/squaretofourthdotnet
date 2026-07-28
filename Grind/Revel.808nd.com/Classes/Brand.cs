using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class Brand : IRevelAddressable, IRevelCreateable, IRevelDeletable
    {
        [Column(TypeName = "datetime2")]
        public DateTime updated_date { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime created_date { get; set; }
        public int id { get; set; }


        [Key]
        [JsonIgnore]
        public int brand_id { get; set; }
        public string call_names { get; set; }
        public string resource_uri { get; set; }
        public List<string> establishments { get; set; }

        //added
        [JsonIgnore]
        public bool is_fourth_active { get; set; }

        [JsonIgnore]
        public string revel_base_url { get; set; }
        [JsonIgnore]
        public string theAddress { get; set; }
        [JsonIgnore]
        public string ResourceUri { get; set; }
        [JsonIgnore]
        public string key_secret { get; set; }
        [JsonIgnore]
        public string fourth_username { get; set; }
        [JsonIgnore]
        public string fourth_password { get; set; }
        [JsonIgnore]
        public Nullable<Guid> fourth_guid { get; set; }
        [JsonIgnore]
        public string fourth_locationID { get; set; }
        [JsonIgnore]
        public string fourth_RevenueCenter { get; set; }
        [JsonIgnore]
        public bool fourth_PushByEstablishment { get; set; }
        [JsonIgnore]
        public string emergency_contact { get; set; }

        public Brand()
        {
            is_fourth_active = true;
            theAddress = @"/enterprise/Brand/?format=json&limit=0";
        }

        public Brand(string baseURL)
        {            
            revel_base_url = baseURL;
            is_fourth_active = true;
            theAddress = @"/enterprise/Brand/?format=json&limit=0";
          
        }

        public int Create(dynamic brand)
        {

           
             id = brand["id"];
             call_names = brand["call_names"];
             resource_uri = brand["resource_uri"];
             name = brand["name"];
             company = brand["company"];
             establishments = new List<string>();
            
            JArray Jestablishments = JArray.Parse(brand["establishments"].ToString());


            foreach (var establishment in Jestablishments)
            {
             establishments.Add(establishment.ToObject<string>());       
            }
         

             try
             {
                 updated_date = Convert.ToDateTime(brand["updated_date"].ToString("yyyy-MM-dd hh:mm:ss"));
                 created_date = Convert.ToDateTime(brand["created_date"].ToString("yyyy-MM-dd hh:mm:ss"));
             }
             catch (Exception ex)
             {
                
                 //whatever
             }

             ResourceUri = resource_uri;

            return 0;
        }

    }




}

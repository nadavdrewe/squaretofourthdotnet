using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{

    public class User : IRevelAddressable, IRevelCreateable, IRevelDeletable
    {

        [Key]
        public int DBKEY_user_id { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool is_active { get; set; }
        public bool is_staff { get; set; }
        public DateTime last_login { get; set; }
        public DateTime date_joined { get; set; }
        public int id { get; set; }
        public string resource_uri { get; set; }

        public string theAddress { get; set; }


        public User()
        {
            theAddress = @"/enterprise/User/?format=json";
        }


        public User(string url)
        {
            theAddress = url;
        }

        public int Create(dynamic json)
        {
            username = json["username"];
            first_name = json["first_name"];
            last_name = json["last_name"];
            is_active = Convert.ToBoolean(json["is_active"]);
            is_staff = Convert.ToBoolean(json["is_staff"]);
            last_login = Convert.ToDateTime(json["last_login"]);
            date_joined = Convert.ToDateTime(json["date_joined"]);

            var test = Convert.ToInt32(json["id"]);
            id = test;
            resource_uri = json["resource_uri"];

            return 0;
        }


        [JsonIgnore]
        public string ResourceUri
        {
            get { return resource_uri; }

            set { resource_uri = value; }
        }
    }
}

using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class HouseAccount : IRevelAddressable, IRevelCreateable //v2 models
    {

        public decimal balance { get; set; }
        public DateTime created_date { get; set; }
        public string customer { get; set; }
        public bool enabled { get; set; }
        public string establishment { get; set; }
        public int id { get; set; }
        public decimal? max_limit { get; set; }
        public string resource_uri { get; set; }
        public DateTime updated_date { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string theAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Create(dynamic Type)
        {
            return -1;
        }

        [JsonIgnore]
        [NotMapped]
        public Customer Customer { get; set; }

        [JsonIgnore]
        [NotMapped]
        public List<HouseAccountPayment> HouseAccountPayments { get; set; }
    }
}

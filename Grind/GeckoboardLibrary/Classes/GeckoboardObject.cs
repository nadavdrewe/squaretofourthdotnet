using Geckoboard._808nd.com;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeckoboardLibrary.Classes
{
    public class GeckoboardObject
    {

        public GeckoboardObject()
        {                       
      
        }

        public GeckoboardObject(string api_key, string pushURL, string chartName, GeckoboardChartAndItemType type, int id=0)
        {
            this.chartName = chartName;
            this.api_key = api_key;
            this.pushURL = pushURL;            
            this.type = type;
            this.ID = id;
        }


        [JsonIgnore]
        public int ID { get; set; }
        [JsonIgnore]
        public string pushURL { get; set; }
        protected string chartName { get; set; }
        public string api_key { get; set; }     
        public GeckoboardChartAndItemType type { get; set; }

        public bool ShouldSerializetype()
        {
            // don't serialize the Manager property if an employee is their own manager
            return (false);
        }

        public string GetPushURL()
        {
            if (String.IsNullOrEmpty(this.pushURL))
                return "";

            return this.pushURL;
        }

    }
}

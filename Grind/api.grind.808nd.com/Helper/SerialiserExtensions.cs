using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace api.grind._808nd.com.Helper
{
    public static class SerialiserExtensions
    {
        public static string SerializeObject(object toSerialize)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented };

            return JsonConvert.SerializeObject(toSerialize, Formatting.None, settings);
        }
    }
}
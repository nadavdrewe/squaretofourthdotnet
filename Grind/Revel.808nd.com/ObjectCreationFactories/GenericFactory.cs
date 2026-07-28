using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ObjectCreationFactories
{
    public class GenericFactory : ICreate
    {
        string errorMessage = "Customer factory couldn't deserialse JSON to Type Provided";
        public T Create<T>(JObject JSONObject) where T :  new()
        {
            try
            {
                var obj = JSONObject.ToObject<T>();
                if (obj != null) return obj;
                throw new Exception("Couldn't create object in GenericFactory - type of" + typeof(T).Name);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't create object in GenericFactory", ex);
            }

        }
    }
}

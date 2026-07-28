using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ObjectCreationFactories
{
    public interface ICreate
    {
        T Create<T>(JObject JSONObject) where T :  new();
    }
}

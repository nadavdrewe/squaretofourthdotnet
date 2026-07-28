using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Revel._808nd.com.Interfaces
{
    public interface IPrimaryKeyable
    {
        [JsonIgnore]
        int PrimaryKey { get; }
    }
}

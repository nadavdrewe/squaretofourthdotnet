using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Utility
{
    public static class ExtensionMethods
    {
        public static string ToRevelDate(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-ddTHH:mm:ss");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.CaternetData
{
    public static class ExtensionMethods
    {
        public static string CoalesceDecimal(this string aString)
        {
            return String.IsNullOrWhiteSpace(aString) ? "0.00" : aString;
        }
    }
}

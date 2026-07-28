using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Extensions
{
    public static class FourthSalesExtensionMethods
    {
        public static string ToCodedTransactionId(this string theString, string recordActivityNumber)
        {
            return String.Format("{0}-{1}", theString, recordActivityNumber);
        }
    }
}

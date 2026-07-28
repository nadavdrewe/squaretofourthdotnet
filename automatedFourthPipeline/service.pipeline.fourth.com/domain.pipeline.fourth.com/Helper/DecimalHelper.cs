using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Helper
{
    public static class DecimalHelper
    {
        public static decimal DivideBy100(this string value)
        {

            var dec = Convert.ToDecimal(value);
            return dec / 100.00M;
        }

        public static decimal DivideBy100(this decimal value)
        {
            return value / 100.00M;
        }

        public static decimal DivideBy100(this int value)
        {
            var dec = Convert.ToDecimal(value);
            return value / 100.00M;
        }


        /// <summary>
        /// Gets paid price as % of base price 
        /// </summary>
        /// <param name="basePrice"></param>
        /// <param name="paidPrice"></param>
        public static decimal GetPercentageDiscountForEveryItem(decimal basePrice, decimal paidPrice)
        {
            return 100.00M - ((paidPrice / basePrice) * 100.00M);
        }
    }
}

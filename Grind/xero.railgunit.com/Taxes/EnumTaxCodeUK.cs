using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xero.railgunit.com.Taxes
{
    public class TaxCode : IHaveXeroMappingCode
    {
        public string Description { get; set; }
        public string XeroValue { get; set; }
        string IHaveXeroMappingCode.TaxCode { get => XeroValue; }
    }

    public static class XeroTaxCodeHelper
    {

        public static IEnumerable<TaxCode> GetTaxCodes()
        {
            return new List<TaxCode>
            {
                new TaxCode{ Description = "20% (VAT on Income)", XeroValue = "OUTPUT2" },
                new TaxCode{ Description = "20% (VAT on Expenses)", XeroValue = "INPUT2" },
                new TaxCode{ Description = "No VAT", XeroValue = "NONE" },
                new TaxCode{ Description = "Zero Rated Income", XeroValue = "ZERORATEDOUTPUT" }

            };
        }

    }
}

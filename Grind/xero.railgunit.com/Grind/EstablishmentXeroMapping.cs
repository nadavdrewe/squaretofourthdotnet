using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xero.railgunit.com.Grind
{
    public class EstablishmentXeroMapping
    {
        public string XeroContactName { get; set; }
        public string EstablishmentId { get; set; }
    }

    public class XeroCompanyContainer
    {
        public IList<EstablishmentXeroMapping> EstablishmentMappings { get; set; }
        public string RevelJSONCall { get; set; }
        public string BaseUrl { get; } = "https://api.xero.com/api.xro/2.0/";
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string PathToCert { get; set; }

        public XeroCompanyContainer()
        {
            EstablishmentMappings = new List<EstablishmentXeroMapping>();
            this.RevelJSONCall = "https://shoreditchgrind.revelup.com/brand/reports/product_mix/data/?sort_by=&sort_reverse=&combo_expand=&employee=&online_app=&online_app_type=&online_app_platform=&dining_option=&show_opened=1&show_unpaid=1&show_irregular=1&sort_view=0&show_product=1&show_sku=1&show_class=1&quantity_settings=3&taxable_not_taxable=1&item_discount=1&order_discount=1&tax_column=1&no-filter=0&range_from=16%2F03%2F2018+04%3A00&range_to=17%2F03%2F2018+04%3A00&format=json";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.Utility;

namespace Revel._808nd.com.ProductMix
{
    public static class ProductMixQueryFactory
    {
        public static string GenerateDateRangeQuery(DateTime start, DateTime end, int establishmentId)
        {
            var startProper = new DateTime(start.Year, start.Month, start.Day, 03, 00, 00).ToRevelDate();
            var endProper = new DateTime(end.Year, end.Month, end.Day, 03, 00, 00).ToRevelDate();

            var query = String.Format(@"reports/product_mix/data/?sort_by=&sort_reverse=&combo_expand=&employee=&online_app=&online_app_type=&online_app_platform=&dining_option=&show_opened=1&show_unpaid=1&show_irregular=1&sort_view=0&show_product=1&show_modifiers=1&show_sku=1&show_class=1&show_category=1&quantity_settings=1&show_price_percent=1&taxable_not_taxable=1&item_discount=1&order_discount=1&no-filter=0&range_from={0}&range_to={1}&establishment={2}&format=json", startProper, endProper, establishmentId.ToString());

            return query;
        }

        public static string GenerateDateRangeQueryWOwnTime(DateTime start, DateTime end, int establishmentId)
        {
            var startProper = start.ToRevelDate();
            var endProper = end.ToRevelDate();

            var query = String.Format(@"reports/product_mix/data/?sort_by=&sort_reverse=&combo_expand=&employee=&online_app=&online_app_type=&online_app_platform=&dining_option=&show_opened=1&show_unpaid=1&show_irregular=1&sort_view=0&show_product=1&show_modifiers=1&show_sku=1&show_class=1&show_category=1&quantity_settings=1&show_price_percent=1&taxable_not_taxable=1&item_discount=1&order_discount=1&no-filter=0&range_from={0}&range_to={1}&establishment={2}&format=json", startProper, endProper, establishmentId.ToString());

            return query;
        }

    }
}


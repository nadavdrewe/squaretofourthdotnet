using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ProductMix
{
    public class ProductMixRootObject
    {
        public List<object> revenue_centers { get; set; }
        public List<object> invalid { get; set; }
        public List<object> liabilities { get; set; }
        public List<object> top_by_profit { get; set; }
        public List<object> out_of_range { get; set; }
        public List<List<string>> donation_fields { get; set; }
        public List<object> productcombos { get; set; }
        public List<List<object>> employees { get; set; }
        public List<ProductClass> product_classes { get; set; }
        public List<object> error_payments { get; set; }
        public List<List<object>> top_by_quantaty { get; set; }
        public List<List<string>> product_fields { get; set; }
        public List<object> donations { get; set; }
        public List<List<object>> posstations { get; set; }
        public List<object> modifier_fields { get; set; }
        public List<Category> categories { get; set; }
        public List<Productmix> productmix { get; set; }
    }
}
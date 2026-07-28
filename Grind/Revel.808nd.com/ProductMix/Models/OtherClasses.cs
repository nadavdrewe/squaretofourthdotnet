using Revel._808nd.com.OperationsReport.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ProductMix
{


    public class XeroProductMixClassGroup
    {
        public string ParentCategoryName { get; set; }
        public IList<Productmix> ProductMixes { get; set; }
    }

    public class ProductClass
    {
        public int parent_class_id { get; set; }
        public string parent_class_name { get; set; }
        public int class_id { get; set; }
        public string class_name { get; set; }
    }

    public class Category
    {
        public string sub_category_name { get; set; }
        public int category_id { get; set; }
        public int sub_category_id { get; set; }
        public string category_name { get; set; }
    }
    
    public class Productmix
    {
        public string product_category { get; set; }
        public string tax { get; set; }
        public string parent_pclass { get; set; }
        public string product_sku { get; set; }
        public string cost { get; set; }
        public string untaxable_sales { get; set; }
        public string n_comps { get; set; }
        public string gm { get; set; }
        public string total { get; set; }
        public string n_items { get; set; }
        public string n_voids { get; set; }
        public string percent_price { get; set; }
        public string gm_percent { get; set; }
        public string crv_value_sales { get; set; }
        public string product_name { get; set; }
        public string row_type { get; set; }
        public string product_class { get; set; }
        public string price { get; set; }
        public string taxable_sales { get; set; }
        public string product_subcategory { get; set; }
        public string parent_product_name { get; set; }
        public string discount { get; set; }
        public string product_barcode { get; set; }
        public string order_discount { get; set; }
        public string food_cost { get; set; }
        public string avg_price { get; set; }
        public string product_weight { get; set; }
        public string crv_value_tax { get; set; }
        public string msrp { get; set; }
    }

    public class RootObject
    {
        public List<object> revenue_centers { get; set; }
        public List<int> invalid { get; set; }
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

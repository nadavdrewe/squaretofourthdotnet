using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.ReportingModel;

namespace Revel._808nd.com.Classes
{
    public partial class Product : IRevelAddressable//: :IIntegerFromPrimaryKey(int,Product)
    {

        [Key]
        public int DBKEY_product_id { get; set; }

        public string active { get; set; }
        public bool allow_price_override { get; set; }
        public object attribute_1 { get; set; }
        public object attribute_1_name { get; set; }
        public object attribute_2 { get; set; }
        public object attribute_2_name { get; set; }
        public object attribute_parent { get; set; }
        public int attribute_type { get; set; }
        public object attribute_value_1 { get; set; }
        public object attribute_value_2 { get; set; }
        public string barcode { get; set; }
        public string brand { get; set; }
        public string[] categories { get; set; }
        public string category { get; set; }
        public int color_code { get; set; }
        public List<object> combo_discount_products { get; set; }
        public object combo_product_1 { get; set; }
        public object combo_product_2 { get; set; }
        public object combo_product_3 { get; set; }
        public object combo_product_4 { get; set; }
        public object combo_product_5 { get; set; }
        public object combo_product_6 { get; set; }
        public object combo_product_7 { get; set; }
        public object combo_product_8 { get; set; }
        public string commission { get; set; }
        public decimal cost { get; set; }
        public object course_number { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public bool crv_enabled { get; set; }
        public object crv_label { get; set; }
        public object crv_value { get; set; }
        public bool deleted { get; set; }
        public object dept { get; set; }
        public string description { get; set; }
        public string dining_options { get; set; }
        public bool disable_modifier_popup { get; set; }
        public bool display_on_kiosk { get; set; }
        public bool display_online { get; set; }
        public bool ebt_no { get; set; }
        public string establishment { get; set; }
        public bool export { get; set; }
        public bool happy_hour { get; set; }
        public object happy_hour_end_time { get; set; }
        public object happy_hour_price { get; set; }
        public object happy_hour_start_time { get; set; }



        public int product_id { get; set; }
        public object image { get; set; }
        public bool is_cold { get; set; }
        public bool is_combo { get; set; }
        public bool is_drink { get; set; }
        public string kitchen_print_name { get; set; }
        public bool lock_enable { get; set; }
        public object lock_uuid { get; set; }
        public decimal max_price { get; set; }
        public string name { get; set; }
        public object point_redeem { get; set; }
        public object point_value { get; set; }
        public int preparation_time { get; set; }
        public decimal price { get; set; }
        public bool price_embedded { get; set; }
        public List<object> printers { get; set; }
        public object product_brand { get; set; }
        public List<object> product_group { get; set; }
        public object product_weight { get; set; }
        public int product_weight_unit { get; set; }
        public string productclass { get; set; }
        public string resource_uri { get; set; }
        public object retail { get; set; }
        public bool rti_combo { get; set; }
        public object shopify_sku { get; set; }
        public string sku { get; set; }
        public bool sold_by_weight { get; set; }
        public int sorting { get; set; }
        public string tare { get; set; }
        public System.Nullable<decimal> tax { get; set; } = 0.00M;
        public int tax_class { get; set; }
        public bool tax_included { get; set; }
        public object third_party_id { get; set; }
        public object unit_size { get; set; }
        public object uom { get; set; }
        public string updated_by { get; set; }
        public string updated_date { get; set; }
        public string uuid { get; set; }
        public bool variable_pricing { get; set; }
        public int variable_pricing_by { get; set; }

        //added by Nadav Drewe
        public List<int?> category_ids { get; set; }
        public int establishment_id { get; set; }
        public int? productclass_id { get; set; }
        public int? tax_id { get; set; }
        public int? brand_id { get; set; }
        public int categoryID { get; set; }

        public int db_brand_id { get; set; }
        public int db_establishment_id { get; set; }

        [JsonIgnore]
        public string theAddress { get; set; }

        public Product()
        {
            theAddress = "/resources/Product/?format=json&limit=0";
        }

        public Product(dynamic jsonSingleProductObject)
        {

            theAddress = "/resources/Product/?format=json&limit=0";
            /*   //keys
               this.category = jsonSingleProductObject["category"];

               var settings = new JsonSerializerSettings();
               settings.Converters.Add(new StringConverter());
           


               //this.categories = JsonConvert.DeserializeObject<List<string>>(category, settings);
            
               //this.category_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(category);
               this.establishment = jsonSingleProductObject["establishment"];
               this.establishment_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(establishment);
               this.productclass = jsonSingleProductObject["productclass"];
         //      this.productclass_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(productclass);
               this.brand = jsonSingleProductObject["brand"];
               this.brand_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(brand);

               //props
               this.name = jsonSingleProductObject["name"];
               this.price = Convert.ToInt32(jsonSingleProductObject["price"]);
               this.sku = jsonSingleProductObject["sku"];
               this.tax_included = jsonSingleProductObject["tax_included"];
               this.tax = jsonSingleProductObject["tax"];
             //  this.tax_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(tax);
               this.active = jsonSingleProductObject["active"];*/
        }





    }
}
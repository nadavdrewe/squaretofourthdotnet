using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public partial class Discount
    {

        public Discount()
        {

        }


        [Key]
        public int DBKEY_discount_id { get; set; }
        public bool active { get; set; }
        public int application_type { get; set; }
        public bool apply_to_base_product_only { get; set; }
        public bool apply_to_entire_application_type { get; set; }
        public bool auto_apply { get; set; }
        public string barcode { get; set; }
        public bool brand_level { get; set; }
        public string created_by { get; set; }
        public string created_date { get; set; }
        public int discount_amount { get; set; }
        public bool discount_at_item_level { get; set; }
        public bool discount_code { get; set; }
        public int discount_type { get; set; }
        public bool display_on_ipad { get; set; }
        public Nullable<DateTime> effective_from { get; set; }
        public Nullable<DateTime> effective_to { get; set; }
        public string establishment { get; set; }
        public int how_often_apply { get; set; }
        public int id { get; set; }
        public bool lock_enable { get; set; }
        public string lock_uuid { get; set; }
        public int maximum_off { get; set; }
        public int minimum_amount { get; set; }
        public object modifier { get; set; }
        public object modifier_class { get; set; }
        public string name { get; set; }
        public bool old_taxed_flag { get; set; }
        public bool password_required { get; set; }
        public object product { get; set; }
        public List<object> product_class { get; set; }
        public List<object> product_group { get; set; }
        public int qualification_subtype { get; set; }
        public int qualification_type { get; set; }
        public string resource_uri { get; set; }
        public bool taxed { get; set; }
        //public Timetable timetable { get; set; }
        public string updated_by { get; set; }
        public string updated_date { get; set; }
        //public object volume { get; set; }
        //public object volume_modifier { get; set; }
        //public object volume_modifier_class { get; set; }
        //public object volume_product { get; set; }
        //public List<object> volume_product_class { get; set; }
        //public List<object> volume_product_group { get; set; }
        //public int volume_type { get; set; }


        //added by ND for DB
        public int discount_id { get; set; }
        public int establishment_id { get; set; }



        public Discount(dynamic jsonSingleDiscountObject)
        {
            this.id = Convert.ToInt32((string)jsonSingleDiscountObject["id"]);
            this.discount_id = Convert.ToInt32((string)jsonSingleDiscountObject["id"]);
            this.name = (string)jsonSingleDiscountObject["name"];

            if ((bool)jsonSingleDiscountObject["active"] != null)
            {
                this.active = (bool)jsonSingleDiscountObject["active"];
            }

            if ((bool)jsonSingleDiscountObject["taxed"] != null)
            {
                this.taxed = (bool)jsonSingleDiscountObject["taxed"];
            }


            
            this.establishment = (string)jsonSingleDiscountObject["establishment"];
            this.discount_amount = Convert.ToInt32((string)jsonSingleDiscountObject["discount_amount"]);


            this.establishment_id =
                this.establishment_id =
                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                        (string)jsonSingleDiscountObject["establishment"]);
            
           /* if ((bool)jsonSingleDiscountObject["lock_enable"] != null)
            {
                this.lock_enable = (bool)jsonSingleDiscountObject["lock_enable"];
            }*/

            this.maximum_off = (string)jsonSingleDiscountObject["maximum_off"] != null ? Convert.ToInt32((string)jsonSingleDiscountObject["maximum_off"]) : 0;
           // this.minimum_amount = (string)jsonSingleDiscountObject["minimum_amount"] != null ? Convert.ToInt32((string)jsonSingleDiscountObject["minimum_amount"]) : 0;

        }


    }
}

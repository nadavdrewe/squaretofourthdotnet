using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Models.WebhookModels.Order
{
    [Table("OrderInfoes")]
    public class OrderInfo
    {

        [Key]
        public int DBKEY_OrderInfoId { get; set; }
        public int gratuity { get; set; }
        public bool web_order { get; set; }
        public bool has_items { get; set; }
        public object registry_data { get; set; }
        public object exchange_discount { get; set; }
        public string gift_reward_data { get; set; }
        public string uuid { get; set; }
        public string created_by { get; set; }
        public string service_fee_untaxed { get; set; }
        public string discount_tax_amount_included { get; set; }
        public int taxable_surcharge { get; set; }
        public int taxable_surcharge_excluded { get; set; }
        public bool sent { get; set; }
        public object exchanged { get; set; }
        public string updated_by { get; set; }
        [NotMapped]
        public List<object> applied_service_fee { get; set; }
        public object delivery_employee { get; set; }
        public int dining_option { get; set; }
        public object delivery_duration { get; set; }
        public bool crv_taxed { get; set; }
        public int surcharge_excluded { get; set; }
        public decimal discount_amount { get; set; }
        public int number_of_people { get; set; }
        public string notes { get; set; }
        [NotMapped]
        public List<object> bills { get; set; }
        public string pos_mode { get; set; }
        public bool ha_applied { get; set; }
        public object table_owner { get; set; }
        public string deleted_discounts { get; set; }
        public int points_added { get; set; }
        public string bills_info { get; set; }
        public decimal tax_excluded_amount { get; set; }
        public int auto_grat_pct { get; set; }
        public bool is_discounted { get; set; }
        public bool notification_email_sent { get; set; }
        public object delivery_distance { get; set; }
        public int gratuity_type { get; set; }
        [NotMapped]
        public List<string> orderhistory { get; set; }
        public bool closed { get; set; }
        public string tax_country { get; set; }
        public string discount_code { get; set; }
        public bool has_delivery_info { get; set; }
        public bool asap { get; set; }
        public object pickup_time { get; set; }
        public int reporting_id { get; set; }
        public int discount_nontaxable_surcharge_included { get; set; }

        //public string discount { get; set; }
        [NotMapped]
        public List<object> packages { get; set; }
        public object delivery_address { get; set; }
        public double final_total { get; set; }
        [NotMapped]
        public List<object> package { get; set; }
        public bool is_invoice { get; set; }
        public object delivery_clock_in { get; set; }
        public int bills_type { get; set; }
        public decimal? discount_tax_amount { get; set; }
        public int rounding_delta { get; set; }
        public bool is_unpaid { get; set; }
        public decimal tax_rebate { get; set; }
        public string table { get; set; }
        public string discount_reason { get; set; }
        public bool notification_text_sent { get; set; }
        public string last_updated_at { get; set; }
        public DateTime? customer_birthdate { get; set; }
        public string vehicle { get; set; }
        public int remaining_due { get; set; }
        [NotMapped]
        public List<AppliedTax> applied_taxes { get; set; }
        public int prevailing_tax { get; set; }
        public string delivery_clock_out { get; set; }
        public string discount_rule_type { get; set; }
        public int service_charge { get; set; }
        public string customer { get; set; }
        public string check_sum { get; set; }
        public string discount_taxed { get; set; }
        public bool has_history { get; set; }
        public double tax { get; set; }
        public int bill_number { get; set; }
        public int id { get; set; }
        public int surcharge { get; set; }
        public int points_redeemed { get; set; }
        public string call_number { get; set; }
        public string delivery_estimated_distance { get; set; }
        public object bill_parent { get; set; }
        public string establishment { get; set; }
        public DateTime updated_date { get; set; }
        public int prevailing_surcharge { get; set; }
        public string invoice_date { get; set; }
        public int crv_value { get; set; }
        public DateTime created_date { get; set; }
        public string call_name { get; set; }
        public bool printed { get; set; }
        public int subtotal { get; set; }
        public string discounted_by { get; set; }
        public string discount_rule_amount { get; set; }
        public bool is_readonly { get; set; }
        public string external_sync { get; set; }
        public string created_at { get; set; }
        public string service_fee_tax { get; set; }
        public decimal discount_total_amount { get; set; }
        public string local_id { get; set; }
        [NotMapped]
        public List<string> exchanged_by { get; set; }
        public string service_fee_taxed { get; set; }
        public string resource_uri { get; set; }

        [JsonIgnore]
        public DateTime WhenReceievedIntoAPI { get; set; }
        [JsonIgnore]
        public bool IsDuplicate { get; set; }
        [JsonIgnore]
        public bool Processed { get; set; }


        public List<OrderInfoPayment> Payments { get; set; }
        public List<OrderInfoItem> Items { get; set; }



        public class AppliedTax2
        {
            public int max_quantity_threshold { get; set; }
            public int min_quantity_threshold { get; set; }
            public string fiscal_rate { get; set; }
            public string name { get; set; }
            public string max_threshold { get; set; }
            public string min_threshold { get; set; }
            public int rounding_type { get; set; }
            public string uuid { get; set; }
            public string actual_tax_rate { get; set; }
            public string tax_amount { get; set; }
            public string tax_rate { get; set; }
            public bool on_full_price { get; set; }
            public object tax_table { get; set; }
            public string order_item { get; set; }
            public int local_tax_id { get; set; }
            public int id { get; set; }
            public string resource_uri { get; set; }
        }
    }


}
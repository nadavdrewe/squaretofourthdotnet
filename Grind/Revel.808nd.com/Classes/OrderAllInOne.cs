using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Revel._808nd.com.Classes
{
    public class OrderAllInOne : IRevelAddressable, IRevelCreateable
    {
        //  public List<string> applied_service_fee { get; set; }
        // public List<string> applied_taxes { get; set; }

        [Key]
        public int DBKEY_id { get; set; }
        public bool asap { get; set; }
        public double? auto_grat_pct { get; set; }
        public int bill_number { get; set; }
        public int? bill_parent { get; set; }
        public string bills_info { get; set; }
        public int bills_type { get; set; }
        public string call_name { get; set; }
        public string call_number { get; set; }
        public string check_sum { get; set; }
        public bool closed { get; set; }
        public string created_at { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public bool crv_taxed { get; set; }
        public decimal? crv_value { get; set; }
        public string customer { get; set; }
        public string customer_birthdate { get; set; }
        public string deleted_discounts { get; set; }
        public string delivery_address { get; set; }
        public string delivery_clock_in { get; set; }
        public string delivery_clock_out { get; set; }
        public string delivery_distance { get; set; }
        public string delivery_duration { get; set; }
        public string delivery_employee { get; set; }
        public string delivery_estimated_distance { get; set; }
        public int dining_option { get; set; }
        public string discount { get; set; }
        public Nullable<decimal> discount_amount { get; set; }
        public string discount_code { get; set; }
        public string discount_nontaxable_surcharge_included { get; set; }
        public string discount_reason { get; set; }
        public string discount_rule_amount { get; set; }
        public string discount_rule_type { get; set; }
        public string discount_tax_amount { get; set; }
        public decimal? discount_tax_amount_included { get; set; }
        public string discount_taxed { get; set; }
        public string discount_total_amount { get; set; }
        public string discounted_by { get; set; }
        public string establishment { get; set; }
        public string exchange_discount { get; set; }
        public string exchanged { get; set; }
        public string external_sync { get; set; }
        public decimal final_total { get; set; }
        public string gift_reward_data { get; set; }
        public decimal? gratuity { get; set; }
        public int? gratuity_type { get; set; }
        public bool ha_applied { get; set; }
        public bool has_delivery_info { get; set; }
        public bool has_history { get; set; }
        public bool has_items { get; set; }
        public int id { get; set; }
        public DateTime? invoice_date { get; set; }
        public bool is_discounted { get; set; }
        public bool is_invoice { get; set; }
        public bool is_readonly { get; set; }
        public bool is_unpaid { get; set; }
        public DateTime? last_updated_at { get; set; }
        public string local_id { get; set; }
        public string notes { get; set; }
        public bool notification_email_sent { get; set; }
        public bool notification_text_sent { get; set; }
        public int number_of_people { get; set; }
        public List<string> orderhistory { get; set; }
        //public List<object> package { get; set; } ??
        public DateTime? pickup_time { get; set; }
        public int points_added { get; set; }
        public int points_redeemed { get; set; }
        public string pos_mode { get; set; }
        public decimal? prevailing_surcharge { get; set; }
        public decimal? prevailing_tax { get; set; }
        public bool printed { get; set; }
        public string registry_data { get; set; }
        public decimal remaining_due { get; set; }
        public int? reporting_id { get; set; }
        public string resource_uri { get; set; }
        public decimal? rounding_delta { get; set; }
        public bool sent { get; set; }
        public decimal? service_charge { get; set; }
        public decimal? service_fee_tax { get; set; }
        public decimal? service_fee_taxed { get; set; }
        public decimal? service_fee_untaxed { get; set; }
        public decimal subtotal { get; set; }
        public decimal surcharge { get; set; }
        public double surcharge_excluded { get; set; }
        //public Table table { get; set; }
        public string table_owner { get; set; }
        public decimal tax { get; set; }
        public string tax_country { get; set; }
        public double tax_excluded_amount { get; set; }
        public double? tax_rebate { get; set; }
        public double? taxable_surcharge { get; set; }
        public double? taxable_surcharge_excluded { get; set; }
        public string updated_by { get; set; }
        public DateTime? updated_date { get; set; }
        public string uuid { get; set; }
        public string vehicle { get; set; }
        public bool web_order { get; set; }
        public string theAddress
        {
            get { return ""; }
            set { }
        }

        public int Create(dynamic Type)
        {
            return -1;
        }
    }

}


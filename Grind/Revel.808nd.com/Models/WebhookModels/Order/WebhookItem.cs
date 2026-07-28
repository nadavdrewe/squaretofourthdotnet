using Newtonsoft.Json;
using Revel._808nd.com.Models.WebhookModels.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Models.WebhookModels
{
    public class OrderInfoItem
    {
        [Key]
        public int DBKEY_Id { get; set; }
        public string gift_card_number { get; set; }
        public bool is_cold { get; set; }
        [NotMapped]
        public List<object> commissions { get; set; }
        public string price_to_display { get; set; }
        public object exchange_discount { get; set; }
        public object parent_uuid { get; set; }
        public string uuid { get; set; }
        public string created_by { get; set; }
        public string service_fee_untaxed { get; set; }
        public double pure_sales { get; set; }
        public object voided_by { get; set; }
        public bool sent { get; set; }
        public object exchanged { get; set; }
        public object dynamic_combo { get; set; }
        public object appointment { get; set; }
        public string updated_by { get; set; }
        public int split_with_seat { get; set; }
        public int dining_option { get; set; }
        public string product_name_override { get; set; }
        public object event_date { get; set; }
        public object discount_amount { get; set; }
        public double tax_amount { get; set; }
        public int split_parts { get; set; }
        public bool catering_complete { get; set; }
        public string sales_tax_exemption_reason { get; set; }
        public int weight { get; set; }
        public bool sold_by_weight { get; set; }
        public string cost { get; set; }
        public bool is_discounted { get; set; }
        public object appointment_ref_uuid { get; set; }
        public int course_number { get; set; }
        public int shared { get; set; }
        public string discount_code { get; set; }
        public bool deleted { get; set; }
        public string discount { get; set; }
        public decimal tax_rate { get; set; }
        public object dynamic_combo_slot { get; set; }
        public int seat_number { get; set; }
        public bool is_store_credit { get; set; }
        public object package_uuid { get; set; }
        public int cup_weight { get; set; }
        public object package { get; set; }
        public string modifier_cost { get; set; }
        public int initial_price { get; set; }
        public string combo_saving_amount { get; set; }
        public bool is_layaway { get; set; }
        public DateTime created_date { get; set; }
        public bool printed { get; set; }
        public int modifier_amount { get; set; }
        public int pump_number { get; set; }
        public object invoice_document_uuid { get; set; }
        public int tax_rebate { get; set; }
        public string special_request { get; set; }
        public string discount_reason { get; set; }
        public int temp_sort { get; set; }
        public bool exclude_from_discounts { get; set; }
        public string commission_amount { get; set; }
        public string serial_number { get; set; }
        public object catering_delivery_date { get; set; }
        public object discount_taxed { get; set; }
        [NotMapped]
        //public List<AppliedTax2> applied_taxes { get; set; }
        public object external_shipping_address { get; set; }
        public int ervc_type { get; set; }
        public object discount_rule_type { get; set; }
        public int cup_qty { get; set; }
        public List<object> modifieritems { get; set; }
        public int split_type { get; set; }
        public List<object> applied_service_fee { get; set; }
        public object kitchen_completed { get; set; }
        public object date_paid { get; set; }
        public bool taxed_flag { get; set; }
        public string uom { get; set; }
        public bool tax_included { get; set; }
        public string wholesale_saving_amount { get; set; }
        public object voided_date { get; set; }
        public object pump_date { get; set; }
        public string station { get; set; }
        public object expedited { get; set; }
        public int id { get; set; }
        public bool on_hold { get; set; }
        public bool not_returnable { get; set; }
        [NotMapped]
        public List<object> ingredientitems { get; set; }
        public object bill_parent { get; set; }
        public object service_provider { get; set; }
        public object start_time { get; set; }
        public DateTime updated_date { get; set; }
        public string product { get; set; }
        public object void_ref_uuid { get; set; }
        public object combo_used { get; set; }
        public int crv_value { get; set; }
        public int price { get; set; }
        public string voided_reason { get; set; }
        public object combo_uuid { get; set; }
        public string order_local_id { get; set; }
        public object reference_discount { get; set; }
        public object combo_product_set { get; set; }
        public object returned_establishment { get; set; }
        public object discounted_by { get; set; }
        public object discount_rule_amount { get; set; }
        public string service_fee_tax { get; set; }
        public int item_type { get; set; }
        public string resource_uri { get; set; }
        public string service_fee_taxed { get; set; }
        public string order { get; set; }
        public int quantity { get; set; }
        [JsonIgnore]
        public bool IsDuplicate { get; set; }


        public OrderInfo OrderInfo { get; set; }
    }
}

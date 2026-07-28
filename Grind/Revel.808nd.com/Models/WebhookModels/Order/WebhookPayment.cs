
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Models.WebhookModels.Order
{
    public class OrderInfoPayment
    {
        [Key]
        public int DBKEY_Id { get; set; }
        public DateTime updated_date { get; set; }
        public bool executed { get; set; }
        public string last_4_cc_digits { get; set; }
        public int currency_type { get; set; }
        public int rounding_delta { get; set; }
        public bool processor_accepted { get; set; }
        public object till_owner { get; set; }
        public int id { get; set; }
        public string currency_amount { get; set; }
        public DateTime payment_date { get; set; }
        public string uuid { get; set; }
        public int gratuity { get; set; }
        public string tip { get; set; }
        public string created_by { get; set; }
        public object refund_transaction_id { get; set; }
        public string station { get; set; }
        public object cc_first_name { get; set; }
        public bool online { get; set; }
        public string establishment { get; set; }
        public object house_account { get; set; }
        public string transaction_id { get; set; }
        public object exchanged { get; set; }
        public string updated_by { get; set; }
        public bool transaction_captured { get; set; }
        public object deleted { get; set; }
        public bool refunded { get; set; }
        public string transaction_data { get; set; }
        public object payer_id { get; set; }
        public int source_type { get; set; }
        public object processor_response { get; set; }
        public object first_4_cc_digits { get; set; }
        public string amount_authorized { get; set; }
        public object signature_img_url { get; set; }
        public string change { get; set; }
        public object invoice_transition_date { get; set; }
        public object other_payment_type { get; set; }
        public int cash_drawer { get; set; }
        public object receipt_email { get; set; }
        public int bill { get; set; }
        public string currency_tip { get; set; }
        public int card_type { get; set; }
        public string amount { get; set; }
        public int payment_type { get; set; }
        public DateTime created_date { get; set; }
        public object cc_last_name { get; set; }
        public object transaction_status { get; set; }
        public string order { get; set; }
        public string resource_uri { get; set; }
        [JsonIgnore]
        public bool IsDuplicate { get; set; }



        public OrderInfo OrderInfo { get; set; }
    }
}

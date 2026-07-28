using Newtonsoft.Json;
using Revel._808nd.com.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{

    public class HouseAccountPayment : IRevelAddressable, IRevelCreateable //v2 models
    {
        public decimal amount { get; set; }
        public decimal amount_authorized { get; set; }
        public int bill { get; set; }
        public int card_type { get; set; }
        public string cc_first_name { get; set; }
        public string cc_last_name { get; set; }
        public float change { get; set; }
        public DateTime created_date { get; set; }
        public int? customer_id { get; set; }
        public bool? deleted { get; set; }
        public string establishment { get; set; }
        public bool? exchanged { get; set; }
        public bool? executed { get; set; }
        public string first_4_cc_digits { get; set; }
        public decimal gratuity { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public Nullable<DateTime> invoice_transition_date { get; set; }
        public bool is_billed { get; set; }
        public bool is_paid { get; set; }
        public string last_4_cc_digits { get; set; }
        public bool online { get; set; }
        public string order { get; set; }
        public string order_local_id { get; set; }
        public string payer_id { get; set; }
        public DateTime? payment_date { get; set; }
        public int payment_type { get; set; }
        public bool? processor_accepted { get; set; }
        public bool? processor_response { get; set; }
        public string receipt_email { get; set; }
        public string refund_transaction_id { get; set; }
        public bool refunded { get; set; }
        public string resource_uri { get; set; }
        public int rounding_delta { get; set; }
        public string signature_img_url { get; set; }
        public int source_type { get; set; }
        public decimal tip { get; set; }
        public bool transaction_captured { get; set; }
        public string transaction_data { get; set; }
        public string transaction_id { get; set; }
        public string transaction_status { get; set; }
        public string updated_date { get; set; }
        public string uuid { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string theAddress { get => ""; set { } }

        public int Create(dynamic Type)
        {
            return -1;
        }
    }
}

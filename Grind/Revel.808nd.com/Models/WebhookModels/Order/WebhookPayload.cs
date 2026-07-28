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
    public class AppliedTax
    {
        public int max_quantity_threshold { get; set; }
        public int min_quantity_threshold { get; set; }
        public string fiscal_rate { get; set; }
        public string name { get; set; }
        public string max_threshold { get; set; }
        public string min_threshold { get; set; }
        public int rounding_type { get; set; }
        public string uuid { get; set; }
        public bool on_full_price { get; set; }
        public string order { get; set; }
        public bool is_prevailing { get; set; }
        public string tax_rate { get; set; }
        public object tax_table { get; set; }
        public int local_tax_id { get; set; }
        public int id { get; set; }
        public object dining_options { get; set; }
        public string resource_uri { get; set; }
    }

    
    

  

    public class OrderInfoHistory
    {
        public string order_closed_by { get; set; }
        public DateTime opened { get; set; }
        public string order_closed_at { get; set; }
        public string order_opened_by { get; set; }
        public string uuid { get; set; }
        public int id { get; set; }
        public DateTime closed { get; set; }
        public string order_opened_at { get; set; }
        public string order { get; set; }
        public string resource_uri { get; set; }
    }

    public class WebhookJSONRootObject
    {
        public List<object> documents { get; set; }
        public OrderInfo orderInfo { get; set; }
        public List<OrderInfoItem> items { get; set; }
        public List<object> shell_combo_items { get; set; }
        public List<OrderInfoPayment> payments { get; set; }
        public object order_exchange { get; set; }
        public List<OrderInfoHistory> history { get; set; }
    }

}

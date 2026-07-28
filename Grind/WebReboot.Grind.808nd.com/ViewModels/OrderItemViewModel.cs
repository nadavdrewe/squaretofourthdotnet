using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebReboot.Grind._808nd.com.ViewModels
{
    public class OrderItemViewModel
    {
        public int order_item_id { get; set; }
        public int product_id { get; set; }
        public int parent_order_id { get; set; }

    }
}

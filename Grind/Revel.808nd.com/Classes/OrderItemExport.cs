using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class OrderItemExport
    {
        [Key]
        public int DBKEY_orderitemExport_id { get; set; }
        public int DBKEY_orderitem_id { get; set; }
        public decimal price { get; set; }
        public decimal modifier_amount { get; set; }
        public decimal discount_amount { get; set; }
        public int quantity { get; set; }
        public decimal tax_amount { get; set; }
        public string voided_reason { get; set; }
        public DateTime? created_date { get; set; }
        public int db_brand_id { get; set; }
        public string sku { get; set; }
        public int db_establishment_id { get; set; }
        public int product_id { get; set; }
        public string product_name_override { get; set; }
        public decimal pure_sales { get; set; }
        public int establishment_id { get; set; }
    }
}

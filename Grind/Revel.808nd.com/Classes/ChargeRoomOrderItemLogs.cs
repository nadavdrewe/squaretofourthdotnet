using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    [Table("ChargeRoomOrderItemLogs")]
    public class ChargeRoomOrderItemLog
    {
        [Key]
        public int Id { get; set; }
        public string OrderItemId { get; set; }
        public string ParentOrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string SKU{ get; set; }
        public string Name { get; set; }
        public string ErrorMsg { get; set; }
        public DateTime Created { get; set; }

    }
}

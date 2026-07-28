using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    [Table("ChargeRoomOrderLog")]
    public class ChargeRoomOrderLog
    {
        [Key]
        public int Id { get; set; }
        public string OrderResourceUri { get; set; }
        public string Customer { get; set; }
        public bool Void { get; set; }
        public decimal OrderVAT { get; set; }
        public decimal OrderAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Establishment { get; set; }
        public string Establishment_Id { get; set; }
        public string RevenueCenter { get; set; }
        public string Message { get; set; }
        public string AdditionalMessage { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
        public DateTime Created { get; set; }

    }
}

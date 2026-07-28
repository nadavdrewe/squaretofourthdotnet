using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebReboot.Grind._808nd.com.ViewModels
{
    public class LogViewModel
    {
        public int order_id { get; set; }
        public DateTime created_date { get; set; }
        public string establishment { get; set; }
        public decimal point { get; set; }
        public string type_of_change { get; set; }
    }
}

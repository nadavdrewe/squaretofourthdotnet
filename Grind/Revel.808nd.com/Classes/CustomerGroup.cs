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
    public class CustomerGroup : IRevelAddressable, IRevelCreateable
    {
        public bool active { get; set; }
        public bool auto_apply_discounts { get; set; }
        public string brand { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public object discount_level { get; set; }
        public List<string> discounts { get; set; }
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string resource_uri { get; set; }
        public int sorting { get; set; }
        public List<string> tax_codes { get; set; }
        public bool tax_free { get; set; }
        public List<string> tax_free_taxes { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }

        [NotMapped]
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

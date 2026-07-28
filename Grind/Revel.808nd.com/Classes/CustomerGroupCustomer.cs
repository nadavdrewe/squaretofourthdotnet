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
    public class CustomerGroupCustomer : IRevelAddressable, IRevelCreateable //v2 models
    {
        public string customer { get; set; }
        public string customer_group { get; set; }
        [Key]
        public int id { get; set; }
        public string resource_uri { get; set; }

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

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
    [Table("ProductClasses")]
    public class ProductClass : IRevelAddressable, IRevelCreateable
    {
        public bool active { get; set; }
        public string admin_class_key { get; set; }
        public string brand { get; set; }
        public string created_by { get; set; }
        public DateTime created_date { get; set; }
        public bool deleted { get; set; }
        public bool exclude_from_discounts { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public bool is_admin_class { get; set; }
        public string name { get; set; }
        public string parent { get; set; }
        public string resource_uri { get; set; }
        public int sorting { get; set; }
        public string updated_by { get; set; }
        public DateTime updated_date { get; set; }

        public ProductClass()
        {
            theAddress = "/products/ProductClass/?format=json&limit=0";
        }

        [JsonIgnore]
        [NotMapped]
        public string theAddress { get; set; }

        public int Create(dynamic Type)
        {
            return -1;
        }
    }
}

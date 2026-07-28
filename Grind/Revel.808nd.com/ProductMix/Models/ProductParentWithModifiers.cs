using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ProductMix.Models
{
    public class ProductParentWithModifiers
    {
        public Productmix MainProduct { get; set; }
        public string MainProductName { get; set; }
        public IList<Productmix> Modifiers { get; set; } = new List<Productmix>();

    }


    public class ProductSkuAndName {
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }

    }
}

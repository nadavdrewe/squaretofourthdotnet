using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xero.railgunit.com.Taxes;

namespace xero.railgunit.com.Grind
{
    public class ProductClassTaxMapping
    {
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string TaxCode { get; set; }
    }



    /// <summary>
    /// Gives you back a Xero mapping if you provide a Revel Category
    /// </summary>
    public class RevelClassTaxMappingService
    {

        public ProductClassTaxMapping GetRevelTaxCodeForCategory(string revelCategory, bool isTaxed = false)
        {

            var taxCodes = XeroTaxCodeHelper.GetTaxCodes();

            //these are tax based codes
            if (isTaxed)
            {
                switch (revelCategory.Trim().ToLower())
                {
                    case "gift":
                        return new ProductClassTaxMapping { CategoryName = "gift", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "bar":
                        return new ProductClassTaxMapping { CategoryName = "bar", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "coffee/hot drinks":
                        return new ProductClassTaxMapping { CategoryName = "coffee/hot drinks", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "events":
                        return new ProductClassTaxMapping { CategoryName = "events", TaxCode = "" };
                    case "food":
                        return new ProductClassTaxMapping { CategoryName = "food", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "soft drinks":
                        return new ProductClassTaxMapping { CategoryName = "soft drinks", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "drinks":
                        return new ProductClassTaxMapping { CategoryName = "drinks", TaxCode = "" };
                    case "juice":
                        return new ProductClassTaxMapping { CategoryName = "juice", TaxCode = "" };
                    default:
                        throw new Exception("Couldnt' idenfity category string passed in to map tax code");

                }
            }
            else
            { //non tax based codes
                switch (revelCategory.Trim().ToLower())
                {
                    case "gift":
                        return new ProductClassTaxMapping { CategoryName = "gift", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "bar":
                        return new ProductClassTaxMapping { CategoryName = "bar", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "coffee/hot drinks":
                        return new ProductClassTaxMapping { CategoryName = "coffee/hot drinks", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "events":
                        return new ProductClassTaxMapping { CategoryName = "events", TaxCode = "" };
                    case "food":
                        return new ProductClassTaxMapping { CategoryName = "food", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "soft drinks":
                        return new ProductClassTaxMapping { CategoryName = "soft drinks", TaxCode = taxCodes.First(x => x.XeroValue == "OUTPUT2").XeroValue };
                    case "drinks":
                        return new ProductClassTaxMapping { CategoryName = "drinks", TaxCode = "" };
                    case "juice":
                        return new ProductClassTaxMapping { CategoryName = "juice", TaxCode = "" };
                    default:
                        throw new Exception("Couldnt' idenfity class string passed in to map tax code");

                }


            }

        }
    }
}

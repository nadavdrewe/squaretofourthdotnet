using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.Api.Core.Model;

namespace xero.railgunit.com.Grind
{
    public class InvoiceLineItemLocationMapping
    {
        TrackingCategory _trackingCat;

        public InvoiceLineItemLocationMapping(TrackingCategory trackingCat)
        {
            _trackingCat = trackingCat;
        }
        public ItemTrackingCategory GetRevelLocationCodeForCategory(string revelStoreId)
        {

            var returnValue = new ItemTrackingCategory();
            returnValue.Id = _trackingCat.Id;
            returnValue.Name = _trackingCat.Name;

            Option selected = new Option();

            switch (revelStoreId.Trim().ToLower())
            {
                //shore
                case "1":
                    selected = _trackingCat.Options.First(y => y.Name == "01 Shoreditch Grind");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //Soho
                case "3":
                    selected = _trackingCat.Options.First(y => y.Name == "02 Soho Grind");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //London
                case "4":
                    selected = _trackingCat.Options.First(y => y.Name == "04 London Grind");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //Holborn
                case "5":
                    selected = _trackingCat.Options.First(y => y.Name == "15 Hatton Garden");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //Royal
                case "6":
                    selected = _trackingCat.Options.First(y => y.Name == "10 Royal Exchange Grind");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //Covent
                case "7":
                    selected = _trackingCat.Options.First(y => y.Name == "08 Covent Grind");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //Clerkenwell
                case "8":
                    selected = _trackingCat.Options.First(y => y.Name == "09 Clerkenwell Grind");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //white 
                case "9":
                    selected = _trackingCat.Options.First(y => y.Name == "12 Whitechapel");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                //Exmpoth
                case "10":
                    selected = _trackingCat.Options.First(y => y.Name == "13 Exmouth Market");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
                case "11":
                    selected = _trackingCat.Options.First(y => y.Name == "16 Facebook");
                    returnValue.OptionId = selected.Id;
                    returnValue.Option = selected.Name;
                    break;
            }

            return returnValue;

            throw new Exception("Couldn't map Revel Location Code for store ID: " + revelStoreId);
        }
    }
}
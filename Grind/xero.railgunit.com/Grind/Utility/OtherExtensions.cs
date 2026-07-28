using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xero.railgunit.com.Grind.Utility
{
    public static class OtherExtensions
    {
        public static string GetGrindName(string grindName)
        {
            var replacedName = grindName.Replace(' ', '_');
            var saveName = String.Format("{0}_Operations.json", replacedName);
            return saveName;
        }

        public static IEnumerable<EstablishmentXeroMapping> GetAllGrindStoresForReportDownload()
        {
            return new List<EstablishmentXeroMapping> {
                new EstablishmentXeroMapping{ XeroContactName =  "Shoreditch",  EstablishmentId =  "1" },
                new EstablishmentXeroMapping{ XeroContactName =  "Soho",  EstablishmentId =  "3" },
                new EstablishmentXeroMapping{ XeroContactName =  "London Grind",  EstablishmentId =  "4" },
                new EstablishmentXeroMapping{ XeroContactName =  "Hatton",  EstablishmentId =  "5" },
                new EstablishmentXeroMapping{ XeroContactName =  "Royal",  EstablishmentId =  "6" },
                new EstablishmentXeroMapping{ XeroContactName =  "Covent",  EstablishmentId =  "7" },
                new EstablishmentXeroMapping{ XeroContactName =  "Clerkenwell",  EstablishmentId =  "8" },
                new EstablishmentXeroMapping{ XeroContactName = "Whitechapel",  EstablishmentId =  "9" },
                 new EstablishmentXeroMapping{ XeroContactName = "Exmouth",  EstablishmentId =  "10" }                            };
        }
    }
}

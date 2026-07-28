using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.OpsReporting
{

    /// <summary>
    /// Maps Revel Id to Selima Id
    /// </summary>
    public static class SelimaMappingService
    {
        public static int Map(int revelEstablishmentId)
        {
            int retval = 0;
            switch (revelEstablishmentId)
            {
                case 1:
                    retval = 14;
                    break;
                case 3:
                    retval = 16;
                    break;
                case 4:
                    retval = 11;
                    break;
                case 5:
                    retval = 8; //hatton
                    break;
                case 6:
                    retval = 13;
                    break;
                case 7:
                    retval = 2; //covent
                    break;
                case 8:
                    retval = 1; //clerkl
                    break;
                case 9:
                    retval = 17;
                    break;
                case 10:
                    retval = 4;
                    break;
                case 11:
                    retval = 6;
                    break;
                case 13:
                    retval = 7;
                    break;
                case 14:
                    retval = 18;
                    break;
                default:
                    throw new Exception("Couldn''t map Grind location to Selima Id");
            }

            return retval;

        }

    }
}

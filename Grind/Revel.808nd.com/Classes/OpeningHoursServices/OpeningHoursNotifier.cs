using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.OpeningHoursServices
{
    public class OpeningHoursNotifier
    {


        bool IsStoreOpeningLate(IHaveOpeningHours store, int minutesLateThatsAProblem, DateTime timeInQuestion)
        {
            try
            {

                if (timeInQuestion > store.OpeningDateTime.AddMinutes(minutesLateThatsAProblem))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                throw new OpeningHoursNotifierException("There was a problem caluclating opening hours");
            }

        }


        class OpeningHoursNotifierException : Exception
        {
            public OpeningHoursNotifierException(string message) : base(message)
            {

                
            }
        }


    }
}

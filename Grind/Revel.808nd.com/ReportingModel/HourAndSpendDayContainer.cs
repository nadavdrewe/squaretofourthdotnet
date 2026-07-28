using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel
{
    public class HourAndSpendDayContainer
    {
        public DateTime DayStart { get; set; }
        public DateTime DayEnd { get; set; }
        public IEnumerable<HourAndSpend> HourAndSpends { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel.ChartModels
{
    public class ChartWeekItemSales
    {
        public string CategoryName { get; set; }
        public string WeekDateStart { get; set; }
        public string Establishment{ get; set; }
        public List<ChartData> ChartData { get; set; }
        public string ChartColour { get; }

        public ChartWeekItemSales()
        {
            ChartData = new List<ChartData>();

            var random = new Random();
            var seed = random.Next(1, 100000);
            var rnd2 = new Random(seed);
            var color = String.Format("#{0:X6}", rnd2.Next(0x1000000)); // = "#A197B9"
            ChartColour = color;
        }

    }
}

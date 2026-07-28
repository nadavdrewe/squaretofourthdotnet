using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel.ChartModels
{
    public class StackedChartItemGrouping
    {
        public int NoOfWeeks { get; set; }
        public List<string> Types { get; set; } 
        public List<ChartData> Data { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.ReportingModel.ChartModels
{
    public class ChartData
    {
        public int WeekIdentifier { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime WeekStart { get; set; }
        public ChartData(string name, decimal value, int weekIdentifier, DateTime weekStart)
        {
            this.WeekIdentifier = weekIdentifier;
            this.Name = name;
            this.Value = value;
            WeekStart = weekStart;
        }
    }
}

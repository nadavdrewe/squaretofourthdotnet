using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geckoboard._808nd.com;

namespace GeckoboardLibrary.Classes.Widgets
{
    public class LineV2Widget : GeckoboardObject
    {
        public LineDataV2 data { get; set; }

        //This is aggregate root - init all related objects
        public LineV2Widget(string api_key, string pushURL, string chartName, GeckoboardChartAndItemType type, int id = 0) : base(api_key, pushURL, chartName, type)
        {
            this.data = new LineDataV2();

            data.series = new List<LineV2Series>();

            data.x_axis = new LineV2XAsis();
            data.x_axis.labels = new List<string>();

            data.y_axis = new LineV2YAxis();

        }

    }

    public class LineDataV2
    {
        public LineV2XAsis x_axis { get; set; }
        public LineV2YAxis y_axis { get; set; }
        public List<LineV2Series> series { get; set; }

    }


    public class LineV2YAxis
    {
        public string format { get; set; }
        public string unit { get; set; }
    }

    public class LineV2Series
    {
        public string name { get; set; }
        public List<decimal> data { get; set; }
    }

    public class LineV2XAsis
    {
        public List<string> labels { get; set; }
        public string type { get; set; }
    }
}

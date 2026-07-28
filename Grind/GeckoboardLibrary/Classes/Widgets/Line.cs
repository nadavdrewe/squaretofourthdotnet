using Geckoboard._808nd.com;
using GeckoboardLibrary.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geckoboard._808nd.com
{
    public class Line : GeckoboardObject
    {

        public LineData data { get; set; }     

        public Line()
            : base()
        {
            this.data = new LineData();
        }


        public Line(int id, string api_key, string pushURL, string chartName, GeckoboardChartAndItemType type, LineSettings settings)
        {
            //anything else?
            this.ID = id;
            this.data = new LineData();
            this.chartName = chartName;
            this.api_key = api_key;
            this.pushURL = pushURL;
            this.type = type;
            this.data.settings = settings;

        }

    }

    public class LineData
    {
        public List<decimal> item { get; set; }
        public LineSettings settings { get; set; }

    }



    public class LineSettings
    {
        public List<string> axisx { get; set; }
        public List<decimal> axisy { get; set; }
        public string colour { get; set; }
    }

    public class AxisX
    {
        public string axisx { get; set; }
    }

    public class AxisY
    {
        public decimal axisy { get; set; }
    }


}
